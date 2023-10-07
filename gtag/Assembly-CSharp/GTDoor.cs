using System;
using BoingKit;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200005D RID: 93
public class GTDoor : MonoBehaviourPun
{
	// Token: 0x060001C2 RID: 450 RVA: 0x0000C9EC File Offset: 0x0000ABEC
	private void Start()
	{
		Collider[] array = this.doorColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		this.tLastOpened = 0f;
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000CA24 File Offset: 0x0000AC24
	private void Update()
	{
		this.UpdateDoorState();
		this.UpdateDoorAnimation();
		Collider[] array;
		if (this.currentState == GTDoor.DoorState.Closed)
		{
			array = this.doorColliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
			return;
		}
		array = this.doorColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000CA84 File Offset: 0x0000AC84
	private void UpdateDoorState()
	{
		this.peopleInHoldOpenVolume = false;
		foreach (GTDoorTrigger gtdoorTrigger in this.doorHoldOpenTriggers)
		{
			gtdoorTrigger.ValidateOverlappingColliders();
			if (gtdoorTrigger.overlapCount > 0)
			{
				this.peopleInHoldOpenVolume = true;
				break;
			}
		}
		this.buttonTriggeredThisFrame = false;
		foreach (GTDoorTrigger gtdoorTrigger2 in this.doorButtonTriggers)
		{
			this.buttonTriggeredThisFrame = (this.buttonTriggeredThisFrame || gtdoorTrigger2.TriggeredThisFrame);
		}
		switch (this.currentState)
		{
		case GTDoor.DoorState.Closed:
			if (this.buttonTriggeredThisFrame)
			{
				if (!PhotonNetwork.InRoom)
				{
					this.OpenDoor();
				}
				else
				{
					base.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[]
					{
						GTDoor.DoorState.Opening
					});
					this.currentState = GTDoor.DoorState.OpeningWaitingOnRPC;
				}
			}
			break;
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
			break;
		case GTDoor.DoorState.Closing:
			if (this.doorSpring.Value < 1f)
			{
				this.currentState = GTDoor.DoorState.Closed;
			}
			if (this.peopleInHoldOpenVolume)
			{
				if (PhotonNetwork.InRoom && base.photonView.IsMine)
				{
					base.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[]
					{
						GTDoor.DoorState.HeldOpen
					});
				}
				this.audioSource.PlayOneShot(this.openSound);
				this.currentState = GTDoor.DoorState.HeldOpenLocally;
			}
			break;
		case GTDoor.DoorState.Open:
			if (Time.time - this.tLastOpened > this.timeUntilDoorCloses)
			{
				if (this.peopleInHoldOpenVolume)
				{
					if (PhotonNetwork.InRoom && base.photonView.IsMine)
					{
						base.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[]
						{
							GTDoor.DoorState.HeldOpen
						});
					}
					this.currentState = GTDoor.DoorState.HeldOpenLocally;
				}
				else if (!PhotonNetwork.InRoom)
				{
					this.CloseDoor();
				}
				else if (base.photonView.IsMine)
				{
					base.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[]
					{
						GTDoor.DoorState.Closing
					});
					this.currentState = GTDoor.DoorState.ClosingWaitingOnRPC;
				}
			}
			break;
		case GTDoor.DoorState.Opening:
			if (this.doorSpring.Value > 89f)
			{
				this.currentState = GTDoor.DoorState.Open;
			}
			break;
		case GTDoor.DoorState.HeldOpen:
			if (!this.peopleInHoldOpenVolume)
			{
				if (!PhotonNetwork.InRoom)
				{
					this.CloseDoor();
				}
				else if (base.photonView.IsMine)
				{
					base.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[]
					{
						GTDoor.DoorState.Closing
					});
					this.currentState = GTDoor.DoorState.ClosingWaitingOnRPC;
				}
			}
			break;
		case GTDoor.DoorState.HeldOpenLocally:
			if (!this.peopleInHoldOpenVolume)
			{
				this.CloseDoor();
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!PhotonNetwork.InRoom)
		{
			GTDoor.DoorState doorState = this.currentState;
			if (doorState == GTDoor.DoorState.ClosingWaitingOnRPC)
			{
				this.CloseDoor();
				return;
			}
			if (doorState != GTDoor.DoorState.OpeningWaitingOnRPC)
			{
				return;
			}
			this.OpenDoor();
		}
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x0000CD40 File Offset: 0x0000AF40
	private void OpenDoor()
	{
		switch (this.currentState)
		{
		case GTDoor.DoorState.Closed:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
			this.ResetDoorOpenedTime();
			this.audioSource.PlayOneShot(this.openSound);
			this.currentState = GTDoor.DoorState.Opening;
			return;
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Closing:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.Opening:
		case GTDoor.DoorState.HeldOpen:
		case GTDoor.DoorState.HeldOpenLocally:
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x0000CDA4 File Offset: 0x0000AFA4
	private void CloseDoor()
	{
		switch (this.currentState)
		{
		case GTDoor.DoorState.Closed:
		case GTDoor.DoorState.Closing:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
		case GTDoor.DoorState.Opening:
			return;
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.HeldOpen:
		case GTDoor.DoorState.HeldOpenLocally:
			this.audioSource.PlayOneShot(this.closeSound);
			this.currentState = GTDoor.DoorState.Closing;
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x0000CE00 File Offset: 0x0000B000
	private void UpdateDoorAnimation()
	{
		switch (this.currentState)
		{
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.Opening:
		case GTDoor.DoorState.HeldOpen:
		case GTDoor.DoorState.HeldOpenLocally:
			this.doorSpring.TrackDampingRatio(90f, 3.1415927f * this.doorOpenSpeed, 1f, Time.deltaTime);
			this.doorTransform.localRotation = Quaternion.Euler(new Vector3(0f, this.doorSpring.Value, 0f));
			return;
		}
		this.doorSpring.TrackDampingRatio(0f, 3.1415927f * this.doorCloseSpeed, 1f, Time.deltaTime);
		this.doorTransform.localRotation = Quaternion.Euler(new Vector3(0f, this.doorSpring.Value, 0f));
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x0000CEDF File Offset: 0x0000B0DF
	public void ResetDoorOpenedTime()
	{
		this.tLastOpened = Time.time;
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x0000CEEC File Offset: 0x0000B0EC
	[PunRPC]
	public void ChangeDoorState(GTDoor.DoorState shouldOpenState)
	{
		switch (shouldOpenState)
		{
		case GTDoor.DoorState.Closed:
		case GTDoor.DoorState.ClosingWaitingOnRPC:
		case GTDoor.DoorState.Open:
		case GTDoor.DoorState.OpeningWaitingOnRPC:
		case GTDoor.DoorState.HeldOpenLocally:
			break;
		case GTDoor.DoorState.Closing:
			switch (this.currentState)
			{
			case GTDoor.DoorState.Closed:
			case GTDoor.DoorState.Closing:
			case GTDoor.DoorState.OpeningWaitingOnRPC:
			case GTDoor.DoorState.Opening:
			case GTDoor.DoorState.HeldOpenLocally:
				break;
			case GTDoor.DoorState.ClosingWaitingOnRPC:
			case GTDoor.DoorState.Open:
			case GTDoor.DoorState.HeldOpen:
				this.CloseDoor();
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
			break;
		case GTDoor.DoorState.Opening:
			switch (this.currentState)
			{
			case GTDoor.DoorState.Closed:
			case GTDoor.DoorState.OpeningWaitingOnRPC:
				this.OpenDoor();
				return;
			case GTDoor.DoorState.ClosingWaitingOnRPC:
			case GTDoor.DoorState.Closing:
			case GTDoor.DoorState.Open:
			case GTDoor.DoorState.Opening:
			case GTDoor.DoorState.HeldOpen:
			case GTDoor.DoorState.HeldOpenLocally:
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			break;
		case GTDoor.DoorState.HeldOpen:
			switch (this.currentState)
			{
			case GTDoor.DoorState.Closed:
			case GTDoor.DoorState.ClosingWaitingOnRPC:
			case GTDoor.DoorState.OpeningWaitingOnRPC:
			case GTDoor.DoorState.Opening:
			case GTDoor.DoorState.HeldOpen:
				break;
			case GTDoor.DoorState.Closing:
				this.audioSource.PlayOneShot(this.openSound);
				this.currentState = GTDoor.DoorState.HeldOpen;
				return;
			case GTDoor.DoorState.Open:
			case GTDoor.DoorState.HeldOpenLocally:
				this.currentState = GTDoor.DoorState.HeldOpen;
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
			break;
		default:
			throw new ArgumentOutOfRangeException("shouldOpenState", shouldOpenState, null);
		}
	}

	// Token: 0x04000288 RID: 648
	[SerializeField]
	private Transform doorTransform;

	// Token: 0x04000289 RID: 649
	[SerializeField]
	private Collider[] doorColliders;

	// Token: 0x0400028A RID: 650
	[SerializeField]
	private GTDoorTrigger[] doorButtonTriggers;

	// Token: 0x0400028B RID: 651
	[SerializeField]
	private GTDoorTrigger[] doorHoldOpenTriggers;

	// Token: 0x0400028C RID: 652
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400028D RID: 653
	[SerializeField]
	private AudioClip openSound;

	// Token: 0x0400028E RID: 654
	[SerializeField]
	private AudioClip closeSound;

	// Token: 0x0400028F RID: 655
	[SerializeField]
	private float doorOpenSpeed = 1f;

	// Token: 0x04000290 RID: 656
	[SerializeField]
	private float doorCloseSpeed = 1f;

	// Token: 0x04000291 RID: 657
	[SerializeField]
	[Range(1.5f, 10f)]
	private float timeUntilDoorCloses = 3f;

	// Token: 0x04000292 RID: 658
	[DebugOption]
	private GTDoor.DoorState currentState;

	// Token: 0x04000293 RID: 659
	[DebugOption]
	private bool beingHeldOpen;

	// Token: 0x04000294 RID: 660
	private float tLastOpened;

	// Token: 0x04000295 RID: 661
	private FloatSpring doorSpring;

	// Token: 0x04000296 RID: 662
	private float doorClosingWarningAdvanceNotice = 1f;

	// Token: 0x04000297 RID: 663
	[DebugOption]
	private bool peopleInHoldOpenVolume;

	// Token: 0x04000298 RID: 664
	[DebugOption]
	private bool buttonTriggeredThisFrame;

	// Token: 0x0200039E RID: 926
	public enum DoorState
	{
		// Token: 0x04001B4F RID: 6991
		Closed,
		// Token: 0x04001B50 RID: 6992
		ClosingWaitingOnRPC,
		// Token: 0x04001B51 RID: 6993
		Closing,
		// Token: 0x04001B52 RID: 6994
		Open,
		// Token: 0x04001B53 RID: 6995
		OpeningWaitingOnRPC,
		// Token: 0x04001B54 RID: 6996
		Opening,
		// Token: 0x04001B55 RID: 6997
		HeldOpen,
		// Token: 0x04001B56 RID: 6998
		HeldOpenLocally
	}
}
