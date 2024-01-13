using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class MoviePlayerSample : MonoBehaviour
{
	public enum VideoShape
	{
		_360,
		_180,
		Quad
	}

	public enum VideoStereo
	{
		Mono,
		TopBottom,
		LeftRight,
		BottomTop
	}

	private bool videoPausedBeforeAppPause;

	private VideoPlayer videoPlayer;

	private OVROverlay overlay;

	private Renderer mediaRenderer;

	private RenderTexture copyTexture;

	private Material externalTex2DMaterial;

	public string MovieName;

	public string DrmLicenseUrl;

	public bool LoopVideo;

	public VideoShape Shape;

	public VideoStereo Stereo;

	public bool DisplayMono;

	private VideoShape _LastShape = (VideoShape)(-1);

	private VideoStereo _LastStereo = (VideoStereo)(-1);

	private bool _LastDisplayMono;

	public bool IsPlaying { get; private set; }

	public long Duration { get; private set; }

	public long PlaybackPosition { get; private set; }

	private void Awake()
	{
		Debug.Log("MovieSample Awake");
		mediaRenderer = GetComponent<Renderer>();
		videoPlayer = GetComponent<VideoPlayer>();
		if (videoPlayer == null)
		{
			videoPlayer = base.gameObject.AddComponent<VideoPlayer>();
		}
		videoPlayer.isLooping = LoopVideo;
		overlay = GetComponent<OVROverlay>();
		if (overlay == null)
		{
			overlay = base.gameObject.AddComponent<OVROverlay>();
		}
		overlay.enabled = false;
		overlay.isExternalSurface = NativeVideoPlayer.IsAvailable;
		overlay.enabled = overlay.currentOverlayShape != OVROverlay.OverlayShape.Equirect || Application.platform == RuntimePlatform.Android;
	}

	private bool IsLocalVideo(string movieName)
	{
		return !movieName.Contains("://");
	}

	private void UpdateShapeAndStereo()
	{
		if (Shape != _LastShape || Stereo != _LastStereo || DisplayMono != _LastDisplayMono)
		{
			Rect rect = new Rect(0f, 0f, 1f, 1f);
			switch (Shape)
			{
			case VideoShape._360:
				overlay.currentOverlayShape = OVROverlay.OverlayShape.Equirect;
				break;
			case VideoShape._180:
				overlay.currentOverlayShape = OVROverlay.OverlayShape.Equirect;
				rect = new Rect(0.25f, 0f, 0.5f, 1f);
				break;
			default:
				overlay.currentOverlayShape = OVROverlay.OverlayShape.Quad;
				break;
			}
			overlay.overrideTextureRectMatrix = true;
			Rect rect2 = new Rect(0f, 0f, 1f, 1f);
			Rect rect3 = new Rect(0f, 0f, 1f, 1f);
			switch (Stereo)
			{
			case VideoStereo.LeftRight:
				rect2 = new Rect(0f, 0f, 0.5f, 1f);
				rect3 = new Rect(0.5f, 0f, 0.5f, 1f);
				break;
			case VideoStereo.TopBottom:
				rect2 = new Rect(0f, 0.5f, 1f, 0.5f);
				rect3 = new Rect(0f, 0f, 1f, 0.5f);
				break;
			case VideoStereo.BottomTop:
				rect2 = new Rect(0f, 0f, 1f, 0.5f);
				rect3 = new Rect(0f, 0.5f, 1f, 0.5f);
				break;
			}
			overlay.invertTextureRects = false;
			overlay.SetSrcDestRects(rect2, DisplayMono ? rect2 : rect3, rect, rect);
			_LastDisplayMono = DisplayMono;
			_LastStereo = Stereo;
			_LastShape = Shape;
		}
	}

	private IEnumerator Start()
	{
		if (mediaRenderer.material == null)
		{
			Debug.LogError("No material for movie surface");
			yield break;
		}
		yield return new WaitForSeconds(1f);
		if (!string.IsNullOrEmpty(MovieName))
		{
			if (IsLocalVideo(MovieName))
			{
				Play(Application.streamingAssetsPath + "/" + MovieName, null);
			}
			else
			{
				Play(MovieName, DrmLicenseUrl);
			}
		}
	}

	public void Play(string moviePath, string drmLicencesUrl)
	{
		if (moviePath != string.Empty)
		{
			Debug.Log("Playing Video: " + moviePath);
			if (overlay.isExternalSurface)
			{
				OVROverlay.ExternalSurfaceObjectCreated externalSurfaceObjectCreated = delegate
				{
					Debug.Log("Playing ExoPlayer with SurfaceObject");
					NativeVideoPlayer.PlayVideo(moviePath, drmLicencesUrl, overlay.externalSurfaceObject);
					NativeVideoPlayer.SetLooping(LoopVideo);
				};
				if (overlay.externalSurfaceObject == IntPtr.Zero)
				{
					overlay.externalSurfaceObjectCreated = externalSurfaceObjectCreated;
				}
				else
				{
					externalSurfaceObjectCreated();
				}
			}
			else
			{
				Debug.Log("Playing Unity VideoPlayer");
				videoPlayer.url = moviePath;
				videoPlayer.Prepare();
				videoPlayer.Play();
			}
			Debug.Log("MovieSample Start");
			IsPlaying = true;
		}
		else
		{
			Debug.LogError("No media file name provided");
		}
	}

	public void Play()
	{
		if (overlay.isExternalSurface)
		{
			NativeVideoPlayer.Play();
		}
		else
		{
			videoPlayer.Play();
		}
		IsPlaying = true;
	}

	public void Pause()
	{
		if (overlay.isExternalSurface)
		{
			NativeVideoPlayer.Pause();
		}
		else
		{
			videoPlayer.Pause();
		}
		IsPlaying = false;
	}

	public void SeekTo(long position)
	{
		long num = Math.Max(0L, Math.Min(Duration, position));
		if (overlay.isExternalSurface)
		{
			NativeVideoPlayer.PlaybackPosition = num;
		}
		else
		{
			videoPlayer.time = (double)num / 1000.0;
		}
	}

	private void Update()
	{
		UpdateShapeAndStereo();
		if (!overlay.isExternalSurface)
		{
			Texture texture = ((videoPlayer.texture != null) ? videoPlayer.texture : Texture2D.blackTexture);
			if (overlay.enabled)
			{
				if (overlay.textures[0] != texture)
				{
					overlay.enabled = false;
					overlay.textures[0] = texture;
					overlay.enabled = true;
				}
			}
			else
			{
				mediaRenderer.material.mainTexture = texture;
				mediaRenderer.material.SetVector("_SrcRectLeft", overlay.srcRectLeft.ToVector());
				mediaRenderer.material.SetVector("_SrcRectRight", overlay.srcRectRight.ToVector());
			}
			IsPlaying = videoPlayer.isPlaying;
			PlaybackPosition = (long)(videoPlayer.time * 1000.0);
			Duration = (long)(videoPlayer.length * 1000.0);
		}
		else
		{
			NativeVideoPlayer.SetListenerRotation(Camera.main.transform.rotation);
			IsPlaying = NativeVideoPlayer.IsPlaying;
			PlaybackPosition = NativeVideoPlayer.PlaybackPosition;
			Duration = NativeVideoPlayer.Duration;
			if (IsPlaying && (int)OVRManager.display.displayFrequency != 60)
			{
				OVRManager.display.displayFrequency = 60f;
			}
			else if (!IsPlaying && (int)OVRManager.display.displayFrequency != 72)
			{
				OVRManager.display.displayFrequency = 72f;
			}
		}
	}

	public void SetPlaybackSpeed(float speed)
	{
		speed = Mathf.Max(0f, speed);
		if (overlay.isExternalSurface)
		{
			NativeVideoPlayer.SetPlaybackSpeed(speed);
		}
		else
		{
			videoPlayer.playbackSpeed = speed;
		}
	}

	public void Stop()
	{
		if (overlay.isExternalSurface)
		{
			NativeVideoPlayer.Stop();
		}
		else
		{
			videoPlayer.Stop();
		}
		IsPlaying = false;
	}

	private void OnApplicationPause(bool appWasPaused)
	{
		Debug.Log("OnApplicationPause: " + appWasPaused);
		if (appWasPaused)
		{
			videoPausedBeforeAppPause = !IsPlaying;
		}
		if (!videoPausedBeforeAppPause)
		{
			if (appWasPaused)
			{
				Pause();
			}
			else
			{
				Play();
			}
		}
	}
}
