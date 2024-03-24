using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;

public class PaperPlaneProjectile : MonoBehaviour
{
	public event PaperPlaneProjectile.PaperPlaneHit OnHit
	{
		[CompilerGenerated]
		add
		{
			PaperPlaneProjectile.PaperPlaneHit paperPlaneHit = this.OnHit;
			PaperPlaneProjectile.PaperPlaneHit paperPlaneHit2;
			do
			{
				paperPlaneHit2 = paperPlaneHit;
				PaperPlaneProjectile.PaperPlaneHit paperPlaneHit3 = (PaperPlaneProjectile.PaperPlaneHit)Delegate.Combine(paperPlaneHit2, value);
				paperPlaneHit = Interlocked.CompareExchange<PaperPlaneProjectile.PaperPlaneHit>(ref this.OnHit, paperPlaneHit3, paperPlaneHit2);
			}
			while (paperPlaneHit != paperPlaneHit2);
		}
		[CompilerGenerated]
		remove
		{
			PaperPlaneProjectile.PaperPlaneHit paperPlaneHit = this.OnHit;
			PaperPlaneProjectile.PaperPlaneHit paperPlaneHit2;
			do
			{
				paperPlaneHit2 = paperPlaneHit;
				PaperPlaneProjectile.PaperPlaneHit paperPlaneHit3 = (PaperPlaneProjectile.PaperPlaneHit)Delegate.Remove(paperPlaneHit2, value);
				paperPlaneHit = Interlocked.CompareExchange<PaperPlaneProjectile.PaperPlaneHit>(ref this.OnHit, paperPlaneHit3, paperPlaneHit2);
			}
			while (paperPlaneHit != paperPlaneHit2);
		}
	}

	public new Transform transform
	{
		get
		{
			return this._tCached;
		}
	}

	private void Awake()
	{
		this._tCached = base.transform;
	}

	private void Start()
	{
		this.ResetProjectile();
	}

	public void ResetProjectile()
	{
		this._timeElapsed = 0f;
		this.flyingObject.SetActive(true);
		this.crashingObject.SetActive(false);
	}

	public void Launch(Vector3 startPos, Vector3 vel)
	{
		base.gameObject.SetActive(true);
		this.ResetProjectile();
		this.transform.position = startPos;
		this.transform.LookAt(this.transform.position + vel.normalized);
		this._speed = Mathf.Clamp(vel.sqrMagnitude / 2f, 1f, 10f);
		this._stopped = false;
	}

	private void Update()
	{
		if (this._stopped)
		{
			if (!this.crashingObject.gameObject.activeSelf)
			{
				if (ObjectPools.instance)
				{
					ObjectPools.instance.Destroy(base.gameObject);
					return;
				}
				base.gameObject.SetActive(false);
			}
			return;
		}
		this._timeElapsed += Time.deltaTime;
		this.nextPos = this.transform.position + this.transform.forward * this._speed * Time.deltaTime;
		if (this._timeElapsed < this.maxFlightTime && (this._timeElapsed < this.minFlightTime || Physics.RaycastNonAlloc(this.transform.position, this.nextPos - this.transform.position, this.results, Vector3.Distance(this.transform.position, this.nextPos)) == 0))
		{
			this.transform.position = this.nextPos;
			this.transform.Rotate(Mathf.Sin(this._timeElapsed) * 10f * Time.deltaTime, 0f, 0f);
			return;
		}
		this._stopped = true;
		this._timeElapsed = 0f;
		PaperPlaneProjectile.PaperPlaneHit onHit = this.OnHit;
		if (onHit != null)
		{
			onHit(this.nextPos);
		}
		this.OnHit = null;
		this.flyingObject.SetActive(false);
		this.crashingObject.SetActive(true);
	}

	public PaperPlaneProjectile()
	{
	}

	public Vector3 startPos;

	public Vector3 endPos;

	[FormerlySerializedAs("_flyTimeOut")]
	[Range(1f, 128f)]
	public float flyTimeOut = 32f;

	[CompilerGenerated]
	private PaperPlaneProjectile.PaperPlaneHit OnHit;

	[Space]
	public float curveTime = 0.4f;

	[Space]
	public Vector3 curveDirection;

	public float curveDistance = 9.8f;

	[Space]
	[NonSerialized]
	private float _timeElapsed;

	[NonSerialized]
	private float _speed;

	[NonSerialized]
	private bool _stopped;

	private Transform _tCached;

	private Vector3 nextPos;

	private RaycastHit[] results = new RaycastHit[1];

	[SerializeField]
	private float maxFlightTime = 7.5f;

	[SerializeField]
	private float minFlightTime = 0.5f;

	[SerializeField]
	private GameObject flyingObject;

	[SerializeField]
	private GameObject crashingObject;

	public delegate void PaperPlaneHit(Vector3 endPoint);
}
