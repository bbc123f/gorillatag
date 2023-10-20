using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x020001BE RID: 446
public class GorillaFireballControllerManager : MonoBehaviour
{
	// Token: 0x06000B5F RID: 2911 RVA: 0x00045C50 File Offset: 0x00043E50
	private void Update()
	{
		if (!this.hasInitialized)
		{
			this.hasInitialized = true;
			List<InputDevice> list = new List<InputDevice>();
			List<InputDevice> list2 = new List<InputDevice>();
			InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, list);
			InputDevices.GetDevicesAtXRNode(XRNode.RightHand, list2);
			if (list.Count == 1)
			{
				this.leftHand = list[0];
			}
			if (list2.Count == 1)
			{
				this.rightHand = list2[0];
			}
		}
		float axis = SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand);
		if (this.leftHandLastState <= this.throwingThreshold && axis > this.throwingThreshold)
		{
			this.CreateFireball(true);
		}
		else if (this.leftHandLastState >= this.throwingThreshold && axis < this.throwingThreshold)
		{
			this.TryThrowFireball(true);
		}
		this.leftHandLastState = axis;
		axis = SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand);
		if (this.rightHandLastState <= this.throwingThreshold && axis > this.throwingThreshold)
		{
			this.CreateFireball(false);
		}
		else if (this.rightHandLastState >= this.throwingThreshold && axis < this.throwingThreshold)
		{
			this.TryThrowFireball(false);
		}
		this.rightHandLastState = axis;
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x00045D54 File Offset: 0x00043F54
	public void TryThrowFireball(bool isLeftHand)
	{
		if (isLeftHand && GorillaPlaySpace.Instance.myVRRig.leftHandTransform.GetComponentInChildren<GorillaFireball>() != null)
		{
			GorillaPlaySpace.Instance.myVRRig.leftHandTransform.GetComponentInChildren<GorillaFireball>().ThrowThisThingo();
			return;
		}
		if (!isLeftHand && GorillaPlaySpace.Instance.myVRRig.rightHandTransform.GetComponentInChildren<GorillaFireball>() != null)
		{
			GorillaPlaySpace.Instance.myVRRig.rightHandTransform.GetComponentInChildren<GorillaFireball>().ThrowThisThingo();
		}
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x00045DD4 File Offset: 0x00043FD4
	public void CreateFireball(bool isLeftHand)
	{
		object[] array = new object[1];
		Vector3 position;
		if (isLeftHand)
		{
			array[0] = true;
			position = GorillaPlaySpace.Instance.myVRRig.leftHandTransform.position;
		}
		else
		{
			array[0] = false;
			position = GorillaPlaySpace.Instance.myVRRig.rightHandTransform.position;
		}
		PhotonNetwork.Instantiate("GorillaPrefabs/GorillaFireball", position, Quaternion.identity, 0, array);
	}

	// Token: 0x04000EB9 RID: 3769
	public InputDevice leftHand;

	// Token: 0x04000EBA RID: 3770
	public InputDevice rightHand;

	// Token: 0x04000EBB RID: 3771
	public bool hasInitialized;

	// Token: 0x04000EBC RID: 3772
	public float leftHandLastState;

	// Token: 0x04000EBD RID: 3773
	public float rightHandLastState;

	// Token: 0x04000EBE RID: 3774
	public float throwingThreshold = 0.9f;
}
