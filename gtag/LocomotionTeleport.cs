using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;

public class LocomotionTeleport : MonoBehaviour
{
	public enum States
	{
		Ready,
		Aim,
		CancelAim,
		PreTeleport,
		CancelTeleport,
		Teleporting,
		PostTeleport
	}

	public enum TeleportIntentions
	{
		None,
		Aim,
		PreTeleport,
		Teleport
	}

	public enum AimCollisionTypes
	{
		Point,
		Sphere,
		Capsule
	}

	public class AimData
	{
		public RaycastHit TargetHitInfo;

		public bool TargetValid;

		public Vector3? Destination;

		public float Radius;

		public List<Vector3> Points { get; private set; }

		public AimData()
		{
			Points = new List<Vector3>();
		}

		public void Reset()
		{
			Points.Clear();
			TargetValid = false;
			Destination = null;
		}
	}

	[Tooltip("Allow linear movement prior to the teleport system being activated.")]
	public bool EnableMovementDuringReady = true;

	[Tooltip("Allow linear movement while the teleport system is in the process of aiming for a teleport target.")]
	public bool EnableMovementDuringAim = true;

	[Tooltip("Allow linear movement while the teleport system is in the process of configuring the landing orientation.")]
	public bool EnableMovementDuringPreTeleport = true;

	[Tooltip("Allow linear movement after the teleport has occurred but before the system has returned to the ready state.")]
	public bool EnableMovementDuringPostTeleport = true;

	[Tooltip("Allow rotation prior to the teleport system being activated.")]
	public bool EnableRotationDuringReady = true;

	[Tooltip("Allow rotation while the teleport system is in the process of aiming for a teleport target.")]
	public bool EnableRotationDuringAim = true;

	[Tooltip("Allow rotation while the teleport system is in the process of configuring the landing orientation.")]
	public bool EnableRotationDuringPreTeleport = true;

	[Tooltip("Allow rotation after the teleport has occurred but before the system has returned to the ready state.")]
	public bool EnableRotationDuringPostTeleport = true;

	[NonSerialized]
	public TeleportAimHandler AimHandler;

	[Tooltip("This prefab will be instantiated as needed and updated to match the current aim target.")]
	public TeleportDestination TeleportDestinationPrefab;

	[Tooltip("TeleportDestinationPrefab will be instantiated into this layer.")]
	public int TeleportDestinationLayer;

	[NonSerialized]
	public TeleportInputHandler InputHandler;

	[NonSerialized]
	public TeleportIntentions CurrentIntention;

	[NonSerialized]
	public bool IsPreTeleportRequested;

	[NonSerialized]
	public bool IsTransitioning;

	[NonSerialized]
	public bool IsPostTeleportRequested;

	private TeleportDestination _teleportDestination;

	[Tooltip("When aiming at possible destinations, the aim collision type determines which shape to use for collision tests.")]
	public AimCollisionTypes AimCollisionType;

	[Tooltip("Use the character collision radius/height/skinwidth for sphere/capsule collision tests.")]
	public bool UseCharacterCollisionData;

	[Tooltip("Radius of the sphere or capsule used for collision testing when aiming to possible teleport destinations. Ignored if UseCharacterCollisionData is true.")]
	public float AimCollisionRadius;

	[Tooltip("Height of the capsule used for collision testing when aiming to possible teleport destinations. Ignored if UseCharacterCollisionData is true.")]
	public float AimCollisionHeight;

	public States CurrentState { get; private set; }

	public Quaternion DestinationRotation => _teleportDestination.OrientationIndicator.rotation;

	public LocomotionController LocomotionController { get; private set; }

	public event Action<bool, Vector3?, Quaternion?, Quaternion?> UpdateTeleportDestination;

	public event Action EnterStateReady;

	public event Action EnterStateAim;

	public event Action<AimData> UpdateAimData;

	public event Action ExitStateAim;

	public event Action EnterStateCancelAim;

	public event Action EnterStatePreTeleport;

	public event Action EnterStateCancelTeleport;

	public event Action EnterStateTeleporting;

	public event Action EnterStatePostTeleport;

	public event Action<Transform, Vector3, Quaternion> Teleported;

	public void EnableMovement(bool ready, bool aim, bool pre, bool post)
	{
		EnableMovementDuringReady = ready;
		EnableMovementDuringAim = aim;
		EnableMovementDuringPreTeleport = pre;
		EnableMovementDuringPostTeleport = post;
	}

