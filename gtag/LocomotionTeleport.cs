using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;

public class LocomotionTeleport : MonoBehaviour
{
	public void EnableMovement(bool ready, bool aim, bool pre, bool post)
	{
		this.EnableMovementDuringReady = ready;
		this.EnableMovementDuringAim = aim;
		this.EnableMovementDuringPreTeleport = pre;
		this.EnableMovementDuringPostTeleport = post;
	}

	public void EnableRotation(bool ready, bool aim, bool pre, bool post)
	{
		this.EnableRotationDuringReady = ready;
		this.EnableRotationDuringAim = aim;
		this.EnableRotationDuringPreTeleport = pre;
		this.EnableRotationDuringPostTeleport = post;
	}

	public LocomotionTeleport.States CurrentState { get; private set; }

	public event Action<bool, Vector3?, Quaternion?, Quaternion?> UpdateTeleportDestination;

	public void OnUpdateTeleportDestination(bool isValidDestination, Vector3? position, Quaternion? rotation, Quaternion? landingRotation)
	{
		if (this.UpdateTeleportDestination != null)
		{
			this.UpdateTeleportDestination(isValidDestination, position, rotation, landingRotation);
		}
	}

	public Quaternion DestinationRotation
	{
		get
		{
			return this._teleportDestination.OrientationIndicator.rotation;
		}
	}

	public LocomotionController LocomotionController { get; private set; }

	public bool AimCollisionTest(Vector3 start, Vector3 end, LayerMask aimCollisionLayerMask, out RaycastHit hitInfo)
	{
		Vector3 vector = end - start;
		float magnitude = vector.magnitude;
		Vector3 vector2 = vector / magnitude;
		switch (this.AimCollisionType)
		{
		case LocomotionTeleport.AimCollisionTypes.Point:
			return Physics.Raycast(start, vector2, out hitInfo, magnitude, aimCollisionLayerMask, QueryTriggerInteraction.Ignore);
		case LocomotionTeleport.AimCollisionTypes.Sphere:
		{
			float num;
			if (this.UseCharacterCollisionData)
			{
				num = this.LocomotionController.CharacterController.radius;
			}
			else
			{
				num = this.AimCollisionRadius;
			}
			return Physics.SphereCast(start, num, vector2, out hitInfo, magnitude, aimCollisionLayerMask, QueryTriggerInteraction.Ignore);
		}
		case LocomotionTeleport.AimCollisionTypes.Capsule:
		{
			float num2;
			float num3;
			if (this.UseCharacterCollisionData)
			{
				CapsuleCollider characterController = this.LocomotionController.CharacterController;
				num2 = characterController.height;
				num3 = characterController.radius;
			}
			else
			{
				num2 = this.AimCollisionHeight;
				num3 = this.AimCollisionRadius;
			}
			return Physics.CapsuleCast(start + new Vector3(0f, num3, 0f), start + new Vector3(0f, num2 + num3, 0f), num3, vector2, out hitInfo, magnitude, aimCollisionLayerMask, QueryTriggerInteraction.Ignore);
		}
		default:
			throw new Exception();
		}
	}

	[Conditional("DEBUG_TELEPORT_STATES")]
	protected void LogState(string msg)
	{
		Debug.Log(Time.frameCount.ToString() + ": " + msg);
	}

	protected void CreateNewTeleportDestination()
	{
		this.TeleportDestinationPrefab.gameObject.SetActive(false);
		TeleportDestination teleportDestination = Object.Instantiate<TeleportDestination>(this.TeleportDestinationPrefab);
		teleportDestination.LocomotionTeleport = this;
		teleportDestination.gameObject.layer = this.TeleportDestinationLayer;
		this._teleportDestination = teleportDestination;
		this._teleportDestination.LocomotionTeleport = this;
	}

	private void DeactivateDestination()
	{
		this._teleportDestination.OnDeactivated();
	}

