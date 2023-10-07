using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x0200009D RID: 157
public class MoviePlayerSample : MonoBehaviour
{
	// Token: 0x17000030 RID: 48
	// (get) Token: 0x06000360 RID: 864 RVA: 0x00014AE6 File Offset: 0x00012CE6
	// (set) Token: 0x06000361 RID: 865 RVA: 0x00014AEE File Offset: 0x00012CEE
	public bool IsPlaying { get; private set; }

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x06000362 RID: 866 RVA: 0x00014AF7 File Offset: 0x00012CF7
	// (set) Token: 0x06000363 RID: 867 RVA: 0x00014AFF File Offset: 0x00012CFF
	public long Duration { get; private set; }

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x06000364 RID: 868 RVA: 0x00014B08 File Offset: 0x00012D08
	// (set) Token: 0x06000365 RID: 869 RVA: 0x00014B10 File Offset: 0x00012D10
	public long PlaybackPosition { get; private set; }

	// Token: 0x06000366 RID: 870 RVA: 0x00014B1C File Offset: 0x00012D1C
	private void Awake()
	{
		Debug.Log("MovieSample Awake");
		this.mediaRenderer = base.GetComponent<Renderer>();
		this.videoPlayer = base.GetComponent<VideoPlayer>();
		if (this.videoPlayer == null)
		{
			this.videoPlayer = base.gameObject.AddComponent<VideoPlayer>();
		}
		this.videoPlayer.isLooping = this.LoopVideo;
		this.overlay = base.GetComponent<OVROverlay>();
		if (this.overlay == null)
		{
			this.overlay = base.gameObject.AddComponent<OVROverlay>();
		}
		this.overlay.enabled = false;
		this.overlay.isExternalSurface = NativeVideoPlayer.IsAvailable;
		this.overlay.enabled = (this.overlay.currentOverlayShape != OVROverlay.OverlayShape.Equirect || Application.platform == RuntimePlatform.Android);
	}

	// Token: 0x06000367 RID: 871 RVA: 0x00014BE7 File Offset: 0x00012DE7
	private bool IsLocalVideo(string movieName)
	{
		return !movieName.Contains("://");
	}

	// Token: 0x06000368 RID: 872 RVA: 0x00014BF8 File Offset: 0x00012DF8
	private void UpdateShapeAndStereo()
	{
		if (this.AutoDetectStereoLayout && this.overlay.isExternalSurface)
		{
			int videoWidth = NativeVideoPlayer.VideoWidth;
			int videoHeight = NativeVideoPlayer.VideoHeight;
			switch (NativeVideoPlayer.VideoStereoMode)
			{
			case NativeVideoPlayer.StereoMode.Unknown:
				if (videoWidth > videoHeight)
				{
					this.Stereo = MoviePlayerSample.VideoStereo.LeftRight;
				}
				else
				{
					this.Stereo = MoviePlayerSample.VideoStereo.TopBottom;
				}
				break;
			case NativeVideoPlayer.StereoMode.Mono:
				this.Stereo = MoviePlayerSample.VideoStereo.Mono;
				break;
			case NativeVideoPlayer.StereoMode.TopBottom:
				this.Stereo = MoviePlayerSample.VideoStereo.TopBottom;
				break;
			case NativeVideoPlayer.StereoMode.LeftRight:
				this.Stereo = MoviePlayerSample.VideoStereo.LeftRight;
				break;
			}
		}
		if (this.Shape != this._LastShape || this.Stereo != this._LastStereo || this.DisplayMono != this._LastDisplayMono)
		{
			Rect rect = new Rect(0f, 0f, 1f, 1f);
			switch (this.Shape)
			{
			case MoviePlayerSample.VideoShape._360:
				this.overlay.currentOverlayShape = OVROverlay.OverlayShape.Equirect;
				goto IL_118;
			case MoviePlayerSample.VideoShape._180:
				this.overlay.currentOverlayShape = OVROverlay.OverlayShape.Equirect;
				rect = new Rect(0.25f, 0f, 0.5f, 1f);
				goto IL_118;
			}
			this.overlay.currentOverlayShape = OVROverlay.OverlayShape.Quad;
			IL_118:
			this.overlay.overrideTextureRectMatrix = true;
			this.overlay.invertTextureRects = false;
			Rect rect2 = new Rect(0f, 0f, 1f, 1f);
			Rect rect3 = new Rect(0f, 0f, 1f, 1f);
			switch (this.Stereo)
			{
			case MoviePlayerSample.VideoStereo.TopBottom:
				rect2 = new Rect(0f, 0.5f, 1f, 0.5f);
				rect3 = new Rect(0f, 0f, 1f, 0.5f);
				break;
			case MoviePlayerSample.VideoStereo.LeftRight:
				rect2 = new Rect(0f, 0f, 0.5f, 1f);
				rect3 = new Rect(0.5f, 0f, 0.5f, 1f);
				break;
			case MoviePlayerSample.VideoStereo.BottomTop:
				rect2 = new Rect(0f, 0f, 1f, 0.5f);
				rect3 = new Rect(0f, 0.5f, 1f, 0.5f);
				break;
			}
			this.overlay.SetSrcDestRects(rect2, this.DisplayMono ? rect2 : rect3, rect, rect);
			this._LastDisplayMono = this.DisplayMono;
			this._LastStereo = this.Stereo;
			this._LastShape = this.Shape;
		}
	}

