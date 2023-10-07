using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000234 RID: 564
public class TimeOfDayEvent : MonoBehaviour
{
	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x06000DEF RID: 3567 RVA: 0x00050F01 File Offset: 0x0004F101
	public float currentTime
	{
		get
		{
			return this._currentTime;
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x06000DF0 RID: 3568 RVA: 0x00050F09 File Offset: 0x0004F109
	// (set) Token: 0x06000DF1 RID: 3569 RVA: 0x00050F11 File Offset: 0x0004F111
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

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x06000DF2 RID: 3570 RVA: 0x00050F1F File Offset: 0x0004F11F
	// (set) Token: 0x06000DF3 RID: 3571 RVA: 0x00050F27 File Offset: 0x0004F127
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

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x06000DF4 RID: 3572 RVA: 0x00050F35 File Offset: 0x0004F135
	public bool isOngoing
	{
		get
		{
			return this._ongoing;
		}
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x00050F40 File Offset: 0x0004F140
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

	// Token: 0x06000DF6 RID: 3574 RVA: 0x00050FC2 File Offset: 0x0004F1C2
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

	// Token: 0x06000DF7 RID: 3575 RVA: 0x00050FF8 File Offset: 0x0004F1F8
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

	// Token: 0x06000DF8 RID: 3576 RVA: 0x0005108F File Offset: 0x0004F28F
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

	// Token: 0x06000DF9 RID: 3577 RVA: 0x000510A8 File Offset: 0x0004F2A8
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

	// Token: 0x06000DFA RID: 3578 RVA: 0x000510C1 File Offset: 0x0004F2C1
	public static implicit operator bool(TimeOfDayEvent ev)
	{
		return ev && ev.isOngoing;
	}

	// Token: 0x040010E5 RID: 4325
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeStart;

	// Token: 0x040010E6 RID: 4326
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeEnd = 1f;

	// Token: 0x040010E7 RID: 4327
	[SerializeField]
	private float _currentTime = -1f;

	// Token: 0x040010E8 RID: 4328
	[SerializeField]
	private bool _ongoing;

	// Token: 0x040010E9 RID: 4329
	[Space]
	public UnityEvent onEventStart;

	// Token: 0x040010EA RID: 4330
	public UnityEvent onEventStop;

	// Token: 0x040010EB RID: 4331
	[Space]
	[SerializeField]
	private double _currentSeconds = -1.0;

	// Token: 0x040010EC RID: 4332
	[SerializeField]
	private double _totalSecondsInRange = -1.0;

	// Token: 0x040010ED RID: 4333
	[NonSerialized]
	private float _elapsed = -1f;

	// Token: 0x040010EE RID: 4334
	[SerializeField]
	private BetterDayNightManager _dayNightManager;
}
