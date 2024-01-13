using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

public class GorillaFriendCollider : MonoBehaviour
{
	public List<string> playerIDsCurrentlyTouching = new List<string>();

	public CapsuleCollider thisCapsule;

	public string[] myAllowedMapsToJoin;

	private readonly Collider[] overlapColliders = new Collider[20];

	private int tagAndBodyLayerMask;

	private float jiggleAmount;

	private Collider otherCollider;

	private GameObject otherColliderGO;

	private VRRig collidingRig;

	private int collisions;

	public void Awake()
	{
		thisCapsule = GetComponent<CapsuleCollider>();
		jiggleAmount = Random.Range(0f, 1f);
		StartCoroutine(UpdatePlayersInSphere());
		tagAndBodyLayerMask = LayerMask.GetMask("Gorilla Tag Collider") | LayerMask.GetMask("Gorilla Body Collider");
	}

	private void AddUserID(in string userID)
	{
		if (!playerIDsCurrentlyTouching.Contains(userID))
		{
			playerIDsCurrentlyTouching.Add(userID);
		}
	}

	private IEnumerator UpdatePlayersInSphere()
	{
		yield return new WaitForSeconds(1f + jiggleAmount);
		WaitForSeconds wait = new WaitForSeconds(1f);
		while (true)
		{
			playerIDsCurrentlyTouching.Clear();
			collisions = Physics.OverlapSphereNonAlloc(base.transform.position, thisCapsule.radius, overlapColliders, tagAndBodyLayerMask);
			collisions = Mathf.Min(collisions, overlapColliders.Length);
			if (collisions > 0)
			{
				for (int i = 0; i < collisions; i++)
				{
					otherCollider = overlapColliders[i];
					if (otherCollider == null)
					{
						continue;
					}
					otherColliderGO = otherCollider.attachedRigidbody.gameObject;
					collidingRig = otherColliderGO.GetComponent<VRRig>();
					if (collidingRig != null && collidingRig.creator != null)
					{
						GorillaFriendCollider gorillaFriendCollider = this;
						string userID = collidingRig.creator.UserId;
						gorillaFriendCollider.AddUserID(in userID);
					}
					else
					{
						if (otherColliderGO.GetComponent<Player>() == null)
						{
							continue;
						}
						GorillaFriendCollider gorillaFriendCollider2 = this;
						string userID = PhotonNetwork.LocalPlayer.UserId;
						gorillaFriendCollider2.AddUserID(in userID);
					}
					overlapColliders[i] = null;
				}
				if (playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId) && GorillaComputer.instance.friendJoinCollider != this)
				{
					GorillaComputer.instance.allowedMapsToJoin = myAllowedMapsToJoin;
					GorillaComputer.instance.friendJoinCollider = this;
					GorillaComputer.instance.UpdateScreen();
				}
				otherCollider = null;
				otherColliderGO = null;
				collidingRig = null;
			}
			yield return wait;
		}
	}
}
