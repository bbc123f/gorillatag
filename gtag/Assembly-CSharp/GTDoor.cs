using System;
using BoingKit;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

public class GTDoor : MonoBehaviourPun
{
	private void Start()
	{
		Collider[] array = this.doorColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		this.tLastOpened = 0f;
	}

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

	public void ResetDoorOpenedTime()
	{
		this.tLastOpened = Time.time;
	}

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

	public GTDoor()
	{
	}

	[SerializeField]
	private Transform doorTransform;

	[SerializeField]
	private Collider[] doorColliders;

	[SerializeField]
	private GTDoorTrigger[] doorButtonTriggers;

	[SerializeField]
	private GTDoorTrigger[] doorHoldOpenTriggers;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioClip openSound;

	[SerializeField]
	private AudioClip closeSound;

	[SerializeField]
	private float doorOpenSpeed = 1f;

	[SerializeField]
	private float doorCloseSpeed = 1f;

	[SerializeField]
	[Range(1.5f, 10f)]
	private float timeUntilDoorCloses = 3f;

	[DebugOption]
	private GTDoor.DoorState currentState;

	[DebugOption]
	private bool beingHeldOpen;

	private float tLastOpened;

	private FloatSpring doorSpring;

	private float doorClosingWarningAdvanceNotice = 1f;

	[DebugOption]
	private bool peopleInHoldOpenVolume;

	[DebugOption]
	private bool buttonTriggeredThisFrame;

	public enum DoorState
	{
		Closed,
		ClosingWaitingOnRPC,
		Closing,
		Open,
		OpeningWaitingOnRPC,
		Opening,
		HeldOpen,
		HeldOpenLocally
	}
}
