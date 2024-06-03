using System;
using System.Runtime.CompilerServices;
using System.Threading;
using GorillaTag.Reactions;
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
				PaperPlaneProjectile.PaperPlaneHit value2 = (PaperPlaneProjectile.PaperPlaneHit)Delegate.Combine(paperPlaneHit2, value);
				paperPlaneHit = Interlocked.CompareExchange<PaperPlaneProjectile.PaperPlaneHit>(ref this.OnHit, value2, paperPlaneHit2);
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
				PaperPlaneProjectile.PaperPlaneHit value2 = (PaperPlaneProjectile.PaperPlaneHit)Delegate.Remove(paperPlaneHit2, value);
				paperPlaneHit = Interlocked.CompareExchange<PaperPlaneProjectile.PaperPlaneHit>(ref this.OnHit, value2, paperPlaneHit2);
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

	public VRRig MyRig
	{
		get
		{
			return this.myRig;
		}
	}

	private void Awake()
	{
		this._tCached = base.transform;
		this.spawnWorldEffects = base.GetComponent<SpawnWorldEffects>();
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

	public void Launch(Vector3 startPos, Quaternion startRot, Vector3 vel)
	{
		base.gameObject.SetActive(true);
		this.ResetProjectile();
		this.transform.position = startPos;
		if (this.enableRotation)
		{
			this.transform.rotation = startRot;
		}
		else
		{
			this.transform.LookAt(this.transform.position + vel.normalized);
		}
		this._direction = vel.normalized;
		this._speed = Mathf.Clamp(vel.sqrMagnitude / 2f, this.minSpeed, this.maxSpeed);
		this._stopped = false;
		this.scaleFactor = 0.7f * (this.transform.lossyScale.x - 1f + 1.4285715f);
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
		this.nextPos = this.transform.position + this._direction * this._speed * Time.deltaTime * this.scaleFactor;
		if (this._timeElapsed < this.maxFlightTime && (this._timeElapsed < this.minFlightTime || Physics.RaycastNonAlloc(this.transform.position, this.nextPos - this.transform.position, this.results, Vector3.Distance(this.transform.position, this.nextPos), this.layerMask.value) == 0))
		{
			this.transform.position = this.nextPos;
			this.transform.Rotate(Mathf.Sin(this._timeElapsed) * 10f * Time.deltaTime, 0f, 0f);
			return;
		}
		if (this._timeElapsed < this.maxFlightTime)
		{
			SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
			if (this.results[0].collider.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
			{
				slingshotProjectileHitNotifier.InvokeHit(this, this.results[0].collider);
			}
			if (this.spawnWorldEffects != null)
			{
				this.spawnWorldEffects.RequestSpawn(this.nextPos);
			}
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

	internal void SetVRRig(VRRig rig)
	{
		this.myRig = rig;
	}

	public PaperPlaneProjectile()
	{
	}

	private const float speedScaleRatio = 0.7f;

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
	private Vector3 _direction;

	[NonSerialized]
	private bool _stopped;

	private Transform _tCached;

	private SpawnWorldEffects spawnWorldEffects;

	private Vector3 nextPos;

	private RaycastHit[] results = new RaycastHit[1];

	[SerializeField]
	private float maxFlightTime = 7.5f;

	[SerializeField]
	private float minFlightTime = 0.5f;

	[SerializeField]
	private float maxSpeed = 10f;

	[SerializeField]
	private float minSpeed = 1f;

	[SerializeField]
	private bool enableRotation;

	[SerializeField]
	private GameObject flyingObject;

	[SerializeField]
	private GameObject crashingObject;

	[SerializeField]
	private LayerMask layerMask;

	private VRRig myRig;

	private float scaleFactor;

	public delegate void PaperPlaneHit(Vector3 endPoint);
}
