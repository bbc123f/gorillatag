using System.Diagnostics;
using Photon.Pun;
using UnityEngine;

public class Tappable : MonoBehaviour
{
	public int tappableId;

	public string staticId;

	public bool useStaticId;

	[Space]
	public TappableManager manager;

	public void Validate()
	{
		TappableManager.CalculateId(this, force: true);
	}

	protected virtual void OnEnable()
	{
		TappableManager.Register(this);
	}

	protected virtual void OnDisable()
	{
		TappableManager.Unregister(this);
	}

	public void OnTap(float tapStrength, float tapTime)
	{
		OnTapLocal(tapStrength, tapTime);
		if (PhotonNetwork.InRoom && (bool)manager)
		{
			manager.photonView.RPC("SendOnTapRPC", RpcTarget.Others, tappableId, tapStrength);
		}
	}

	public virtual void OnTapLocal(float tapStrength, float tapTime)
	{
	}

	private void RecalculateId()
	{
		TappableManager.CalculateId(this, force: true);
	}

	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		TappableManager.CalculateId(this);
	}
}
