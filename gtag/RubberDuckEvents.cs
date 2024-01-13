using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utilities;

public class RubberDuckEvents : MonoBehaviour
{
	public int PlayerId;

	public string PlayerIdString;

	public PhotonEvent Activate;

	public PhotonEvent Deactivate;

	public void Init(Player player)
	{
		string text = player.UserId;
		if (string.IsNullOrEmpty(text))
		{
			bool num = player == PhotonNetwork.LocalPlayer;
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			text = ((!num || !(instance != null)) ? player.NickName : instance._playFabPlayerIdCache);
		}
		PlayerIdString = text + "." + base.gameObject.name;
		PlayerId = PlayerIdString.GetStaticHash();
		Dispose();
		Activate = new PhotonEvent(string.Format("{0}.{1}", PlayerId, "Activate"));
		Deactivate = new PhotonEvent(string.Format("{0}.{1}", PlayerId, "Deactivate"));
		Activate.reliable = false;
		Deactivate.reliable = false;
	}

	private void OnEnable()
	{
		Activate?.Enable();
		Deactivate?.Enable();
	}

	private void OnDisable()
	{
		Activate?.Disable();
		Deactivate?.Disable();
	}

	private void OnDestroy()
	{
		Dispose();
	}

	private void Dispose()
	{
		Activate?.Dispose();
		Activate = null;
		Deactivate?.Dispose();
		Deactivate = null;
	}
}
