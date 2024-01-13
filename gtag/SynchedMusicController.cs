using System;
using GorillaNetworking;
using GorillaTag;
using UnityEngine;

public class SynchedMusicController : MonoBehaviour
{
	public int mySeed;

	public System.Random randomNumberGenerator = new System.Random();

	[Tooltip("In milliseconds.")]
	public long minimumWait = 900000L;

	[Tooltip("In milliseconds. A random value between 0 and this will be picked. The max wait time is randomInterval + minimumWait.")]
	public int randomInterval = 600000;

	[DebugOption]
	public long[] songStartTimes;

	public int[] audioSourcesForPlaying;

	public int[] audioClipsForPlaying;

	public AudioSource audioSource;

	public AudioSource[] audioSourceArray;

	public AudioClip[] songsArray;

	[DebugOption]
	public int lastPlayIndex;

	[DebugOption]
	public long currentTime;

	[DebugOption]
	public long totalLoopTime;

	public GorillaPressableButton muteButton;

	public bool usingMultipleSongs;

	public bool usingMultipleSources;

	[DebugReadout]
	public bool isPlayingCurrently;

	[DebugOption]
	public bool testPlay;

	public bool twoLayer;

	public string locationName;

	private void Start()
	{
		totalLoopTime = 0L;
		AudioSource[] array = audioSourceArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].mute = PlayerPrefs.GetInt(locationName + "Muted", 0) != 0;
		}
		audioSource.mute = PlayerPrefs.GetInt(locationName + "Muted", 0) != 0;
		muteButton.isOn = audioSource.mute;
		muteButton.UpdateColor();
		randomNumberGenerator = new System.Random(mySeed);
		GenerateSongStartRandomTimes();
		if (twoLayer)
		{
			array = audioSourceArray;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].clip.LoadAudioData();
			}
		}
	}

	private void Update()
	{
		isPlayingCurrently = audioSource.isPlaying;
		if (testPlay)
		{
			testPlay = false;
			if (usingMultipleSources && usingMultipleSongs)
			{
				audioSource = audioSourceArray[UnityEngine.Random.Range(0, audioSourceArray.Length)];
				audioSource.clip = songsArray[UnityEngine.Random.Range(0, songsArray.Length)];
				audioSource.time = 0f;
			}
			if (twoLayer)
			{
				StartPlayingSongs(0L, 0L);
			}
			else if (audioSource.volume != 0f)
			{
				audioSource.Play();
			}
		}
		if (GorillaComputer.instance.startupMillis == 0L || totalLoopTime == 0L || songStartTimes.Length == 0)
		{
			return;
		}
		currentTime = (GorillaComputer.instance.startupMillis + (long)(Time.realtimeSinceStartup * 1000f)) % totalLoopTime;
		if (audioSource.isPlaying)
		{
			return;
		}
		if (lastPlayIndex >= 0 && songStartTimes[lastPlayIndex % songStartTimes.Length] < currentTime && currentTime < songStartTimes[(lastPlayIndex + 1) % songStartTimes.Length])
		{
			if (twoLayer)
			{
				if (songStartTimes[lastPlayIndex] + (long)(audioSource.clip.length * 1000f) > currentTime)
				{
					StartPlayingSongs(songStartTimes[lastPlayIndex], currentTime);
				}
			}
			else if (usingMultipleSongs && usingMultipleSources)
			{
				if (songStartTimes[lastPlayIndex] + (long)(songsArray[audioClipsForPlaying[lastPlayIndex]].length * 1000f) > currentTime)
				{
					StartPlayingSong(songStartTimes[lastPlayIndex], currentTime, songsArray[audioClipsForPlaying[lastPlayIndex]], audioSourceArray[audioSourcesForPlaying[lastPlayIndex]]);
				}
			}
			else if (songStartTimes[lastPlayIndex] + (long)(audioSource.clip.length * 1000f) > currentTime)
			{
				StartPlayingSong(songStartTimes[lastPlayIndex], currentTime);
			}
			return;
		}
		for (int i = 0; i < songStartTimes.Length; i++)
		{
			if (songStartTimes[i] > currentTime)
			{
				lastPlayIndex = (i - 1) % songStartTimes.Length;
				break;
			}
		}
	}

	private void StartPlayingSong(long timeStarted, long currentTime)
	{
		if (audioSource.volume != 0f)
		{
			audioSource.Play();
		}
		audioSource.time = (float)(currentTime - timeStarted) / 1000f;
	}

	private void StartPlayingSongs(long timeStarted, long currentTime)
	{
		AudioSource[] array = audioSourceArray;
		foreach (AudioSource audioSource in array)
		{
			if (audioSource.volume != 0f)
			{
				audioSource.Play();
			}
			audioSource.time = (float)(currentTime - timeStarted) / 1000f;
		}
	}

	private void StartPlayingSong(long timeStarted, long currentTime, AudioClip clipToPlay, AudioSource sourceToPlay)
	{
		audioSource = sourceToPlay;
		sourceToPlay.clip = clipToPlay;
		if (sourceToPlay.isActiveAndEnabled && sourceToPlay.volume != 0f)
		{
			sourceToPlay.Play();
		}
		sourceToPlay.time = (float)(currentTime - timeStarted) / 1000f;
	}

	private void GenerateSongStartRandomTimes()
	{
		songStartTimes = new long[500];
		audioSourcesForPlaying = new int[500];
		audioClipsForPlaying = new int[500];
		songStartTimes[0] = minimumWait + randomNumberGenerator.Next(randomInterval);
		for (int i = 1; i < songStartTimes.Length; i++)
		{
			songStartTimes[i] = songStartTimes[i - 1] + minimumWait + randomNumberGenerator.Next(randomInterval);
		}
		if (usingMultipleSources)
		{
			for (int j = 0; j < audioSourcesForPlaying.Length; j++)
			{
				audioSourcesForPlaying[j] = randomNumberGenerator.Next(audioSourceArray.Length);
			}
		}
		if (usingMultipleSongs)
		{
			for (int k = 0; k < audioClipsForPlaying.Length; k++)
			{
				audioClipsForPlaying[k] = randomNumberGenerator.Next(songsArray.Length);
			}
		}
		if (usingMultipleSongs)
		{
			totalLoopTime = songStartTimes[songStartTimes.Length - 1] + (long)(songsArray[audioClipsForPlaying[audioClipsForPlaying.Length - 1]].length * 1000f);
			return;
		}
		if (audioSource.clip == null)
		{
			Debug.LogError("Using single-song flow, audio sources should have the song they're going to play already attached!");
		}
		totalLoopTime = songStartTimes[songStartTimes.Length - 1] + (long)(audioSource.clip.length * 1000f);
	}

	public void MuteAudio(GorillaPressableButton pressedButton)
	{
		if (audioSource.mute)
		{
			PlayerPrefs.SetInt(locationName + "Muted", 0);
			PlayerPrefs.Save();
			audioSource.mute = false;
			AudioSource[] array = audioSourceArray;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].mute = false;
			}
			pressedButton.isOn = false;
			pressedButton.UpdateColor();
		}
		else
		{
			PlayerPrefs.SetInt(locationName + "Muted", 1);
			PlayerPrefs.Save();
			audioSource.mute = true;
			AudioSource[] array = audioSourceArray;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].mute = true;
			}
			pressedButton.isOn = true;
			pressedButton.UpdateColor();
		}
	}
}