	// Token: 0x06000369 RID: 873 RVA: 0x00014E74 File Offset: 0x00013074
	private IEnumerator Start()
	{
		if (this.mediaRenderer.material == null)
		{
			Debug.LogError("No material for movie surface");
			yield break;
		}
		yield return new WaitForSeconds(1f);
		if (!string.IsNullOrEmpty(this.MovieName))
		{
			if (this.IsLocalVideo(this.MovieName))
			{
				this.Play(Application.streamingAssetsPath + "/" + this.MovieName, null);
			}
			else
			{
				this.Play(this.MovieName, this.DrmLicenseUrl);
			}
		}
		yield break;
	}

	// Token: 0x0600036A RID: 874 RVA: 0x00014E84 File Offset: 0x00013084
	public void Play(string moviePath, string drmLicencesUrl)
	{
		if (moviePath != string.Empty)
		{
			Debug.Log("Playing Video: " + moviePath);
			if (this.overlay.isExternalSurface)
			{
				OVROverlay.ExternalSurfaceObjectCreated externalSurfaceObjectCreated = delegate()
				{
					Debug.Log("Playing ExoPlayer with SurfaceObject");
					NativeVideoPlayer.PlayVideo(moviePath, drmLicencesUrl, this.overlay.externalSurfaceObject);
					NativeVideoPlayer.SetLooping(this.LoopVideo);
				};
				if (this.overlay.externalSurfaceObject == IntPtr.Zero)
				{
					this.overlay.externalSurfaceObjectCreated = externalSurfaceObjectCreated;
				}
				else
				{
					externalSurfaceObjectCreated();
				}
			}
			else
			{
				Debug.Log("Playing Unity VideoPlayer");
				this.videoPlayer.url = moviePath;
				this.videoPlayer.Prepare();
				this.videoPlayer.Play();
			}
			Debug.Log("MovieSample Start");
			this.IsPlaying = true;
			return;
		}
		Debug.LogError("No media file name provided");
	}

	// Token: 0x0600036B RID: 875 RVA: 0x00014F6A File Offset: 0x0001316A
	public void Play()
	{
		if (this.overlay.isExternalSurface)
		{
			NativeVideoPlayer.Play();
		}
		else
		{
			this.videoPlayer.Play();
		}
		this.IsPlaying = true;
	}

	// Token: 0x0600036C RID: 876 RVA: 0x00014F92 File Offset: 0x00013192
	public void Pause()
	{
		if (this.overlay.isExternalSurface)
		{
			NativeVideoPlayer.Pause();
		}
		else
		{
			this.videoPlayer.Pause();
		}
		this.IsPlaying = false;
	}

	// Token: 0x0600036D RID: 877 RVA: 0x00014FBC File Offset: 0x000131BC
	public void SeekTo(long position)
	{
		long num = Math.Max(0L, Math.Min(this.Duration, position));
		if (this.overlay.isExternalSurface)
		{
			NativeVideoPlayer.PlaybackPosition = num;
			return;
		}
		this.videoPlayer.time = (double)num / 1000.0;
	}

