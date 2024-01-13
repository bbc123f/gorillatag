using System;
using BoingKit;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

public class GTDoor : MonoBehaviourPun
{
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
	private DoorState currentState;

	[DebugOption]
	private bool beingHeldOpen;

	private float tLastOpened;

	private FloatSpring doorSpring;

	private float doorClosingWarningAdvanceNotice = 1f;

	[DebugOption]
	private bool peopleInHoldOpenVolume;

	[DebugOption]
	private bool buttonTriggeredThisFrame;

	private void Start()
	{
		Collider[] array = doorColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		tLastOpened = 0f;
	}

	private void Update()
	{
		UpdateDoorState();
		UpdateDoorAnimation();
		if (currentState == DoorState.Closed)
		{
			Collider[] array = doorColliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}
		else
		{
			Collider[] array = doorColliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
		}
	}

	private void UpdateDoorState()
	{
		peopleInHoldOpenVolume = false;
		GTDoorTrigger[] array = doorHoldOpenTriggers;
		foreach (GTDoorTrigger obj in array)
		{
			obj.ValidateOverlappingColliders();
			if (obj.overlapCount > 0)
			{
				peopleInHoldOpenVolume = true;
				break;
			}
		}
		buttonTriggeredThisFrame = false;
		array = doorButtonTriggers;
		foreach (GTDoorTrigger gTDoorTrigger in array)
		{
			buttonTriggeredThisFrame = buttonTriggeredThisFrame || gTDoorTrigger.TriggeredThisFrame;
		}
		switch (currentState)
		{
		case DoorState.Open:
			if (!(Time.time - tLastOpened > timeUntilDoorCloses))
			{
				break;
			}
			if (peopleInHoldOpenVolume)
			{
				if (PhotonNetwork.InRoom && base.photonView.IsMine)
				{
					base.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, DoorState.HeldOpen);
				}
				currentState = DoorState.HeldOpenLocally;
			}
			else if (!PhotonNetwork.InRoom)
			{
				CloseDoor();
			}
			else if (base.photonView.IsMine)
			{
				base.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, DoorState.Closing);
				currentState = DoorState.ClosingWaitingOnRPC;
			}
			break;
		case DoorState.Closing:
			if (doorSpring.Value < 1f)
			{
				currentState = DoorState.Closed;
			}
			if (peopleInHoldOpenVolume)
			{
				if (PhotonNetwork.InRoom && base.photonView.IsMine)
				{
					base.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, DoorState.HeldOpen);
				}
				audioSource.PlayOneShot(openSound);
				currentState = DoorState.HeldOpenLocally;
			}
			break;
		case DoorState.Opening:
			if (doorSpring.Value > 89f)
			{
				currentState = DoorState.Open;
			}
			break;
		case DoorState.HeldOpen:
			if (!peopleInHoldOpenVolume)
			{
				if (!PhotonNetwork.InRoom)
				{
					CloseDoor();
				}
				else if (base.photonView.IsMine)
				{
					base.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, DoorState.Closing);
					currentState = DoorState.ClosingWaitingOnRPC;
				}
			}
			break;
		case DoorState.HeldOpenLocally:
			if (!peopleInHoldOpenVolume)
			{
				CloseDoor();
			}
			break;
		case DoorState.Closed:
			if (buttonTriggeredThisFrame)
			{
				if (!PhotonNetwork.InRoom)
				{
					OpenDoor();
					break;
				}
				base.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, DoorState.Opening);
				currentState = DoorState.OpeningWaitingOnRPC;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case DoorState.ClosingWaitingOnRPC:
		case DoorState.OpeningWaitingOnRPC:
			break;
		}
		if (!PhotonNetwork.InRoom)
		{
			switch (currentState)
			{
			case DoorState.ClosingWaitingOnRPC:
				CloseDoor();
				break;
			case DoorState.OpeningWaitingOnRPC:
				OpenDoor();
				break;
			}
		}
	}

	private void OpenDoor()
	{
		switch (currentState)
		{
		case DoorState.Closed:
		case DoorState.OpeningWaitingOnRPC:
			ResetDoorOpenedTime();
			audioSource.PlayOneShot(openSound);
			currentState = DoorState.Opening;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case DoorState.ClosingWaitingOnRPC:
		case DoorState.Closing:
		case DoorState.Open:
		case DoorState.Opening:
		case DoorState.HeldOpen:
		case DoorState.HeldOpenLocally:
			break;
		}
	}

	private void CloseDoor()
	{
		switch (currentState)
		{
		case DoorState.ClosingWaitingOnRPC:
		case DoorState.Open:
		case DoorState.HeldOpen:
		case DoorState.HeldOpenLocally:
			audioSource.PlayOneShot(closeSound);
			currentState = DoorState.Closing;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case DoorState.Closed:
		case DoorState.Closing:
		case DoorState.OpeningWaitingOnRPC:
		case DoorState.Opening:
			break;
		}
	}

	private void UpdateDoorAnimation()
	{
		switch (currentState)
		{
		case DoorState.ClosingWaitingOnRPC:
		case DoorState.Open:
		case DoorState.Opening:
		case DoorState.HeldOpen:
		case DoorState.HeldOpenLocally:
			doorSpring.TrackDampingRatio(90f, (float)Math.PI * doorOpenSpeed, 1f, Time.deltaTime);
			doorTransform.localRotation = Quaternion.Euler(new Vector3(0f, doorSpring.Value, 0f));
			break;
		default:
			doorSpring.TrackDampingRatio(0f, (float)Math.PI * doorCloseSpeed, 1f, Time.deltaTime);
			doorTransform.localRotation = Quaternion.Euler(new Vector3(0f, doorSpring.Value, 0f));
			break;
		}
	}

	public void ResetDoorOpenedTime()
	{
		tLastOpened = Time.time;
	}

	[PunRPC]
	public void ChangeDoorState(DoorState shouldOpenState)
	{
		switch (shouldOpenState)
		{
		case DoorState.HeldOpen:
			switch (currentState)
			{
			case DoorState.Open:
			case DoorState.HeldOpenLocally:
				currentState = DoorState.HeldOpen;
				break;
			case DoorState.Closing:
				audioSource.PlayOneShot(openSound);
				currentState = DoorState.HeldOpen;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case DoorState.Closed:
			case DoorState.ClosingWaitingOnRPC:
			case DoorState.OpeningWaitingOnRPC:
			case DoorState.Opening:
			case DoorState.HeldOpen:
				break;
			}
			break;
		case DoorState.Closing:
			switch (currentState)
			{
			case DoorState.ClosingWaitingOnRPC:
			case DoorState.Open:
			case DoorState.HeldOpen:
				CloseDoor();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case DoorState.Closed:
			case DoorState.Closing:
			case DoorState.OpeningWaitingOnRPC:
			case DoorState.Opening:
			case DoorState.HeldOpenLocally:
				break;
			}
			break;
		case DoorState.Opening:
			switch (currentState)
			{
			case DoorState.Closed:
			case DoorState.OpeningWaitingOnRPC:
				OpenDoor();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case DoorState.ClosingWaitingOnRPC:
			case DoorState.Closing:
			case DoorState.Open:
			case DoorState.Opening:
			case DoorState.HeldOpen:
			case DoorState.HeldOpenLocally:
				break;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException("shouldOpenState", shouldOpenState, null);
		case DoorState.Closed:
		case DoorState.ClosingWaitingOnRPC:
		case DoorState.Open:
		case DoorState.OpeningWaitingOnRPC:
		case DoorState.HeldOpenLocally:
			break;
		}
	}
}
