using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000235 RID: 565
public class TimeOfDayEvent : MonoBehaviour
{
	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x06000DF6 RID: 3574 RVA: 0x000512DD File Offset: 0x0004F4DD
	public float currentTime
	{
		get
		{
			return this._currentTime;
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x06000DF7 RID: 3575 RVA: 0x000512E5 File Offset: 0x0004F4E5
	// (set) Token: 0x06000DF8 RID: 3576 RVA: 0x000512ED File Offset: 0x0004F4ED
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

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x06000DF9 RID: 3577 RVA: 0x000512FB File Offset: 0x0004F4FB
	// (set) Token: 0x06000DFA RID: 3578 RVA: 0x00051303 File Offset: 0x0004F503
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

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x06000DFB RID: 3579 RVA: 0x00051311 File Offset: 0x0004F511
	public bool isOngoing
	{
		get
		{
			return this._ongoing;
		}
	}

	// Token: 0x06000DFC RID: 3580 RVA: 0x0005131C File Offset: 0x0004F51C
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

	// Token: 0x06000DFD RID: 3581 RVA: 0x0005139E File Offset: 0x0004F59E
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

	// Token: 0x06000DFE RID: 3582 RVA: 0x000513D4 File Offset: 0x0004F5D4
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

	// Token: 0x06000DFF RID: 3583 RVA: 0x0005146B File Offset: 0x0004F66B
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

	// Token: 0x06000E00 RID: 3584 RVA: 0x00051484 File Offset: 0x0004F684
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

	// Token: 0x06000E01 RID: 3585 RVA: 0x0005149D File Offset: 0x0004F69D
	public static implicit operator bool(TimeOfDayEvent ev)
	{
		return ev && ev.isOngoing;
	}

	// Token: 0x040010EB RID: 4331
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeStart;

	// Token: 0x040010EC RID: 4332
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeEnd = 1f;

	// Token: 0x040010ED RID: 4333
	[SerializeField]
	private float _currentTime = -1f;

	// Token: 0x040010EE RID: 4334
	[SerializeField]
	private bool _ongoing;

	// Token: 0x040010EF RID: 4335
	[Space]
	public UnityEvent onEventStart;

	// Token: 0x040010F0 RID: 4336
	public UnityEvent onEventStop;

	// Token: 0x040010F1 RID: 4337
	[Space]
	[SerializeField]
	private double _currentSeconds = -1.0;

	// Token: 0x040010F2 RID: 4338
	[SerializeField]
	private double _totalSecondsInRange = -1.0;

	// Token: 0x040010F3 RID: 4339
	[NonSerialized]
	private float _elapsed = -1f;

	// Token: 0x040010F4 RID: 4340
	[SerializeField]
	private BetterDayNightManager _dayNightManager;
}
