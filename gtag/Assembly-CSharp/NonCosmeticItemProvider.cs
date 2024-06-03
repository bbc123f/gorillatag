using System;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

public class NonCosmeticItemProvider : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component != null)
		{
			int healthyFlowersInZoneCount = FlowersManager.Instance.GetHealthyFlowersInZoneCount(this.zone);
			if (this.useCondition && this.itemType == NonCosmeticItemProvider.ItemType.honeycomb && (healthyFlowersInZoneCount < this.conditionThreshold || healthyFlowersInZoneCount < 0))
			{
				return;
			}
			GorillaGameManager.instance.FindVRRigForPlayer(PhotonNetwork.LocalPlayer).RPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
			{
				true,
				component.isLeftHand
			});
		}
	}

	public NonCosmeticItemProvider()
	{
	}

	public GTZone zone;

	[Tooltip("only for honeycomb")]
	public bool useCondition;

	public int conditionThreshold;

	public NonCosmeticItemProvider.ItemType itemType;

	public enum ItemType
	{
		honeycomb
	}
}
