using System;
using Photon.Pun;
using UnityEngine;

internal class OwnershipGaurd : MonoBehaviour
{
	private void Start()
	{
		if (this.autoRegisterAll)
		{
			this.photonViews = base.GetComponents<PhotonView>();
		}
		if (this.photonViews == null)
		{
			return;
		}
		OwnershipGaurdHandler.RegisterViews(this.photonViews);
	}

	private void OnDestroy()
	{
		if (this.photonViews == null)
		{
			return;
		}
		OwnershipGaurdHandler.RemoveViews(this.photonViews);
	}

	public OwnershipGaurd()
	{
	}

	[SerializeField]
	private PhotonView[] photonViews;

	[SerializeField]
	private bool autoRegisterAll = true;
}
