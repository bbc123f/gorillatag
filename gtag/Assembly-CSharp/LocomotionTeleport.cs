using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000082 RID: 130
public class LocomotionTeleport : MonoBehaviour
{
	// Token: 0x0600029E RID: 670 RVA: 0x00011533 File Offset: 0x0000F733
	public void EnableMovement(bool ready, bool aim, bool pre, bool post)
	{
		this.EnableMovementDuringReady = ready;
		this.EnableMovementDuringAim = aim;
		this.EnableMovementDuringPreTeleport = pre;
		this.EnableMovementDuringPostTeleport = post;
	}

	// Token: 0x0600029F RID: 671 RVA: 0x00011552 File Offset: 0x0000F752
	public void EnableRotation(bool ready, bool aim, bool pre, bool post)
	{
		this.EnableRotationDuringReady = ready;
		this.EnableRotationDuringAim = aim;
		this.EnableRotationDuringPreTeleport = pre;
		this.EnableRotationDuringPostTeleport = post;
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x060002A0 RID: 672 RVA: 0x00011571 File Offset: 0x0000F771
	// (set) Token: 0x060002A1 RID: 673 RVA: 0x00011579 File Offset: 0x0000F779
	public LocomotionTeleport.States CurrentState { get; private set; }

	// Token: 0x14000003 RID: 3
	// (add) Token: 0x060002A2 RID: 674 RVA: 0x00011584 File Offset: 0x0000F784
	// (remove) Token: 0x060002A3 RID: 675 RVA: 0x000115BC File Offset: 0x0000F7BC
	public event Action<bool, Vector3?, Quaternion?, Quaternion?> UpdateTeleportDestination;

	// Token: 0x060002A4 RID: 676 RVA: 0x000115F1 File Offset: 0x0000F7F1
	public void OnUpdateTeleportDestination(bool isValidDestination, Vector3? position, Quaternion? rotation, Quaternion? landingRotation)
	{
		if (this.UpdateTeleportDestination != null)
		{
			this.UpdateTeleportDestination(isValidDestination, position, rotation, landingRotation);
		}
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x060002A5 RID: 677 RVA: 0x0001160B File Offset: 0x0000F80B
	public Quaternion DestinationRotation
	{
		get
		{
			return this._teleportDestination.OrientationIndicator.rotation;
		}
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x060002A6 RID: 678 RVA: 0x0001161D File Offset: 0x0000F81D
	// (set) Token: 0x060002A7 RID: 679 RVA: 0x00011625 File Offset: 0x0000F825
	public LocomotionController LocomotionController { get; private set; }

	// Token: 0x060002A8 RID: 680 RVA: 0x00011630 File Offset: 0x0000F830
	public bool AimCollisionTest(Vector3 start, Vector3 end, LayerMask aimCollisionLayerMask, out RaycastHit hitInfo)
	{
		Vector3 a = end - start;
		float magnitude = a.magnitude;
		Vector3 direction = a / magnitude;
		switch (this.AimCollisionType)
		{
		case LocomotionTeleport.AimCollisionTypes.Point:
			return Physics.Raycast(start, direction, out hitInfo, magnitude, aimCollisionLayerMask, QueryTriggerInteraction.Ignore);
		case LocomotionTeleport.AimCollisionTypes.Sphere:
		{
			float radius;
			if (this.UseCharacterCollisionData)
			{
				radius = this.LocomotionController.CharacterController.radius;
			}
			else
			{
				radius = this.AimCollisionRadius;
			}
			return Physics.SphereCast(start, radius, direction, out hitInfo, magnitude, aimCollisionLayerMask, QueryTriggerInteraction.Ignore);
		}
		case LocomotionTeleport.AimCollisionTypes.Capsule:
		{
			float num;
			float num2;
			if (this.UseCharacterCollisionData)
			{
				CapsuleCollider characterController = this.LocomotionController.CharacterController;
				num = characterController.height;
				num2 = characterController.radius;
			}
			else
			{
				num = this.AimCollisionHeight;
				num2 = this.AimCollisionRadius;
			}
			return Physics.CapsuleCast(start + new Vector3(0f, num2, 0f), start + new Vector3(0f, num + num2, 0f), num2, direction, out hitInfo, magnitude, aimCollisionLayerMask, QueryTriggerInteraction.Ignore);
		}
		default:
			throw new Exception();
		}
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x0001173C File Offset: 0x0000F93C
	[Conditional("DEBUG_TELEPORT_STATES")]
	protected void LogState(string msg)
	{
		Debug.Log(Time.frameCount.ToString() + ": " + msg);
	}

	// Token: 0x060002AA RID: 682 RVA: 0x00011768 File Offset: 0x0000F968
	protected void CreateNewTeleportDestination()
	{
		this.TeleportDestinationPrefab.gameObject.SetActive(false);
		TeleportDestination teleportDestination = Object.Instantiate<TeleportDestination>(this.TeleportDestinationPrefab);
		teleportDestination.LocomotionTeleport = this;
		teleportDestination.gameObject.layer = this.TeleportDestinationLayer;
		this._teleportDestination = teleportDestination;
		this._teleportDestination.LocomotionTeleport = this;
	}

	// Token: 0x060002AB RID: 683 RVA: 0x000117BD File Offset: 0x0000F9BD
	private void DeactivateDestination()
	{
		this._teleportDestination.OnDeactivated();
	}

	// Token: 0x060002AC RID: 684 RVA: 0x000117CA File Offset: 0x0000F9CA
	public void RecycleTeleportDestination(TeleportDestination oldDestination)
	{
		if (oldDestination == this._teleportDestination)
		{
			this.CreateNewTeleportDestination();
		}
		Object.Destroy(oldDestination.gameObject);
	}

	// Token: 0x060002AD RID: 685 RVA: 0x000117EB File Offset: 0x0000F9EB
	private void EnableMotion(bool enableLinear, bool enableRotation)
	{
		this.LocomotionController.PlayerController.EnableLinearMovement = enableLinear;
		this.LocomotionController.PlayerController.EnableRotation = enableRotation;
	}

	// Token: 0x060002AE RID: 686 RVA: 0x0001180F File Offset: 0x0000FA0F
	private void Awake()
	{
		this.LocomotionController = base.GetComponent<LocomotionController>();
		this.CreateNewTeleportDestination();
	}

	// Token: 0x060002AF RID: 687 RVA: 0x00011823 File Offset: 0x0000FA23
	public virtual void OnEnable()
	{
		this.CurrentState = LocomotionTeleport.States.Ready;
		base.StartCoroutine(this.ReadyStateCoroutine());
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x00011839 File Offset: 0x0000FA39
	public virtual void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x14000004 RID: 4
	// (add) Token: 0x060002B1 RID: 689 RVA: 0x00011844 File Offset: 0x0000FA44
	// (remove) Token: 0x060002B2 RID: 690 RVA: 0x0001187C File Offset: 0x0000FA7C
	public event Action EnterStateReady;

	// Token: 0x060002B3 RID: 691 RVA: 0x000118B1 File Offset: 0x0000FAB1
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

	// Token: 0x14000005 RID: 5
	// (add) Token: 0x060002B4 RID: 692 RVA: 0x000118C0 File Offset: 0x0000FAC0
	// (remove) Token: 0x060002B5 RID: 693 RVA: 0x000118F8 File Offset: 0x0000FAF8
	public event Action EnterStateAim;

	// Token: 0x14000006 RID: 6
	// (add) Token: 0x060002B6 RID: 694 RVA: 0x00011930 File Offset: 0x0000FB30
	// (remove) Token: 0x060002B7 RID: 695 RVA: 0x00011968 File Offset: 0x0000FB68
	public event Action<LocomotionTeleport.AimData> UpdateAimData;

	// Token: 0x060002B8 RID: 696 RVA: 0x0001199D File Offset: 0x0000FB9D
	public void OnUpdateAimData(LocomotionTeleport.AimData aimData)
	{
		if (this.UpdateAimData != null)
		{
			this.UpdateAimData(aimData);
		}
	}

	// Token: 0x14000007 RID: 7
	// (add) Token: 0x060002B9 RID: 697 RVA: 0x000119B4 File Offset: 0x0000FBB4
	// (remove) Token: 0x060002BA RID: 698 RVA: 0x000119EC File Offset: 0x0000FBEC
	public event Action ExitStateAim;

	// Token: 0x060002BB RID: 699 RVA: 0x00011A21 File Offset: 0x0000FC21
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

	// Token: 0x14000008 RID: 8
	// (add) Token: 0x060002BC RID: 700 RVA: 0x00011A30 File Offset: 0x0000FC30
	// (remove) Token: 0x060002BD RID: 701 RVA: 0x00011A68 File Offset: 0x0000FC68
	public event Action EnterStateCancelAim;

	// Token: 0x060002BE RID: 702 RVA: 0x00011A9D File Offset: 0x0000FC9D
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

	// Token: 0x14000009 RID: 9
	// (add) Token: 0x060002BF RID: 703 RVA: 0x00011AAC File Offset: 0x0000FCAC
	// (remove) Token: 0x060002C0 RID: 704 RVA: 0x00011AE4 File Offset: 0x0000FCE4
	public event Action EnterStatePreTeleport;

	// Token: 0x060002C1 RID: 705 RVA: 0x00011B19 File Offset: 0x0000FD19
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

	// Token: 0x1400000A RID: 10
	// (add) Token: 0x060002C2 RID: 706 RVA: 0x00011B28 File Offset: 0x0000FD28
	// (remove) Token: 0x060002C3 RID: 707 RVA: 0x00011B60 File Offset: 0x0000FD60
	public event Action EnterStateCancelTeleport;

	// Token: 0x060002C4 RID: 708 RVA: 0x00011B95 File Offset: 0x0000FD95
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

	// Token: 0x1400000B RID: 11
	// (add) Token: 0x060002C5 RID: 709 RVA: 0x00011BA4 File Offset: 0x0000FDA4
	// (remove) Token: 0x060002C6 RID: 710 RVA: 0x00011BDC File Offset: 0x0000FDDC
	public event Action EnterStateTeleporting;

	// Token: 0x060002C7 RID: 711 RVA: 0x00011C11 File Offset: 0x0000FE11
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

	// Token: 0x1400000C RID: 12
	// (add) Token: 0x060002C8 RID: 712 RVA: 0x00011C20 File Offset: 0x0000FE20
	// (remove) Token: 0x060002C9 RID: 713 RVA: 0x00011C58 File Offset: 0x0000FE58
	public event Action EnterStatePostTeleport;

	// Token: 0x060002CA RID: 714 RVA: 0x00011C8D File Offset: 0x0000FE8D
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

	// Token: 0x1400000D RID: 13
	// (add) Token: 0x060002CB RID: 715 RVA: 0x00011C9C File Offset: 0x0000FE9C
	// (remove) Token: 0x060002CC RID: 716 RVA: 0x00011CD4 File Offset: 0x0000FED4
	public event Action<Transform, Vector3, Quaternion> Teleported;

	// Token: 0x060002CD RID: 717 RVA: 0x00011D0C File Offset: 0x0000FF0C
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

	// Token: 0x060002CE RID: 718 RVA: 0x00011D84 File Offset: 0x0000FF84
	public Vector3 GetCharacterPosition()
	{
		return this.LocomotionController.CharacterController.transform.position;
	}

	// Token: 0x060002CF RID: 719 RVA: 0x00011D9C File Offset: 0x0000FF9C
	public Quaternion GetHeadRotationY()
	{
		Quaternion result = Quaternion.identity;
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(XRNode.Head);
		if (deviceAtXRNode.isValid)
		{
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out result);
		}
		Vector3 eulerAngles = result.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		result = Quaternion.Euler(eulerAngles);
		return result;
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x00011DF8 File Offset: 0x0000FFF8
	public void DoWarp(Vector3 startPos, float positionPercent)
	{
		Vector3 position = this._teleportDestination.OrientationIndicator.position;
		position.y += this.LocomotionController.CharacterController.height / 2f;
		Transform transform = this.LocomotionController.CharacterController.transform;
		Vector3 position2 = Vector3.Lerp(startPos, position, positionPercent);
		transform.position = position2;
	}

	// Token: 0x04000375 RID: 885
	[Tooltip("Allow linear movement prior to the teleport system being activated.")]
	public bool EnableMovementDuringReady = true;

	// Token: 0x04000376 RID: 886
	[Tooltip("Allow linear movement while the teleport system is in the process of aiming for a teleport target.")]
	public bool EnableMovementDuringAim = true;

	// Token: 0x04000377 RID: 887
	[Tooltip("Allow linear movement while the teleport system is in the process of configuring the landing orientation.")]
	public bool EnableMovementDuringPreTeleport = true;

	// Token: 0x04000378 RID: 888
	[Tooltip("Allow linear movement after the teleport has occurred but before the system has returned to the ready state.")]
	public bool EnableMovementDuringPostTeleport = true;

	// Token: 0x04000379 RID: 889
	[Tooltip("Allow rotation prior to the teleport system being activated.")]
	public bool EnableRotationDuringReady = true;

	// Token: 0x0400037A RID: 890
	[Tooltip("Allow rotation while the teleport system is in the process of aiming for a teleport target.")]
	public bool EnableRotationDuringAim = true;

	// Token: 0x0400037B RID: 891
	[Tooltip("Allow rotation while the teleport system is in the process of configuring the landing orientation.")]
	public bool EnableRotationDuringPreTeleport = true;

	// Token: 0x0400037C RID: 892
	[Tooltip("Allow rotation after the teleport has occurred but before the system has returned to the ready state.")]
	public bool EnableRotationDuringPostTeleport = true;

	// Token: 0x0400037E RID: 894
	[NonSerialized]
	public TeleportAimHandler AimHandler;

	// Token: 0x0400037F RID: 895
	[Tooltip("This prefab will be instantiated as needed and updated to match the current aim target.")]
	public TeleportDestination TeleportDestinationPrefab;

	// Token: 0x04000380 RID: 896
	[Tooltip("TeleportDestinationPrefab will be instantiated into this layer.")]
	public int TeleportDestinationLayer;

	// Token: 0x04000382 RID: 898
	[NonSerialized]
	public TeleportInputHandler InputHandler;

	// Token: 0x04000383 RID: 899
	[NonSerialized]
	public LocomotionTeleport.TeleportIntentions CurrentIntention;

	// Token: 0x04000384 RID: 900
	[NonSerialized]
	public bool IsPreTeleportRequested;

	// Token: 0x04000385 RID: 901
	[NonSerialized]
	public bool IsTransitioning;

	// Token: 0x04000386 RID: 902
	[NonSerialized]
	public bool IsPostTeleportRequested;

	// Token: 0x04000387 RID: 903
	private TeleportDestination _teleportDestination;

	// Token: 0x04000389 RID: 905
	[Tooltip("When aiming at possible destinations, the aim collision type determines which shape to use for collision tests.")]
	public LocomotionTeleport.AimCollisionTypes AimCollisionType;

	// Token: 0x0400038A RID: 906
	[Tooltip("Use the character collision radius/height/skinwidth for sphere/capsule collision tests.")]
	public bool UseCharacterCollisionData;

	// Token: 0x0400038B RID: 907
	[Tooltip("Radius of the sphere or capsule used for collision testing when aiming to possible teleport destinations. Ignored if UseCharacterCollisionData is true.")]
	public float AimCollisionRadius;

	// Token: 0x0400038C RID: 908
	[Tooltip("Height of the capsule used for collision testing when aiming to possible teleport destinations. Ignored if UseCharacterCollisionData is true.")]
	public float AimCollisionHeight;

	// Token: 0x020003B0 RID: 944
	public enum States
	{
		// Token: 0x04001B80 RID: 7040
		Ready,
		// Token: 0x04001B81 RID: 7041
		Aim,
		// Token: 0x04001B82 RID: 7042
		CancelAim,
		// Token: 0x04001B83 RID: 7043
		PreTeleport,
		// Token: 0x04001B84 RID: 7044
		CancelTeleport,
		// Token: 0x04001B85 RID: 7045
		Teleporting,
		// Token: 0x04001B86 RID: 7046
		PostTeleport
	}

	// Token: 0x020003B1 RID: 945
	public enum TeleportIntentions
	{
		// Token: 0x04001B88 RID: 7048
		None,
		// Token: 0x04001B89 RID: 7049
		Aim,
		// Token: 0x04001B8A RID: 7050
		PreTeleport,
		// Token: 0x04001B8B RID: 7051
		Teleport
	}

	// Token: 0x020003B2 RID: 946
	public enum AimCollisionTypes
	{
		// Token: 0x04001B8D RID: 7053
		Point,
		// Token: 0x04001B8E RID: 7054
		Sphere,
		// Token: 0x04001B8F RID: 7055
		Capsule
	}

	// Token: 0x020003B3 RID: 947
	public class AimData
	{
		// Token: 0x06001B0A RID: 6922 RVA: 0x000951FE File Offset: 0x000933FE
		public AimData()
		{
			this.Points = new List<Vector3>();
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06001B0B RID: 6923 RVA: 0x00095211 File Offset: 0x00093411
		// (set) Token: 0x06001B0C RID: 6924 RVA: 0x00095219 File Offset: 0x00093419
		public List<Vector3> Points { get; private set; }

		// Token: 0x06001B0D RID: 6925 RVA: 0x00095222 File Offset: 0x00093422
		public void Reset()
		{
			this.Points.Clear();
			this.TargetValid = false;
			this.Destination = null;
		}

		// Token: 0x04001B90 RID: 7056
		public RaycastHit TargetHitInfo;

		// Token: 0x04001B91 RID: 7057
		public bool TargetValid;

		// Token: 0x04001B92 RID: 7058
		public Vector3? Destination;

		// Token: 0x04001B93 RID: 7059
		public float Radius;
	}
}
