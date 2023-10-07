using System;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000131 RID: 305
public class RubberDuckEvents : MonoBehaviour
{
	// Token: 0x060007FA RID: 2042 RVA: 0x00032738 File Offset: 0x00030938
	public void Init(Player player)
	{
		string text = player.UserId;
		if (string.IsNullOrEmpty(text))
		{
			bool flag = player == PhotonNetwork.LocalPlayer;
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			if (flag && instance != null)
			{
				text = instance._playFabPlayerIdCache;
			}
			else
			{
				text = player.NickName;
			}
		}
		this.PlayerIdString = text + "." + base.gameObject.name;
		this.PlayerId = this.PlayerIdString.GetStaticHash();
		this.Dispose();
		this.Activate = new PhotonEvent(string.Format("{0}.{1}", this.PlayerId, "Activate"));
		this.Deactivate = new PhotonEvent(string.Format("{0}.{1}", this.PlayerId, "Deactivate"));
		this.Activate.reliable = false;
		this.Deactivate.reliable = false;
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x00032814 File Offset: 0x00030A14
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

	// Token: 0x060007FC RID: 2044 RVA: 0x00032837 File Offset: 0x00030A37
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

	// Token: 0x060007FD RID: 2045 RVA: 0x0003285A File Offset: 0x00030A5A
	private void OnDestroy()
	{
		this.Dispose();
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x00032862 File Offset: 0x00030A62
	private void Dispose()
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

	// Token: 0x040009A3 RID: 2467
	public int PlayerId;

	// Token: 0x040009A4 RID: 2468
	public string PlayerIdString;

	// Token: 0x040009A5 RID: 2469
	public PhotonEvent Activate;

	// Token: 0x040009A6 RID: 2470
	public PhotonEvent Deactivate;
}
