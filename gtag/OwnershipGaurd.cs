using Photon.Pun;
using UnityEngine;

internal class OwnershipGaurd : MonoBehaviour
{
	[SerializeField]
	private PhotonView[] photonViews;

	[SerializeField]
	private bool autoRegisterAll = true;

	private void Start()
	{
		if (autoRegisterAll)
		{
			photonViews = GetComponents<PhotonView>();
		}
		if (photonViews != null)
		{
			OwnershipGaurdHandler.RegisterViews(photonViews);
		}
	}

	private void OnDestroy()
	{
		if (photonViews != null)
		{
			OwnershipGaurdHandler.RemoveViews(photonViews);
		}
	}
}
