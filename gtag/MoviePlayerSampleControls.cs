using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoviePlayerSampleControls : MonoBehaviour
{
	private enum PlaybackState
	{
		Playing,
		Paused,
		Rewinding,
		FastForwarding
	}

	public MoviePlayerSample Player;

	public OVRInputModule InputModule;

	public OVRGazePointer GazePointer;

	public GameObject LeftHand;

	public GameObject RightHand;

	public Canvas Canvas;

	public ButtonDownListener PlayPause;

	public MediaPlayerImage PlayPauseImage;

	public Slider ProgressBar;

	public ButtonDownListener FastForward;

	public MediaPlayerImage FastForwardImage;

	public ButtonDownListener Rewind;

	public MediaPlayerImage RewindImage;

	public float TimeoutTime = 10f;

	private bool _isVisible;

	private float _lastButtonTime;

	private bool _didSeek;

	private long _seekPreviousPosition;

	private long _rewindStartPosition;

	private float _rewindStartTime;

	private PlaybackState _state;

	private void Start()
	{
		PlayPause.onButtonDown += OnPlayPauseClicked;
		FastForward.onButtonDown += OnFastForwardClicked;
		Rewind.onButtonDown += OnRewindClicked;
		ProgressBar.onValueChanged.AddListener(OnSeekBarMoved);
		PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
		FastForwardImage.buttonType = MediaPlayerImage.ButtonType.SkipForward;
		RewindImage.buttonType = MediaPlayerImage.ButtonType.SkipBack;
		SetVisible(visible: false);
	}

	private void OnPlayPauseClicked()
	{
		switch (_state)
		{
		case PlaybackState.Paused:
			Player.Play();
			PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
			FastForwardImage.buttonType = MediaPlayerImage.ButtonType.FastForward;
			RewindImage.buttonType = MediaPlayerImage.ButtonType.Rewind;
			_state = PlaybackState.Playing;
			break;
		case PlaybackState.Playing:
			Player.Pause();
			PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Play;
			FastForwardImage.buttonType = MediaPlayerImage.ButtonType.SkipForward;
			RewindImage.buttonType = MediaPlayerImage.ButtonType.SkipBack;
			_state = PlaybackState.Paused;
			break;
		case PlaybackState.FastForwarding:
			Player.SetPlaybackSpeed(1f);
			PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
			_state = PlaybackState.Playing;
			break;
		case PlaybackState.Rewinding:
			Player.Play();
			_state = PlaybackState.Playing;
			PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
			break;
		}
	}

	private void OnFastForwardClicked()
	{
		switch (_state)
		{
		case PlaybackState.FastForwarding:
			Player.SetPlaybackSpeed(1f);
			_state = PlaybackState.Playing;
			PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
			break;
		case PlaybackState.Rewinding:
			Player.Play();
			Player.SetPlaybackSpeed(2f);
			_state = PlaybackState.FastForwarding;
			break;
		case PlaybackState.Playing:
			Player.SetPlaybackSpeed(2f);
			PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Play;
			_state = PlaybackState.FastForwarding;
			break;
		case PlaybackState.Paused:
			Seek(Player.PlaybackPosition + 15000);
			break;
		}
	}

	private void OnRewindClicked()
	{
		switch (_state)
		{
		case PlaybackState.Playing:
		case PlaybackState.FastForwarding:
			Player.SetPlaybackSpeed(1f);
			Player.Pause();
			_rewindStartPosition = Player.PlaybackPosition;
			_rewindStartTime = Time.time;
			PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Play;
			_state = PlaybackState.Rewinding;
			break;
		case PlaybackState.Rewinding:
			Player.Play();
			PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
			_state = PlaybackState.Playing;
			break;
		case PlaybackState.Paused:
			Seek(Player.PlaybackPosition - 15000);
			break;
		}
	}

	private void OnSeekBarMoved(float value)
	{
		long num = (long)(value * (float)Player.Duration);
		if (Mathf.Abs(num - Player.PlaybackPosition) > 200f)
		{
			Seek(num);
		}
	}

	private void Seek(long pos)
	{
		_didSeek = true;
		_seekPreviousPosition = Player.PlaybackPosition;
		Player.SeekTo(pos);
	}

	private void Update()
	{
		if (OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
		{
			_lastButtonTime = Time.time;
			if (!_isVisible)
			{
				SetVisible(visible: true);
			}
		}
		if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
		{
			InputModule.rayTransform = LeftHand.transform;
			GazePointer.rayTransform = LeftHand.transform;
		}
		else
		{
			InputModule.rayTransform = RightHand.transform;
			GazePointer.rayTransform = RightHand.transform;
		}
		if (OVRInput.Get(OVRInput.Button.Back) && _isVisible)
		{
			SetVisible(visible: false);
		}
		if (_state == PlaybackState.Rewinding)
		{
			ProgressBar.value = Mathf.Clamp01(((float)_rewindStartPosition - 1000f * (Time.time - _rewindStartTime)) / (float)Player.Duration);
		}
		if (_isVisible && _state == PlaybackState.Playing && Time.time - _lastButtonTime > TimeoutTime)
		{
			SetVisible(visible: false);
		}
		if (_isVisible && (!_didSeek || Mathf.Abs(_seekPreviousPosition - Player.PlaybackPosition) > 50f))
		{
			_didSeek = false;
			if (Player.Duration > 0)
			{
				ProgressBar.value = (float)((double)Player.PlaybackPosition / (double)Player.Duration);
			}
			else
			{
				ProgressBar.value = 0f;
			}
		}
	}

	private void SetVisible(bool visible)
	{
		Canvas.enabled = visible;
		_isVisible = visible;
		Player.DisplayMono = visible;
		LeftHand.SetActive(visible);
		RightHand.SetActive(visible);
		Debug.Log("Controls Visible: " + visible);
	}
}
