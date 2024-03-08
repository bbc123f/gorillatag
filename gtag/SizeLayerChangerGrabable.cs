using System;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using UnityEngine;

public class SizeLayerChangerGrabable : MonoBehaviour, IGorillaGrabable
{
	Transform IGorillaGrabable.OnGrabbed(GorillaGrabber g)
	{
		if (this.grabChangesSizeLayer)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer);
			rigContainer.Rig.sizeManager.currentSizeLayerMaskValue = this.grabbedSizeLayerMask.Mask;
		}
		return base.transform;
	}

	Transform IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
	{
		if (this.releaseChangesSizeLayer)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer);
			rigContainer.Rig.sizeManager.currentSizeLayerMaskValue = this.releasedSizeLayerMask.Mask;
		}
		return base.transform;
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
