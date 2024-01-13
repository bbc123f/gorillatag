using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking;

public class GorillaKeyboardButton : GorillaTriggerBox
{
	public string characterString;

	public GorillaComputer computer;

	public float pressTime;

	public bool functionKey;

	public bool testClick;

	public bool repeatTestClick;

	public float repeatCooldown = 2f;

	private float lastTestClick;

	private void Start()
	{
		pressTime = 0f;
		computer = GorillaComputer.instance;
	}

	public void Update()
	{
		if (testClick)
		{
			testClick = false;
			computer.PressButton(this);
		}
		if (repeatTestClick && lastTestClick + repeatCooldown < Time.time)
		{
			lastTestClick = Time.time;
			testClick = true;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		Debug.Log("collision detected" + collider, collider);
		if (!(collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null))
		{
			return;
		}
		GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
		Debug.Log("buttan press");
		computer.PressButton(this);
		if (component != null)
		{
			GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, component.isLeftHand, 0.1f);
			if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
			{
				GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.Others, 66, component.isLeftHand, 0.1f);
			}
		}
	}
}
