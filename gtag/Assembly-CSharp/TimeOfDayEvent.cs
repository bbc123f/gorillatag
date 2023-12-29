using System;
using UnityEngine;
using UnityEngine.Events;

public class TimeOfDayEvent : MonoBehaviour
{
	public float currentTime
	{
		get
		{
			return this._currentTime;
		}
	}

	public float timeStart
	{
		get
		{
			return this._timeStart;
		}
		set
		{
			this._timeStart = Mathf.Clamp01(value);
		}
	}

	public float timeEnd
	{
		get
		{
			return this._timeEnd;
		}
		set
		{
			this._timeEnd = Mathf.Clamp01(value);
		}
	}

	public bool isOngoing
	{
		get
		{
			return this._ongoing;
		}
	}

	private void Start()
	{
		if (!this._dayNightManager)
		{
			this._dayNightManager = BetterDayNightManager.instance;
		}
		if (!this._dayNightManager)
		{
			return;
		}
		for (int i = 0; i < this._dayNightManager.timeOfDayRange.Length; i++)
		{
			this._totalSecondsInRange += this._dayNightManager.timeOfDayRange[i] * 3600.0;
		}
		this._totalSecondsInRange = Math.Floor(this._totalSecondsInRange);
	}

	private void Update()
	{
		this._elapsed += Time.deltaTime;
		if (this._elapsed < 1f)
		{
			return;
		}
		this._elapsed = 0f;
		this.UpdateTime();
	}

	private void UpdateTime()
	{
		this._currentSeconds = ((ITimeOfDaySystem)this._dayNightManager).currentTimeInSeconds;
		this._currentSeconds = Math.Floor(this._currentSeconds);
		this._currentTime = (float)(this._currentSeconds / this._totalSecondsInRange);
		bool flag = this._currentTime >= 0f && this._currentTime >= this._timeStart && this._currentTime <= this._timeEnd;
		if (!this._ongoing && flag)
		{
			this.StartEvent();
		}
		if (this._ongoing && !flag)
		{
			this.StopEvent();
		}
	}

	private void StartEvent()
	{
		this._ongoing = true;
		UnityEvent unityEvent = this.onEventStart;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	private void StopEvent()
	{
		this._ongoing = false;
		UnityEvent unityEvent = this.onEventStop;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	public static implicit operator bool(TimeOfDayEvent ev)
	{
		return ev && ev.isOngoing;
	}

	[SerializeField]
	[Range(0f, 1f)]
	private float _timeStart;

	[SerializeField]
	[Range(0f, 1f)]
	private float _timeEnd = 1f;

	[SerializeField]
	private float _currentTime = -1f;

	[SerializeField]
	private bool _ongoing;

	[Space]
	public UnityEvent onEventStart;

	public UnityEvent onEventStop;

	[Space]
	[SerializeField]
	private double _currentSeconds = -1.0;

	[SerializeField]
	private double _totalSecondsInRange = -1.0;

	[NonSerialized]
	private float _elapsed = -1f;

	[SerializeField]
	private BetterDayNightManager _dayNightManager;
}
