using System;
using GorillaExtensions;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000130 RID: 304
public class RubberDuck : TransferrableObject
{
	// Token: 0x17000058 RID: 88
	// (get) Token: 0x060007ED RID: 2029 RVA: 0x00031FC7 File Offset: 0x000301C7
	// (set) Token: 0x060007EE RID: 2030 RVA: 0x00031FD9 File Offset: 0x000301D9
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

	// Token: 0x060007EF RID: 2031 RVA: 0x00031FF8 File Offset: 0x000301F8
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

	// Token: 0x060007F0 RID: 2032 RVA: 0x00032098 File Offset: 0x00030298
	public override void OnEnable()
	{
		base.OnEnable();
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			RubberDuckEvents events = this._events;
			VRRig myOnlineRig = this.myOnlineRig;
			Player player;
			if ((player = ((myOnlineRig != null) ? myOnlineRig.creator : null)) == null)
			{
				VRRig myRig = this.myRig;
				player = ((myRig != null) ? myRig.creator : null);
			}
			events.Init(player);
		}
		if (this._events != null)
		{
			this._events.Activate += this.OnSqueezeActivate;
			this._events.Deactivate += this.OnSqueezeDeactivate;
		}
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x0003214F File Offset: 0x0003034F
	public override void OnDisable()
	{
		base.OnDisable();
		if (this._events != null)
		{
			Object.Destroy(this._events);
		}
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x00032170 File Offset: 0x00030370
	private void OnSqueezeActivate(int sender, int target, object[] args)
	{
		if (sender != target)
		{
			return;
		}
		float rate = this.particleFXEmissionSqueeze;
		this.PlayParticleFX(rate);
		if (this._sfxActivate && !this._sfxActivate.isPlaying)
		{
			this._sfxActivate.PlayNext(0f, 1f);
		}
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x000321C0 File Offset: 0x000303C0
	private void OnSqueezeDeactivate(int sender, int target, object[] args)
	{
		if (sender != target)
		{
			return;
		}
		float rate = this.particleFXEmissionIdle;
		this.PlayParticleFX(rate);
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x000321E0 File Offset: 0x000303E0
	protected override void LateUpdate()
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

	// Token: 0x060007F5 RID: 2037 RVA: 0x000322FC File Offset: 0x000304FC
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
			activate.RaiseAll(new object[]
			{
				this.particleFXEmissionSqueeze
			});
		}
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x000323DC File Offset: 0x000305DC
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
			deactivate.RaiseAll(new object[]
			{
				this.particleFXEmissionIdle
			});
		}
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x000324A4 File Offset: 0x000306A4
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

	// Token: 0x060007F8 RID: 2040 RVA: 0x000324F8 File Offset: 0x000306F8
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x00032503 File Offset: 0x00030703
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x0400098D RID: 2445
	[DebugOption]
	public bool disableActivation;

	// Token: 0x0400098E RID: 2446
	[DebugOption]
	public bool disableDeactivation;

	// Token: 0x0400098F RID: 2447
	private SkinnedMeshRenderer skinRenderer;

	// Token: 0x04000990 RID: 2448
	[FormerlySerializedAs("duckieLerp")]
	public float blendShapeMaxWeight = 1f;

	// Token: 0x04000991 RID: 2449
	private int tempHandPos;

	// Token: 0x04000992 RID: 2450
	[GorillaSoundLookup]
	public int squeezeSound = 75;

	// Token: 0x04000993 RID: 2451
	[GorillaSoundLookup]
	public int squeezeReleaseSound = 76;

	// Token: 0x04000994 RID: 2452
	public float squeezeStrength = 0.05f;

	// Token: 0x04000995 RID: 2453
	public float releaseStrength = 0.03f;

	// Token: 0x04000996 RID: 2454
	public ParticleSystem particleFX;

	// Token: 0x04000997 RID: 2455
	[Tooltip("The emission rate of the particle effect when not squeezed.")]
	public float particleFXEmissionIdle = 0.8f;

	// Token: 0x04000998 RID: 2456
	[Tooltip("The emission rate of the particle effect when squeezed.")]
	public float particleFXEmissionSqueeze = 10f;

	// Token: 0x04000999 RID: 2457
	[Tooltip("The animation of the particle effect returning to the idle emission rate. X axis is time, Y axis is the emission lerp value where 0 is idle, 1 is squeezed.")]
	public AnimationCurve particleFXEmissionCooldownCurve;

	// Token: 0x0400099A RID: 2458
	private bool hasSkinRenderer;

	// Token: 0x0400099B RID: 2459
	private ParticleSystem.EmissionModule pFXEmissionModule;

	// Token: 0x0400099C RID: 2460
	private bool hasParticleFX;

	// Token: 0x0400099D RID: 2461
	private float squeezeTimeElapsed;

	// Token: 0x0400099E RID: 2462
	[SerializeField]
	private RubberDuckEvents _events;

	// Token: 0x0400099F RID: 2463
	[SerializeField]
	private bool _raiseActivate = true;

	// Token: 0x040009A0 RID: 2464
	[SerializeField]
	private bool _raiseDeactivate = true;

	// Token: 0x040009A1 RID: 2465
	[SerializeField]
	private SoundEffects _sfxActivate;

	// Token: 0x040009A2 RID: 2466
	[SerializeField]
	private bool _fxActive;
}
