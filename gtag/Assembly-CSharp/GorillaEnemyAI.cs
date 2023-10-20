using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001BB RID: 443
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class GorillaEnemyAI : MonoBehaviourPun, IPunObservable, IInRoomCallbacks
{
	// Token: 0x06000B4A RID: 2890 RVA: 0x00045824 File Offset: 0x00043A24
	private void Start()
	{
		this.agent = base.GetComponent<NavMeshAgent>();
		this.r = base.GetComponent<Rigidbody>();
		this.r.useGravity = true;
		if (!base.photonView.IsMine)
		{
			this.agent.enabled = false;
			this.r.isKinematic = true;
		}
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x0004587C File Offset: 0x00043A7C
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.eulerAngles);
			return;
		}
		this.targetPosition = (Vector3)stream.ReceiveNext();
		this.targetRotation = (Vector3)stream.ReceiveNext();
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x000458E0 File Offset: 0x00043AE0
	private void Update()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.FindClosestPlayer();
			if (this.playerTransform != null)
			{
				this.agent.destination = this.playerTransform.position;
			}
			base.transform.LookAt(new Vector3(this.playerTransform.transform.position.x, base.transform.position.y, this.playerTransform.position.z));
			this.r.velocity *= 0.99f;
			return;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, this.lerpValue);
		base.transform.eulerAngles = Vector3.Lerp(base.transform.eulerAngles, this.targetRotation, this.lerpValue);
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x000459D0 File Offset: 0x00043BD0
	private void FindClosestPlayer()
	{
		VRRig[] array = Object.FindObjectsOfType<VRRig>();
		VRRig vrrig = null;
		float num = 100000f;
		foreach (VRRig vrrig2 in array)
		{
			Vector3 vector = vrrig2.transform.position - base.transform.position;
			if (vector.magnitude < num)
			{
				vrrig = vrrig2;
				num = vector.magnitude;
			}
		}
		this.playerTransform = vrrig.transform;
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x00045A41 File Offset: 0x00043C41
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == 19)
		{
			PhotonNetwork.Destroy(base.photonView);
		}
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x00045A5D File Offset: 0x00043C5D
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.agent.enabled = true;
			this.r.isKinematic = false;
		}
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x00045A7E File Offset: 0x00043C7E
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x00045A80 File Offset: 0x00043C80
	void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x00045A82 File Offset: 0x00043C82
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x00045A84 File Offset: 0x00043C84
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x04000EAE RID: 3758
	public Transform playerTransform;

	// Token: 0x04000EAF RID: 3759
	private NavMeshAgent agent;

	// Token: 0x04000EB0 RID: 3760
	private Rigidbody r;

	// Token: 0x04000EB1 RID: 3761
	private Vector3 targetPosition;

	// Token: 0x04000EB2 RID: 3762
	private Vector3 targetRotation;

	// Token: 0x04000EB3 RID: 3763
	public float lerpValue;
}
