using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using Sirenix.OdinInspector;
using UnityEngine;

public class SynchedMusicController : MonoBehaviour
{
	private void Start()
	{
		if (this.usingNewSyncedSongsCode)
		{
			this.New_Start();
			return;
		}
		this.totalLoopTime = 0L;
		AudioSource[] array = this.audioSourceArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].mute = PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0;
		}
		this.audioSource.mute = PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0;
		this.muteButton.isOn = this.audioSource.mute;
		this.muteButton.UpdateColor();
		this.randomNumberGenerator = new Random(this.mySeed);
		this.GenerateSongStartRandomTimes();
		if (this.twoLayer)
		{
			array = this.audioSourceArray;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].clip.LoadAudioData();
			}
		}
	}

	private void Update()
	{
		if (this.usingNewSyncedSongsCode)
		{
			this.New_Update();
			return;
		}
		this.isPlayingCurrently = this.audioSource.isPlaying;
		if (this.testPlay)
		{
			this.testPlay = false;
			if (this.usingMultipleSources && this.usingMultipleSongs)
			{
				this.audioSource = this.audioSourceArray[Random.Range(0, this.audioSourceArray.Length)];
				this.audioSource.clip = this.songsArray[Random.Range(0, this.songsArray.Length)];
				this.audioSource.time = 0f;
			}
			if (this.twoLayer)
			{
				this.StartPlayingSongs(0L, 0L);
			}
			else if (this.audioSource.volume != 0f)
			{
				this.audioSource.Play();
			}
		}
		if (GorillaComputer.instance == null)
		{
			return;
		}
		if (GorillaComputer.instance.startupMillis != 0L)
		{
			if (this.totalLoopTime == 0L || this.songStartTimes.Length == 0)
			{
				return;
			}
			this.currentTime = (GorillaComputer.instance.startupMillis + (long)(Time.realtimeSinceStartup * 1000f)) % this.totalLoopTime;
			if (!this.audioSource.isPlaying)
			{
				if (this.lastPlayIndex >= 0 && this.songStartTimes[this.lastPlayIndex % this.songStartTimes.Length] < this.currentTime && this.currentTime < this.songStartTimes[(this.lastPlayIndex + 1) % this.songStartTimes.Length])
				{
					if (this.twoLayer)
					{
						if (this.songStartTimes[this.lastPlayIndex] + (long)(this.audioSource.clip.length * 1000f) > this.currentTime)
						{
							this.StartPlayingSongs(this.songStartTimes[this.lastPlayIndex], this.currentTime);
							return;
						}
					}
					else if (this.usingMultipleSongs && this.usingMultipleSources)
					{
						if (this.songStartTimes[this.lastPlayIndex] + (long)(this.songsArray[this.audioClipsForPlaying[this.lastPlayIndex]].length * 1000f) > this.currentTime)
						{
							this.StartPlayingSong(this.songStartTimes[this.lastPlayIndex], this.currentTime, this.songsArray[this.audioClipsForPlaying[this.lastPlayIndex]], this.audioSourceArray[this.audioSourcesForPlaying[this.lastPlayIndex]]);
							return;
						}
					}
					else if (this.songStartTimes[this.lastPlayIndex] + (long)(this.audioSource.clip.length * 1000f) > this.currentTime)
					{
						this.StartPlayingSong(this.songStartTimes[this.lastPlayIndex], this.currentTime);
						return;
					}
				}
				else
				{
					for (int i = 0; i < this.songStartTimes.Length; i++)
					{
						if (this.songStartTimes[i] > this.currentTime)
						{
							this.lastPlayIndex = (i - 1) % this.songStartTimes.Length;
							return;
						}
					}
				}
			}
		}
	}

	private void StartPlayingSong(long timeStarted, long currentTime)
	{
		if (this.audioSource.volume != 0f)
		{
			this.audioSource.Play();
		}
		this.audioSource.time = (float)(currentTime - timeStarted) / 1000f;
	}

	private void StartPlayingSongs(long timeStarted, long currentTime)
	{
		foreach (AudioSource audioSource in this.audioSourceArray)
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
		this.audioSource = sourceToPlay;
		sourceToPlay.clip = clipToPlay;
		if (sourceToPlay.isActiveAndEnabled && sourceToPlay.volume != 0f)
		{
			sourceToPlay.Play();
		}
		sourceToPlay.time = (float)(currentTime - timeStarted) / 1000f;
	}

	private void GenerateSongStartRandomTimes()
	{
		this.songStartTimes = new long[500];
		this.audioSourcesForPlaying = new int[500];
		this.audioClipsForPlaying = new int[500];
		this.songStartTimes[0] = this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		for (int i = 1; i < this.songStartTimes.Length; i++)
		{
			this.songStartTimes[i] = this.songStartTimes[i - 1] + this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		}
		if (this.usingMultipleSources)
		{
			for (int j = 0; j < this.audioSourcesForPlaying.Length; j++)
			{
				this.audioSourcesForPlaying[j] = this.randomNumberGenerator.Next(this.audioSourceArray.Length);
			}
		}
		if (this.usingMultipleSongs)
		{
			for (int k = 0; k < this.audioClipsForPlaying.Length; k++)
			{
				this.audioClipsForPlaying[k] = this.randomNumberGenerator.Next(this.songsArray.Length);
			}
		}
		if (this.usingMultipleSongs)
		{
			this.totalLoopTime = this.songStartTimes[this.songStartTimes.Length - 1] + (long)(this.songsArray[this.audioClipsForPlaying[this.audioClipsForPlaying.Length - 1]].length * 1000f);
			return;
		}
		if (this.audioSource.clip == null)
		{
			Debug.LogError("Using single-song flow, audio sources should have the song they're going to play already attached!");
		}
		this.totalLoopTime = this.songStartTimes[this.songStartTimes.Length - 1] + (long)(this.audioSource.clip.length * 1000f);
	}

	public void MuteAudio(GorillaPressableButton pressedButton)
	{
		AudioSource[] array;
		if (this.audioSource.mute)
		{
			PlayerPrefs.SetInt(this.locationName + "Muted", 0);
			PlayerPrefs.Save();
			this.audioSource.mute = false;
			array = this.audioSourceArray;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].mute = false;
			}
			pressedButton.isOn = false;
			pressedButton.UpdateColor();
			return;
		}
		PlayerPrefs.SetInt(this.locationName + "Muted", 1);
		PlayerPrefs.Save();
		this.audioSource.mute = true;
		array = this.audioSourceArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].mute = true;
		}
		pressedButton.isOn = true;
		pressedButton.UpdateColor();
	}

	protected void New_Start()
	{
		this.totalLoopTime = 0L;
		AudioSource[] array = this.audioSourceArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].mute = PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0;
		}
		this.audioSource.mute = PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0;
		this.muteButton.isOn = this.audioSource.mute;
		this.muteButton.UpdateColor();
		this.randomNumberGenerator = new Random(this.mySeed);
		this.New_GeneratePlaylistArrays();
		foreach (SynchedMusicController.SyncedSongInfo syncedSongInfo in this.syncedSongs)
		{
			if (syncedSongInfo.songLayers.Length > 1)
			{
				SynchedMusicController.SyncedSongLayerInfo[] songLayers = syncedSongInfo.songLayers;
				for (int j = 0; j < songLayers.Length; j++)
				{
					songLayers[j].audioClip.LoadAudioData();
				}
			}
		}
		string text = this.New_Validate();
		if (text.Length > 0)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Disabling SynchedMusicController on ",
				base.name,
				" due to invalid setup: ",
				text,
				" Path: ",
				this.GetComponentPath(int.MaxValue)
			}), this);
		}
	}

	protected void OnEnable()
	{
		this.lastPlayIndex = -1;
	}

	protected void New_Update()
	{
		if (!GorillaComputer.hasInstance)
		{
			return;
		}
		long startupMillis = GorillaComputer.instance.startupMillis;
		if (startupMillis <= 0L)
		{
			return;
		}
		long num = (startupMillis + (long)(Time.realtimeSinceStartup * 1000f)) % this.totalLoopTime;
		bool flag = false;
		if (this.lastPlayIndex < 0)
		{
			flag = true;
			for (int i = 1; i < 256; i++)
			{
				if (this.songStartTimes[i] > num)
				{
					this.lastPlayIndex = (i - 1) % 256;
					break;
				}
			}
			if (this.lastPlayIndex < 0)
			{
				this.lastPlayIndex = 255;
			}
		}
		int num2 = (this.lastPlayIndex + 1) % 256;
		if (this.songStartTimes[num2] < num)
		{
			this.lastPlayIndex = num2;
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		long num3 = this.songStartTimes[this.lastPlayIndex];
		SynchedMusicController.SyncedSongInfo syncedSongInfo = this.syncedSongs[this.audioClipsForPlaying[this.lastPlayIndex]];
		float length = syncedSongInfo.songLayers[0].audioClip.length;
		float num4 = (float)(num - num3) / 1000f;
		if (num4 < 0f || length < num4)
		{
			return;
		}
		for (int j = 0; j < syncedSongInfo.songLayers.Length; j++)
		{
			SynchedMusicController.SyncedSongLayerInfo syncedSongLayerInfo = syncedSongInfo.songLayers[j];
			if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.All)
			{
				foreach (AudioSource audioSource in this.audioSourceArray)
				{
					audioSource.clip = syncedSongLayerInfo.audioClip;
					if (audioSource.volume > 0f)
					{
						audioSource.Play();
					}
					audioSource.time = num4;
				}
			}
			else if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Shuffle)
			{
				AudioSource audioSource2 = this.audioSourceArray[this.audioSourcesForPlaying[this.lastPlayIndex]];
				audioSource2.clip = syncedSongLayerInfo.audioClip;
				if (audioSource2.volume > 0f)
				{
					audioSource2.Play();
				}
				audioSource2.time = num4;
			}
			else if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Specific)
			{
				foreach (AudioSource audioSource3 in syncedSongLayerInfo.audioSources)
				{
					audioSource3.clip = syncedSongLayerInfo.audioClip;
					if (audioSource3.volume > 0f)
					{
						audioSource3.Play();
					}
					audioSource3.time = num4;
				}
			}
		}
	}

	private string New_Validate()
	{
		if (this.syncedSongs == null)
		{
			return "syncedSongs array cannot be null.";
		}
		if (this.syncedSongs.Length == 0)
		{
			return "syncedSongs array cannot be empty.";
		}
		for (int i = 0; i < this.syncedSongs.Length; i++)
		{
			SynchedMusicController.SyncedSongInfo syncedSongInfo = this.syncedSongs[i];
			if (syncedSongInfo.songLayers == null)
			{
				return string.Format("Song {0}'s songLayers array is null.", i);
			}
			if (syncedSongInfo.songLayers.Length == 0)
			{
				return string.Format("Song {0}'s songLayers array is empty.", i);
			}
			for (int j = 0; j < syncedSongInfo.songLayers.Length; j++)
			{
				SynchedMusicController.SyncedSongLayerInfo syncedSongLayerInfo = syncedSongInfo.songLayers[j];
				if (syncedSongLayerInfo.audioClip == null)
				{
					return string.Format("Song {0}'s song layer {1} does not have an audio clip.", i, j);
				}
				if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Specific)
				{
					if (syncedSongLayerInfo.audioSources == null || syncedSongLayerInfo.audioSources.Length == 0)
					{
						return string.Format("Song {0}'s song layer {1} has audioSourcePickMode set to {2} ", i, j, syncedSongLayerInfo.audioSourcePickMode) + "but layer's audioSources array is empty or null.";
					}
				}
				else if (this.audioSourceArray == null || this.audioSourceArray.Length == 0)
				{
					return string.Format("{0} is null or empty, while Song {1}'s song layer {2} has ", "audioSourceArray", i, j) + string.Format("audioSourcePickMode set to {0}, which uses the ", syncedSongLayerInfo.audioSourcePickMode) + "component's audioSourceArray.";
				}
			}
		}
		return string.Empty;
	}

	private void New_GeneratePlaylistArrays()
	{
		this.songStartTimes = new long[256];
		this.songStartTimes[0] = this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		for (int i = 1; i < this.songStartTimes.Length; i++)
		{
			this.songStartTimes[i] = this.songStartTimes[i - 1] + this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		}
		this.audioSourcesForPlaying = new int[256];
		bool flag = false;
		SynchedMusicController.SyncedSongInfo[] array = this.syncedSongs;
		for (int j = 0; j < array.Length; j++)
		{
			SynchedMusicController.SyncedSongLayerInfo[] songLayers = array[j].songLayers;
			for (int k = 0; k < songLayers.Length; k++)
			{
				if (songLayers[k].audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Shuffle)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			for (int l = 0; l < this.audioSourcesForPlaying.Length; l++)
			{
				this.audioSourcesForPlaying[l] = this.randomNumberGenerator.Next(this.audioSourceArray.Length);
			}
		}
		this.audioClipsForPlaying = new int[256];
		for (int m = 0; m < this.audioClipsForPlaying.Length; m++)
		{
			if (this.shufflePlaylist)
			{
				this.audioClipsForPlaying[m] = this.randomNumberGenerator.Next(this.syncedSongs.Length);
			}
			else
			{
				this.audioClipsForPlaying[m] = m % this.syncedSongs.Length;
			}
		}
		long num = (long)this.syncedSongs[this.audioClipsForPlaying[this.audioClipsForPlaying.Length - 1]].songLayers[0].audioClip.length * 1000L;
		long num2 = this.songStartTimes[this.songStartTimes.Length - 1];
		this.totalLoopTime = num + num2;
	}

	[SerializeField]
	private bool usingNewSyncedSongsCode;

	[SerializeField]
	private bool shufflePlaylist = true;

	[SerializeField]
	private SynchedMusicController.SyncedSongInfo[] syncedSongs;

	[Tooltip("This should be unique per sound post. Sound posts that share the same seed and the same song count will play songs a the same times.")]
	public int mySeed;

	private Random randomNumberGenerator = new Random();

	[Tooltip("In milliseconds.")]
	public long minimumWait = 900000L;

	[Tooltip("In milliseconds. A random value between 0 and this will be picked. The max wait time is randomInterval + minimumWait.")]
	public int randomInterval = 600000;

	[DebugReadout]
	public long[] songStartTimes;

	[DebugReadout]
	public int[] audioSourcesForPlaying;

	[DebugReadout]
	public int[] audioClipsForPlaying;

	public AudioSource audioSource;

	public AudioSource[] audioSourceArray;

	public AudioClip[] songsArray;

	[DebugReadout]
	public int lastPlayIndex;

	[DebugReadout]
	public long currentTime;

	[DebugReadout]
	public long totalLoopTime;

	public GorillaPressableButton muteButton;

	public bool usingMultipleSongs;

	public bool usingMultipleSources;

	[DebugReadout]
	public bool isPlayingCurrently;

	[DebugReadout]
	public bool testPlay;

	public bool twoLayer;

	[Tooltip("Used to store the muted sound posts in player prefs.")]
	public string locationName;

	private const int kPlaylistLength = 256;

	[Serializable]
	public struct SyncedSongInfo
	{
		[Tooltip("A layer for a song. For no layers, just add a single entry.")]
		[RequiredListLength(1, null)]
		public SynchedMusicController.SyncedSongLayerInfo[] songLayers;
	}

	[Serializable]
	public struct SyncedSongLayerInfo
	{
		[Tooltip("The clip that will be played.")]
		public AudioClip audioClip;

		public SynchedMusicController.AudioSourcePickMode audioSourcePickMode;

		[Tooltip("")]
		public AudioSource[] audioSources;
	}

	public enum AudioSourcePickMode
	{
		All,
		Shuffle,
		Specific
	}
}
