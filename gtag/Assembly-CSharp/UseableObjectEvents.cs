using System;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class UseableObjectEvents : MonoBehaviour
{
	public void Init(Player player)
	{
		bool flag = player == PhotonNetwork.LocalPlayer;
		PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
		string str;
		if (flag && instance != null)
		{
			str = instance._playFabPlayerIdCache;
		}
		else
		{
			str = player.NickName;
		}
		this.PlayerIdString = str + "." + base.gameObject.name;
		this.PlayerId = this.PlayerIdString.GetStaticHash();
		this.DisposeEvents();
		this.Activate = new PhotonEvent(this.PlayerId.ToString() + ".Activate");
		this.Deactivate = new PhotonEvent(this.PlayerId.ToString() + ".Deactivate");
		this.Activate.reliable = false;
		this.Deactivate.reliable = false;
	}

	private void OnEnable()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Enable();
		}
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate == null)
		{
			return;
		}
		deactivate.Enable();
	}

	private void OnDisable()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Disable();
		}
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate == null)
		{
			return;
		}
		deactivate.Disable();
	}

	private void OnDestroy()
	{
		this.DisposeEvents();
	}

	private void DisposeEvents()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Dispose();
		}
		this.Activate = null;
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate != null)
		{
			deactivate.Dispose();
		}
		this.Deactivate = null;
	}

	public UseableObjectEvents()
	{
	}

	[NonSerialized]
	private string PlayerIdString;

	[NonSerialized]
	private int PlayerId;

	public PhotonEvent Activate;

	public PhotonEvent Deactivate;
}
