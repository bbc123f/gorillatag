using System;
using GorillaExtensions;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class RubberDuck : TransferrableObject
{
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

	public bool fxActive
	{
		get
		{
			if (hasParticleFX)
			{
				return _fxActive;
			}
			return false;
		}
		set
		{
			if (hasParticleFX)
			{
				pFXEmissionModule.enabled = value;
				_fxActive = value;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (skinRenderer == null)
		{
			skinRenderer = GetComponentInChildren<SkinnedMeshRenderer>(includeInactive: true);
		}
		hasSkinRenderer = skinRenderer != null;
		myThreshold = 0.7f;
		hysterisis = 0.3f;
		hasParticleFX = particleFX != null;
		if (hasParticleFX)
		{
			pFXEmissionModule = particleFX.emission;
			pFXEmissionModule.rateOverTime = particleFXEmissionIdle;
		}
		fxActive = false;
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (_events == null)
		{
			_events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			_events.Init(myOnlineRig?.creator ?? myRig?.creator);
		}
		if (_events != null)
		{
			_events.Activate += new Action<int, int, object[]>(OnSqueezeActivate);
			_events.Deactivate += new Action<int, int, object[]>(OnSqueezeDeactivate);
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
		if (_events != null)
		{
			UnityEngine.Object.Destroy(_events);
		}
	}

	private void OnSqueezeActivate(int sender, int target, object[] args)
	{
		if (sender == target)
		{
			float rate = particleFXEmissionSqueeze;
			PlayParticleFX(rate);
			if ((bool)_sfxActivate && !_sfxActivate.isPlaying)
			{
				_sfxActivate.PlayNext();
			}
		}
	}

	private void OnSqueezeDeactivate(int sender, int target, object[] args)
	{
		if (sender == target)
		{
			float rate = particleFXEmissionIdle;
			PlayParticleFX(rate);
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		float num = 0f;
		if (InHand())
		{
			tempHandPos = ((myOnlineRig != null) ? myOnlineRig.ReturnHandPosition() : myRig.ReturnHandPosition());
			num = ((currentState != PositionState.InLeftHand) ? ((float)Mathf.FloorToInt((float)(tempHandPos % 10) / 1f)) : ((float)Mathf.FloorToInt((float)(tempHandPos % 10000) / 1000f)));
		}
		if (hasSkinRenderer)
		{
			skinRenderer.SetBlendShapeWeight(0, Mathf.Lerp(skinRenderer.GetBlendShapeWeight(0), num * 11.1f, blendShapeMaxWeight));
		}
		if (fxActive)
		{
			squeezeTimeElapsed += Time.deltaTime;
			pFXEmissionModule.rateOverTime = Mathf.Lerp(particleFXEmissionIdle, particleFXEmissionSqueeze, particleFXEmissionCooldownCurve.Evaluate(squeezeTimeElapsed));
			if (squeezeTimeElapsed > particleFXEmissionSqueeze)
			{
				fxActive = false;
			}
		}
	}

	public override void OnActivate()
	{
		base.OnActivate();
		if (IsMyItem())
		{
			bool flag = currentState == PositionState.InLeftHand;
			if ((bool)GorillaGameManager.instance)
			{
				GorillaGameManager.instance.FindVRRigForPlayer(PhotonNetwork.LocalPlayer).RPC("PlayHandTap", RpcTarget.All, squeezeSound, flag, 0.33f);
			}
			GorillaTagger.Instance.StartVibration(flag, squeezeStrength, Time.deltaTime);
		}
		if (_raiseActivate)
		{
			_events?.Activate?.RaiseAll(particleFXEmissionSqueeze);
		}
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		if (IsMyItem())
		{
			bool flag = currentState == PositionState.InLeftHand;
			if ((bool)GorillaGameManager.instance)
			{
				GorillaGameManager.instance.FindVRRigForPlayer(PhotonNetwork.LocalPlayer).RPC("PlayHandTap", RpcTarget.All, squeezeReleaseSound, flag, 0.33f);
			}
			GorillaTagger.Instance.StartVibration(flag, releaseStrength, Time.deltaTime);
		}
		if (_raiseDeactivate)
		{
			_events?.Deactivate?.RaiseAll(particleFXEmissionIdle);
		}
	}

	public void PlayParticleFX(float rate)
	{
		if (hasParticleFX && (currentState == PositionState.InLeftHand || currentState == PositionState.InRightHand))
		{
			if (!fxActive)
			{
				fxActive = true;
			}
			squeezeTimeElapsed = 0f;
			pFXEmissionModule.rateOverTime = rate;
		}
	}

	public override bool CanActivate()
	{
		return !disableActivation;
	}

	public override bool CanDeactivate()
	{
		return !disableDeactivation;
	}
}
