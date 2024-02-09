using System;
using System.Diagnostics;
using Photon.Pun;
using UnityEngine;

public class Tappable : MonoBehaviour
{
	public void Validate()
	{
		TappableManager.CalculateId(this, true);
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
		this.OnTapLocal(tapStrength, tapTime);
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnTapRPC", RpcTarget.Others, new object[] { this.tappableId, tapStrength });
	}

	public virtual void OnTapLocal(float tapStrength, float tapTime)
	{
	}

	private void RecalculateId()
	{
		TappableManager.CalculateId(this, true);
	}

	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		TappableManager.CalculateId(this, false);
	}

	public int tappableId;

	public string staticId;

	public bool useStaticId;

	[Space]
	public TappableManager manager;
}
