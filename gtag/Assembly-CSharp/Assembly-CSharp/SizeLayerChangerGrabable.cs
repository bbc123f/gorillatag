using System;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using UnityEngine;

public class SizeLayerChangerGrabable : MonoBehaviour, IGorillaGrabable
{
	void IGorillaGrabable.OnGrabbed()
	{
		if (this.grabChangesSizeLayer)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer);
			rigContainer.Rig.sizeManager.currentSizeLayerMaskValue = this.grabbedSizeLayerMask.Mask;
		}
	}

	void IGorillaGrabable.OnGrabReleased()
	{
		if (this.releaseChangesSizeLayer)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer);
			rigContainer.Rig.sizeManager.currentSizeLayerMaskValue = this.releasedSizeLayerMask.Mask;
		}
	}

	[SerializeField]
	private bool grabChangesSizeLayer = true;

	[SerializeField]
	private bool releaseChangesSizeLayer = true;

	[SerializeField]
	private SizeLayerMask grabbedSizeLayerMask;

	[SerializeField]
	private SizeLayerMask releasedSizeLayerMask;
}
