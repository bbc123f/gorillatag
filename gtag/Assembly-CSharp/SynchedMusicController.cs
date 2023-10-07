﻿using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x020001A4 RID: 420
public class SynchedMusicController : MonoBehaviour
{
	// Token: 0x06000ACA RID: 2762 RVA: 0x000424E8 File Offset: 0x000406E8
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
			array[i].mute = (PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0);
		}
		this.audioSource.mute = (PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0);
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

	// Token: 0x06000ACB RID: 2763 RVA: 0x000425CC File Offset: 0x000407CC
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

	// Token: 0x06000ACC RID: 2764 RVA: 0x0004289D File Offset: 0x00040A9D
	private void StartPlayingSong(long timeStarted, long currentTime)
	{
		if (this.audioSource.volume != 0f)
		{
			this.audioSource.Play();
		}
		this.audioSource.time = (float)(currentTime - timeStarted) / 1000f;
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x000428D4 File Offset: 0x00040AD4
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

	// Token: 0x06000ACE RID: 2766 RVA: 0x00042920 File Offset: 0x00040B20
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

	// Token: 0x06000ACF RID: 2767 RVA: 0x0004296C File Offset: 0x00040B6C
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

	// Token: 0x06000AD0 RID: 2768 RVA: 0x00042B08 File Offset: 0x00040D08
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

	// Token: 0x06000AD1 RID: 2769 RVA: 0x00042BC8 File Offset: 0x00040DC8
	protected void New_Start()
	{
		this.totalLoopTime = 0L;
		AudioSource[] array = this.audioSourceArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].mute = (PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0);
		}
		this.audioSource.mute = (PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0);
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

	// Token: 0x06000AD2 RID: 2770 RVA: 0x00042D1D File Offset: 0x00040F1D
	protected void OnEnable()
	{
		this.lastPlayIndex = -1;
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x00042D28 File Offset: 0x00040F28
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

	// Token: 0x06000AD4 RID: 2772 RVA: 0x00042F74 File Offset: 0x00041174
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

	// Token: 0x06000AD5 RID: 2773 RVA: 0x000430DC File Offset: 0x000412DC
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

	// Token: 0x04000D91 RID: 3473
	[SerializeField]
	private bool usingNewSyncedSongsCode;

	// Token: 0x04000D92 RID: 3474
	[SerializeField]
	private bool shufflePlaylist = true;

	// Token: 0x04000D93 RID: 3475
	[SerializeField]
	private SynchedMusicController.SyncedSongInfo[] syncedSongs;

	// Token: 0x04000D94 RID: 3476
	[Tooltip("This should be unique per sound post. Sound posts that share the same seed and the same song count will play songs a the same times.")]
	public int mySeed;

	// Token: 0x04000D95 RID: 3477
	private Random randomNumberGenerator = new Random();

	// Token: 0x04000D96 RID: 3478
	[Tooltip("In milliseconds.")]
	public long minimumWait = 900000L;

	// Token: 0x04000D97 RID: 3479
	[Tooltip("In milliseconds. A random value between 0 and this will be picked. The max wait time is randomInterval + minimumWait.")]
	public int randomInterval = 600000;

	// Token: 0x04000D98 RID: 3480
	[DebugReadout]
	public long[] songStartTimes;

	// Token: 0x04000D99 RID: 3481
	[DebugReadout]
	public int[] audioSourcesForPlaying;

	// Token: 0x04000D9A RID: 3482
	[DebugReadout]
	public int[] audioClipsForPlaying;

	// Token: 0x04000D9B RID: 3483
	public AudioSource audioSource;

	// Token: 0x04000D9C RID: 3484
	public AudioSource[] audioSourceArray;

	// Token: 0x04000D9D RID: 3485
	public AudioClip[] songsArray;

	// Token: 0x04000D9E RID: 3486
	[DebugReadout]
	public int lastPlayIndex;

	// Token: 0x04000D9F RID: 3487
	[DebugReadout]
	public long currentTime;

	// Token: 0x04000DA0 RID: 3488
	[DebugReadout]
	public long totalLoopTime;

	// Token: 0x04000DA1 RID: 3489
	public GorillaPressableButton muteButton;

	// Token: 0x04000DA2 RID: 3490
	public bool usingMultipleSongs;

	// Token: 0x04000DA3 RID: 3491
	public bool usingMultipleSources;

	// Token: 0x04000DA4 RID: 3492
	[DebugReadout]
	public bool isPlayingCurrently;

	// Token: 0x04000DA5 RID: 3493
	[DebugReadout]
	public bool testPlay;

	// Token: 0x04000DA6 RID: 3494
	public bool twoLayer;

	// Token: 0x04000DA7 RID: 3495
	[Tooltip("Used to store the muted sound posts in player prefs.")]
	public string locationName;

	// Token: 0x04000DA8 RID: 3496
	private const int kPlaylistLength = 256;

	// Token: 0x02000446 RID: 1094
	[Serializable]
	public struct SyncedSongInfo
	{
		// Token: 0x04001DB1 RID: 7601
		[Tooltip("A layer for a song. For no layers, just add a single entry.")]
		[RequiredListLength(1, null)]
		public SynchedMusicController.SyncedSongLayerInfo[] songLayers;
	}

	// Token: 0x02000447 RID: 1095
	[Serializable]
	public struct SyncedSongLayerInfo
	{
		// Token: 0x04001DB2 RID: 7602
		[Tooltip("The clip that will be played.")]
		public AudioClip audioClip;

		// Token: 0x04001DB3 RID: 7603
		public SynchedMusicController.AudioSourcePickMode audioSourcePickMode;

		// Token: 0x04001DB4 RID: 7604
		[Tooltip("")]
		public AudioSource[] audioSources;
	}

	// Token: 0x02000448 RID: 1096
	public enum AudioSourcePickMode
	{
		// Token: 0x04001DB6 RID: 7606
		All,
		// Token: 0x04001DB7 RID: 7607
		Shuffle,
		// Token: 0x04001DB8 RID: 7608
		Specific
	}
}