	public void RecycleTeleportDestination(TeleportDestination oldDestination)
	{
		if (oldDestination == this._teleportDestination)
		{
			this.CreateNewTeleportDestination();
		}
		Object.Destroy(oldDestination.gameObject);
	}

	private void EnableMotion(bool enableLinear, bool enableRotation)
	{
		this.LocomotionController.PlayerController.EnableLinearMovement = enableLinear;
		this.LocomotionController.PlayerController.EnableRotation = enableRotation;
	}

	private void Awake()
	{
		this.LocomotionController = base.GetComponent<LocomotionController>();
		this.CreateNewTeleportDestination();
	}

	public virtual void OnEnable()
	{
		this.CurrentState = LocomotionTeleport.States.Ready;
		base.StartCoroutine(this.ReadyStateCoroutine());
	}

	public virtual void OnDisable()
	{
		base.StopAllCoroutines();
	}

	public event Action EnterStateReady;

	protected IEnumerator ReadyStateCoroutine()
	{
		yield return null;
		this.CurrentState = LocomotionTeleport.States.Ready;
		this.EnableMotion(this.EnableMovementDuringReady, this.EnableRotationDuringReady);
		if (this.EnterStateReady != null)
		{
			this.EnterStateReady();
		}
		while (this.CurrentIntention != LocomotionTeleport.TeleportIntentions.Aim)
		{
			yield return null;
		}
		yield return null;
		base.StartCoroutine(this.AimStateCoroutine());
		yield break;
	}

	public event Action EnterStateAim;

	public event Action<LocomotionTeleport.AimData> UpdateAimData;

	public void OnUpdateAimData(LocomotionTeleport.AimData aimData)
	{
		if (this.UpdateAimData != null)
		{
			this.UpdateAimData(aimData);
		}
	}

	public event Action ExitStateAim;

