using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	public class GorillaKeyboardButton : GorillaTriggerBox
	{
		private void Start()
		{
			this.pressTime = 0f;
			this.computer = GorillaComputer.instance;
		}

		private void OnTriggerEnter(Collider collider)
		{
			Debug.Log("collision detected" + ((collider != null) ? collider.ToString() : null), collider);
			if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
			{
				GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
				Debug.Log("buttan press");
				this.computer.PressButton(this);
				if (component != null)
				{
					GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
					GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, component.isLeftHand, 0.1f);
					if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
					{
						GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.Others, new object[]
						{
							66,
							component.isLeftHand,
							0.1f
						});
					}
				}
			}
		}

		public string characterString;

		public GorillaComputer computer;

		public float pressTime;

		public bool functionKey;

		public bool testClick;

		public bool repeatTestClick;

		public float repeatCooldown = 2f;

		private float lastTestClick;
	}
}
