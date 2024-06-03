using System;
using GorillaExtensions;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

public class RubberDuck : TransferrableObject
{
	public bool fxActive
	{
		get
		{
			return this.hasParticleFX && this._fxActive;
		}
		set
		{
			if (!this.hasParticleFX)
			{
				return;
			}
			this.pFXEmissionModule.enabled = value;
			this._fxActive = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (this.skinRenderer == null)
		{
			this.skinRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>(true);
		}
		this.hasSkinRenderer = (this.skinRenderer != null);
		this.myThreshold = 0.7f;
		this.hysterisis = 0.3f;
		this.hasParticleFX = (this.particleFX != null);
		if (this.hasParticleFX)
		{
			this.pFXEmissionModule = this.particleFX.emission;
			this.pFXEmissionModule.rateOverTime = this.particleFXEmissionIdle;
		}
		this.fxActive = false;
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			Player player = (this.myOnlineRig != null) ? this.myOnlineRig.creator : ((this.myRig != null) ? ((this.myRig.creator != null) ? this.myRig.creator : PhotonNetwork.LocalPlayer) : null);
			if (player != null)
			{
				this._events.Init(player);
			}
			else
			{
				Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
			}
		}
		if (this._events != null)
		{
			this._events.Activate += this.OnSqueezeActivate;
			this._events.Deactivate += this.OnSqueezeDeactivate;
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
		if (this._events != null)
		{
			this._events.Activate -= this.OnSqueezeActivate;
			this._events.Deactivate -= this.OnSqueezeDeactivate;
			Object.Destroy(this._events);
			this._events = null;
		}
	}

	private void OnSqueezeActivate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "OnSqueezeActivate");
		if (sender != target)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creatorWrapped.ID)
		{
			return;
		}
		this.SqueezeActivateLocal();
	}

	private void SqueezeActivateLocal()
	{
		this.PlayParticleFX(this.particleFXEmissionSqueeze);
		if (this._sfxActivate && !this._sfxActivate.isPlaying)
		{
			this._sfxActivate.PlayNext(0f, 1f);
		}
	}

	private void OnSqueezeDeactivate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "OnSqueezeDeactivate");
		if (sender != target)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creatorWrapped.ID)
		{
			return;
		}
		this.SqueezeDeactivateLocal();
	}

	private void SqueezeDeactivateLocal()
	{
		this.PlayParticleFX(this.particleFXEmissionIdle);
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
		float num = 0f;
		if (base.InHand())
		{
			this.tempHandPos = ((this.myOnlineRig != null) ? this.myOnlineRig.ReturnHandPosition() : this.myRig.ReturnHandPosition());
			if (this.currentState == TransferrableObject.PositionState.InLeftHand)
			{
				num = (float)Mathf.FloorToInt((float)(this.tempHandPos % 10000) / 1000f);
			}
			else
			{
				num = (float)Mathf.FloorToInt((float)(this.tempHandPos % 10) / 1f);
			}
		}
		if (this.hasSkinRenderer)
		{
			this.skinRenderer.SetBlendShapeWeight(0, Mathf.Lerp(this.skinRenderer.GetBlendShapeWeight(0), num * 11.1f, this.blendShapeMaxWeight));
		}
		if (this.fxActive)
		{
			this.squeezeTimeElapsed += Time.deltaTime;
			this.pFXEmissionModule.rateOverTime = Mathf.Lerp(this.particleFXEmissionIdle, this.particleFXEmissionSqueeze, this.particleFXEmissionCooldownCurve.Evaluate(this.squeezeTimeElapsed));
			if (this.squeezeTimeElapsed > this.particleFXEmissionSqueeze)
			{
				this.fxActive = false;
			}
		}
	}

	public override void OnActivate()
	{
		base.OnActivate();
		if (this.IsMyItem())
		{
			bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
			RigContainer localRig = VRRigCache.Instance.localRig;
			localRig.Rig.PlayHandTapLocal(this.squeezeSound, flag, 0.33f);
			if (localRig.photonView)
			{
				localRig.photonView.RPC("PlayHandTap", RpcTarget.Others, new object[]
				{
					this.squeezeSound,
					flag,
					0.33f
				});
			}
			GorillaTagger.Instance.StartVibration(flag, this.squeezeStrength, Time.deltaTime);
		}
		if (this._raiseActivate)
		{
			if (RoomSystem.JoinedRoom)
			{
				RubberDuckEvents events = this._events;
				if (events == null)
				{
					return;
				}
				PhotonEvent activate = events.Activate;
				if (activate == null)
				{
					return;
				}
				activate.RaiseAll(Array.Empty<object>());
				return;
			}
			else
			{
				this.SqueezeActivateLocal();
			}
		}
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		if (this.IsMyItem())
		{
			bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
			if (GorillaGameManager.instance)
			{
				GorillaGameManager.instance.FindVRRigForPlayer(PhotonNetwork.LocalPlayer).RPC("PlayHandTap", RpcTarget.All, new object[]
				{
					this.squeezeReleaseSound,
					flag,
					0.33f
				});
			}
			GorillaTagger.Instance.StartVibration(flag, this.releaseStrength, Time.deltaTime);
		}
		if (this._raiseDeactivate)
		{
			if (RoomSystem.JoinedRoom)
			{
				RubberDuckEvents events = this._events;
				if (events == null)
				{
					return;
				}
				PhotonEvent deactivate = events.Deactivate;
				if (deactivate == null)
				{
					return;
				}
				deactivate.RaiseAll(Array.Empty<object>());
				return;
			}
			else
			{
				this.SqueezeDeactivateLocal();
			}
		}
	}

	public void PlayParticleFX(float rate)
	{
		if (!this.hasParticleFX)
		{
			return;
		}
		if (this.currentState != TransferrableObject.PositionState.InLeftHand && this.currentState != TransferrableObject.PositionState.InRightHand)
		{
			return;
		}
		if (!this.fxActive)
		{
			this.fxActive = true;
		}
		this.squeezeTimeElapsed = 0f;
		this.pFXEmissionModule.rateOverTime = rate;
	}

	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	public RubberDuck()
	{
	}

	[DebugOption]
	public bool disableActivation;

	[DebugOption]
	public bool disableDeactivation;

	private SkinnedMeshRenderer skinRenderer;

	[FormerlySerializedAs("duckieLerp")]
	public float blendShapeMaxWeight = 1f;

	private int tempHandPos;

	[GorillaSoundLookup]
	public int squeezeSound = 75;

	[GorillaSoundLookup]
	public int squeezeReleaseSound = 76;

	public float squeezeStrength = 0.05f;

	public float releaseStrength = 0.03f;

	public ParticleSystem particleFX;

	[Tooltip("The emission rate of the particle effect when not squeezed.")]
	public float particleFXEmissionIdle = 0.8f;

	[Tooltip("The emission rate of the particle effect when squeezed.")]
	public float particleFXEmissionSqueeze = 10f;

	[Tooltip("The animation of the particle effect returning to the idle emission rate. X axis is time, Y axis is the emission lerp value where 0 is idle, 1 is squeezed.")]
	public AnimationCurve particleFXEmissionCooldownCurve;

	private bool hasSkinRenderer;

	private ParticleSystem.EmissionModule pFXEmissionModule;

	private bool hasParticleFX;

	private float squeezeTimeElapsed;

	[SerializeField]
	private RubberDuckEvents _events;

	[SerializeField]
	private bool _raiseActivate = true;

	[SerializeField]
	private bool _raiseDeactivate = true;

	[SerializeField]
	private SoundEffects _sfxActivate;

	[SerializeField]
	private bool _fxActive;
}