	public void EnableRotation(bool ready, bool aim, bool pre, bool post)
	{
		EnableRotationDuringReady = ready;
		EnableRotationDuringAim = aim;
		EnableRotationDuringPreTeleport = pre;
		EnableRotationDuringPostTeleport = post;
	}

	public void OnUpdateTeleportDestination(bool isValidDestination, Vector3? position, Quaternion? rotation, Quaternion? landingRotation)
	{
		if (this.UpdateTeleportDestination != null)
		{
			this.UpdateTeleportDestination(isValidDestination, position, rotation, landingRotation);
		}
	}

	public bool AimCollisionTest(Vector3 start, Vector3 end, LayerMask aimCollisionLayerMask, out RaycastHit hitInfo)
	{
		Vector3 vector = end - start;
		float magnitude = vector.magnitude;
		Vector3 direction = vector / magnitude;
		switch (AimCollisionType)
		{
		case AimCollisionTypes.Capsule:
		{
			float num;
			float num2;
			if (UseCharacterCollisionData)
			{
				CapsuleCollider characterController = LocomotionController.CharacterController;
				num = characterController.height;
				num2 = characterController.radius;
			}
			else
			{
				num = AimCollisionHeight;
				num2 = AimCollisionRadius;
			}
			return Physics.CapsuleCast(start + new Vector3(0f, num2, 0f), start + new Vector3(0f, num + num2, 0f), num2, direction, out hitInfo, magnitude, aimCollisionLayerMask, QueryTriggerInteraction.Ignore);
		}
		case AimCollisionTypes.Point:
			return Physics.Raycast(start, direction, out hitInfo, magnitude, aimCollisionLayerMask, QueryTriggerInteraction.Ignore);
		case AimCollisionTypes.Sphere:
		{
			float radius = ((!UseCharacterCollisionData) ? AimCollisionRadius : LocomotionController.CharacterController.radius);
			return Physics.SphereCast(start, radius, direction, out hitInfo, magnitude, aimCollisionLayerMask, QueryTriggerInteraction.Ignore);
		}
		default:
			throw new Exception();
		}
	}

	[Conditional("DEBUG_TELEPORT_STATES")]
	protected void LogState(string msg)
	{
		UnityEngine.Debug.Log(Time.frameCount + ": " + msg);
	}

	protected void CreateNewTeleportDestination()
	{
		TeleportDestinationPrefab.gameObject.SetActive(value: false);
		TeleportDestination teleportDestination = UnityEngine.Object.Instantiate(TeleportDestinationPrefab);
		teleportDestination.LocomotionTeleport = this;
		teleportDestination.gameObject.layer = TeleportDestinationLayer;
		_teleportDestination = teleportDestination;
		_teleportDestination.LocomotionTeleport = this;
	}

	private void DeactivateDestination()
	{
		_teleportDestination.OnDeactivated();
	}

	public void RecycleTeleportDestination(TeleportDestination oldDestination)
	{
		if (oldDestination == _teleportDestination)
		{
			CreateNewTeleportDestination();
		}
		UnityEngine.Object.Destroy(oldDestination.gameObject);
	}

	private void EnableMotion(bool enableLinear, bool enableRotation)
	{
		LocomotionController.PlayerController.EnableLinearMovement = enableLinear;
		LocomotionController.PlayerController.EnableRotation = enableRotation;
	}

	private void Awake()
	{
		LocomotionController = GetComponent<LocomotionController>();
		CreateNewTeleportDestination();
	}

	public virtual void OnEnable()
	{
		CurrentState = States.Ready;
		StartCoroutine(ReadyStateCoroutine());
	}

	public virtual void OnDisable()
	{
		StopAllCoroutines();
	}

	protected IEnumerator ReadyStateCoroutine()
	{
		yield return null;
		CurrentState = States.Ready;
		EnableMotion(EnableMovementDuringReady, EnableRotationDuringReady);
		if (this.EnterStateReady != null)
		{
			this.EnterStateReady();
		}
		while (CurrentIntention != TeleportIntentions.Aim)
		{
			yield return null;
		}
		yield return null;
		StartCoroutine(AimStateCoroutine());
	}

	public void OnUpdateAimData(AimData aimData)
	{
		if (this.UpdateAimData != null)
		{
			this.UpdateAimData(aimData);
		}
	}