	// Token: 0x0600036E RID: 878 RVA: 0x00015008 File Offset: 0x00013208
	private void Update()
	{
		this.UpdateShapeAndStereo();
		if (!this.overlay.isExternalSurface)
		{
			Texture texture = (this.videoPlayer.texture != null) ? this.videoPlayer.texture : Texture2D.blackTexture;
			if (this.overlay.enabled)
			{
				if (this.overlay.textures[0] != texture)
				{
					this.overlay.enabled = false;
					this.overlay.textures[0] = texture;
					this.overlay.enabled = true;
				}
			}
			else
			{
				this.mediaRenderer.material.mainTexture = texture;
				this.mediaRenderer.material.SetVector("_SrcRectLeft", this.overlay.srcRectLeft.ToVector());
				this.mediaRenderer.material.SetVector("_SrcRectRight", this.overlay.srcRectRight.ToVector());
			}
			this.IsPlaying = this.videoPlayer.isPlaying;
			this.PlaybackPosition = (long)(this.videoPlayer.time * 1000.0);
			this.Duration = (long)(this.videoPlayer.length * 1000.0);
			return;
		}
		NativeVideoPlayer.SetListenerRotation(Camera.main.transform.rotation);
		this.IsPlaying = NativeVideoPlayer.IsPlaying;
		this.PlaybackPosition = NativeVideoPlayer.PlaybackPosition;
		this.Duration = NativeVideoPlayer.Duration;
		if (this.IsPlaying && (int)OVRManager.display.displayFrequency != 60)
		{
			OVRManager.display.displayFrequency = 60f;
			return;
		}
		if (!this.IsPlaying && (int)OVRManager.display.displayFrequency != 72)
		{
			OVRManager.display.displayFrequency = 72f;
		}
	}

	// Token: 0x0600036F RID: 879 RVA: 0x000151C5 File Offset: 0x000133C5
	public void SetPlaybackSpeed(float speed)
	{
		speed = Mathf.Max(0f, speed);
		if (this.overlay.isExternalSurface)
		{
			NativeVideoPlayer.SetPlaybackSpeed(speed);
			return;
		}
		this.videoPlayer.playbackSpeed = speed;
	}

	// Token: 0x06000370 RID: 880 RVA: 0x000151F4 File Offset: 0x000133F4
	public void Stop()
	{
		if (this.overlay.isExternalSurface)
		{
			NativeVideoPlayer.Stop();
		}
		else
		{
			this.videoPlayer.Stop();
		}
		this.IsPlaying = false;
	}

	// Token: 0x06000371 RID: 881 RVA: 0x0001521C File Offset: 0x0001341C
	private void OnApplicationPause(bool appWasPaused)
	{
		Debug.Log("OnApplicationPause: " + appWasPaused.ToString());
		if (appWasPaused)
		{
			this.videoPausedBeforeAppPause = !this.IsPlaying;
		}
		if (!this.videoPausedBeforeAppPause)
		{
			if (appWasPaused)
			{
				this.Pause();
				return;
			}
			this.Play();
		}
	}

	// Token: 0x04000402 RID: 1026
	private bool videoPausedBeforeAppPause;

	// Token: 0x04000403 RID: 1027
	private VideoPlayer videoPlayer;

	// Token: 0x04000404 RID: 1028
	private OVROverlay overlay;

	// Token: 0x04000405 RID: 1029
	private Renderer mediaRenderer;

	// Token: 0x04000409 RID: 1033
	private RenderTexture copyTexture;

	// Token: 0x0400040A RID: 1034
	private Material externalTex2DMaterial;

	// Token: 0x0400040B RID: 1035
	public string MovieName;

	// Token: 0x0400040C RID: 1036
	public string DrmLicenseUrl;

	// Token: 0x0400040D RID: 1037
	public bool LoopVideo;

	// Token: 0x0400040E RID: 1038
	public MoviePlayerSample.VideoShape Shape;

	// Token: 0x0400040F RID: 1039
	public MoviePlayerSample.VideoStereo Stereo;

	// Token: 0x04000410 RID: 1040
	public bool AutoDetectStereoLayout;

	// Token: 0x04000411 RID: 1041
	public bool DisplayMono;

	// Token: 0x04000412 RID: 1042
	private MoviePlayerSample.VideoShape _LastShape = (MoviePlayerSample.VideoShape)(-1);

	// Token: 0x04000413 RID: 1043
	private MoviePlayerSample.VideoStereo _LastStereo = (MoviePlayerSample.VideoStereo)(-1);

	// Token: 0x04000414 RID: 1044
	private bool _LastDisplayMono;

	// Token: 0x020003C5 RID: 965
	public enum VideoShape
	{
		// Token: 0x04001BDA RID: 7130
		_360,
		// Token: 0x04001BDB RID: 7131
		_180,
		// Token: 0x04001BDC RID: 7132
		Quad
	}

	// Token: 0x020003C6 RID: 966
	public enum VideoStereo
	{
		// Token: 0x04001BDE RID: 7134
		Mono,
		// Token: 0x04001BDF RID: 7135
		TopBottom,
		// Token: 0x04001BE0 RID: 7136
		LeftRight,
		// Token: 0x04001BE1 RID: 7137
		BottomTop
	}
}