	protected IEnumerator AimStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.Aim;
		this.EnableMotion(this.EnableMovementDuringAim, this.EnableRotationDuringAim);
		if (this.EnterStateAim != null)
		{
			this.EnterStateAim();
		}
		this._teleportDestination.gameObject.SetActive(true);
		while (this.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim)
		{
			yield return null;
		}
		if (this.ExitStateAim != null)
		{
			this.ExitStateAim();
		}
		yield return null;
		if ((this.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport || this.CurrentIntention == LocomotionTeleport.TeleportIntentions.Teleport) && this._teleportDestination.IsValidDestination)
		{
			base.StartCoroutine(this.PreTeleportStateCoroutine());
		}
		else
		{
			base.StartCoroutine(this.CancelAimStateCoroutine());
		}
		yield break;
	}

	public event Action EnterStateCancelAim;

	protected IEnumerator CancelAimStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.CancelAim;
		if (this.EnterStateCancelAim != null)
		{
			this.EnterStateCancelAim();
		}
		this.DeactivateDestination();
		yield return null;
		base.StartCoroutine(this.ReadyStateCoroutine());
		yield break;
	}

	public event Action EnterStatePreTeleport;

	protected IEnumerator PreTeleportStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.PreTeleport;
		this.EnableMotion(this.EnableMovementDuringPreTeleport, this.EnableRotationDuringPreTeleport);
		if (this.EnterStatePreTeleport != null)
		{
			this.EnterStatePreTeleport();
		}
		while (this.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport || this.IsPreTeleportRequested)
		{
			yield return null;
		}
		if (this._teleportDestination.IsValidDestination)
		{
			base.StartCoroutine(this.TeleportingStateCoroutine());
		}
		else
		{
			base.StartCoroutine(this.CancelTeleportStateCoroutine());
		}
		yield break;
	}

	public event Action EnterStateCancelTeleport;

	protected IEnumerator CancelTeleportStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.CancelTeleport;
		if (this.EnterStateCancelTeleport != null)
		{
			this.EnterStateCancelTeleport();
		}
		this.DeactivateDestination();
		yield return null;
		base.StartCoroutine(this.ReadyStateCoroutine());
		yield break;
	}

	public event Action EnterStateTeleporting;

	protected IEnumerator TeleportingStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.Teleporting;
		this.EnableMotion(false, false);
		if (this.EnterStateTeleporting != null)
		{
			this.EnterStateTeleporting();
		}
		while (this.IsTransitioning)
		{
			yield return null;
		}
		yield return null;
		base.StartCoroutine(this.PostTeleportStateCoroutine());
		yield break;
	}

	public event Action EnterStatePostTeleport;

	protected IEnumerator PostTeleportStateCoroutine()
	{
		this.CurrentState = LocomotionTeleport.States.PostTeleport;
		this.EnableMotion(this.EnableMovementDuringPostTeleport, this.EnableRotationDuringPostTeleport);
		if (this.EnterStatePostTeleport != null)
		{
			this.EnterStatePostTeleport();
		}
		while (this.IsPostTeleportRequested)
		{
			yield return null;
		}
		this.DeactivateDestination();
		yield return null;
		base.StartCoroutine(this.ReadyStateCoroutine());
		yield break;
	}

	public event Action<Transform, Vector3, Quaternion> Teleported;

	public void DoTeleport()
	{
		CapsuleCollider characterController = this.LocomotionController.CharacterController;
		Transform transform = characterController.transform;
		Vector3 position = this._teleportDestination.OrientationIndicator.position;
		position.y += characterController.height * 0.5f;
		Quaternion landingRotation = this._teleportDestination.LandingRotation;
		if (this.Teleported != null)
		{
			this.Teleported(transform, position, landingRotation);
		}
		transform.position = position;
		transform.rotation = landingRotation;
	}

	public Vector3 GetCharacterPosition()
	{
		return this.LocomotionController.CharacterController.transform.position;
	}

	public Quaternion GetHeadRotationY()
	{
		Quaternion quaternion = Quaternion.identity;
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(XRNode.Head);
		if (deviceAtXRNode.isValid)
		{
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out quaternion);
		}
		Vector3 eulerAngles = quaternion.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		quaternion = Quaternion.Euler(eulerAngles);
		return quaternion;
	}

	public void DoWarp(Vector3 startPos, float positionPercent)
	{
		Vector3 position = this._teleportDestination.OrientationIndicator.position;
		position.y += this.LocomotionController.CharacterController.height / 2f;
		Transform transform = this.LocomotionController.CharacterController.transform;
		Vector3 vector = Vector3.Lerp(startPos, position, positionPercent);
		transform.position = vector;
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
	public LocomotionTeleport.TeleportIntentions CurrentIntention;

	[NonSerialized]
	public bool IsPreTeleportRequested;

	[NonSerialized]
	public bool IsTransitioning;

	[NonSerialized]
	public bool IsPostTeleportRequested;

	private TeleportDestination _teleportDestination;

	[Tooltip("When aiming at possible destinations, the aim collision type determines which shape to use for collision tests.")]
	public LocomotionTeleport.AimCollisionTypes AimCollisionType;

	[Tooltip("Use the character collision radius/height/skinwidth for sphere/capsule collision tests.")]
	public bool UseCharacterCollisionData;

	[Tooltip("Radius of the sphere or capsule used for collision testing when aiming to possible teleport destinations. Ignored if UseCharacterCollisionData is true.")]
	public float AimCollisionRadius;

	[Tooltip("Height of the capsule used for collision testing when aiming to possible teleport destinations. Ignored if UseCharacterCollisionData is true.")]
	public float AimCollisionHeight;

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
		public AimData()
		{
			this.Points = new List<Vector3>();
		}

		public List<Vector3> Points { get; private set; }

		public void Reset()
		{
			this.Points.Clear();
			this.TargetValid = false;
			this.Destination = null;
		}

		public RaycastHit TargetHitInfo;

		public bool TargetValid;

		public Vector3? Destination;

		public float Radius;
	}
}