	protected IEnumerator AimStateCoroutine()
	{
		CurrentState = States.Aim;
		EnableMotion(EnableMovementDuringAim, EnableRotationDuringAim);
		if (this.EnterStateAim != null)
		{
			this.EnterStateAim();
		}
		_teleportDestination.gameObject.SetActive(value: true);
		while (CurrentIntention == TeleportIntentions.Aim)
		{
			yield return null;
		}
		if (this.ExitStateAim != null)
		{
			this.ExitStateAim();
		}
		yield return null;
		if ((CurrentIntention == TeleportIntentions.PreTeleport || CurrentIntention == TeleportIntentions.Teleport) && _teleportDestination.IsValidDestination)
		{
			StartCoroutine(PreTeleportStateCoroutine());
		}
		else
		{
			StartCoroutine(CancelAimStateCoroutine());
		}
	}

	protected IEnumerator CancelAimStateCoroutine()
	{
		CurrentState = States.CancelAim;
		if (this.EnterStateCancelAim != null)
		{
			this.EnterStateCancelAim();
		}
		DeactivateDestination();
		yield return null;
		StartCoroutine(ReadyStateCoroutine());
	}

	protected IEnumerator PreTeleportStateCoroutine()
	{
		CurrentState = States.PreTeleport;
		EnableMotion(EnableMovementDuringPreTeleport, EnableRotationDuringPreTeleport);
		if (this.EnterStatePreTeleport != null)
		{
			this.EnterStatePreTeleport();
		}
		while (CurrentIntention == TeleportIntentions.PreTeleport || IsPreTeleportRequested)
		{
			yield return null;
		}
		if (_teleportDestination.IsValidDestination)
		{
			StartCoroutine(TeleportingStateCoroutine());
		}
		else
		{
			StartCoroutine(CancelTeleportStateCoroutine());
		}
	}

	protected IEnumerator CancelTeleportStateCoroutine()
	{
		CurrentState = States.CancelTeleport;
		if (this.EnterStateCancelTeleport != null)
		{
			this.EnterStateCancelTeleport();
		}
		DeactivateDestination();
		yield return null;
		StartCoroutine(ReadyStateCoroutine());
	}

	protected IEnumerator TeleportingStateCoroutine()
	{
		CurrentState = States.Teleporting;
		EnableMotion(enableLinear: false, enableRotation: false);
		if (this.EnterStateTeleporting != null)
		{
			this.EnterStateTeleporting();
		}
		while (IsTransitioning)
		{
			yield return null;
		}
		yield return null;
		StartCoroutine(PostTeleportStateCoroutine());
	}

	protected IEnumerator PostTeleportStateCoroutine()
	{
		CurrentState = States.PostTeleport;
		EnableMotion(EnableMovementDuringPostTeleport, EnableRotationDuringPostTeleport);
		if (this.EnterStatePostTeleport != null)
		{
			this.EnterStatePostTeleport();
		}
		while (IsPostTeleportRequested)
		{
			yield return null;
		}
		DeactivateDestination();
		yield return null;
		StartCoroutine(ReadyStateCoroutine());
	}

	public void DoTeleport()
	{
		CapsuleCollider characterController = LocomotionController.CharacterController;
		Transform transform = characterController.transform;
		Vector3 position = _teleportDestination.OrientationIndicator.position;
		position.y += characterController.height * 0.5f;
		Quaternion landingRotation = _teleportDestination.LandingRotation;
		if (this.Teleported != null)
		{
			this.Teleported(transform, position, landingRotation);
		}
		transform.position = position;
		transform.rotation = landingRotation;
	}

	public Vector3 GetCharacterPosition()
	{
		return LocomotionController.CharacterController.transform.position;
	}

	public Quaternion GetHeadRotationY()
	{
		Quaternion value = Quaternion.identity;
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(XRNode.Head);
		if (deviceAtXRNode.isValid)
		{
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out value);
		}
		Vector3 eulerAngles = value.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		return Quaternion.Euler(eulerAngles);
	}

	public void DoWarp(Vector3 startPos, float positionPercent)
	{
		Vector3 position = _teleportDestination.OrientationIndicator.position;
		position.y += LocomotionController.CharacterController.height / 2f;
		Transform obj = LocomotionController.CharacterController.transform;
		Vector3 position2 = Vector3.Lerp(startPos, position, positionPercent);
		obj.position = position2;
	}
}
