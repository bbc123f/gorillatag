using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020002B5 RID: 693
	public class GorillaKeyboardButton : GorillaTriggerBox
	{
		// Token: 0x0600128C RID: 4748 RVA: 0x0006BCF5 File Offset: 0x00069EF5
		private void Start()
		{
			this.pressTime = 0f;
			this.computer = GorillaComputer.instance;
		}

		// Token: 0x0600128D RID: 4749 RVA: 0x0006BD10 File Offset: 0x00069F10
		public void Update()
		{
			if (this.testClick)
			{
				this.testClick = false;
				this.computer.PressButton(this);
			}
			if (this.repeatTestClick && this.lastTestClick + this.repeatCooldown < Time.time)
			{
				this.lastTestClick = Time.time;
				this.testClick = true;
			}
		}

		// Token: 0x0600128E RID: 4750 RVA: 0x0006BD68 File Offset: 0x00069F68
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

		// Token: 0x04001582 RID: 5506
		public string characterString;

		// Token: 0x04001583 RID: 5507
		public GorillaComputer computer;

		// Token: 0x04001584 RID: 5508
		public float pressTime;

		// Token: 0x04001585 RID: 5509
		public bool functionKey;

		// Token: 0x04001586 RID: 5510
		public bool testClick;

		// Token: 0x04001587 RID: 5511
		public bool repeatTestClick;

		// Token: 0x04001588 RID: 5512
		public float repeatCooldown = 2f;

		// Token: 0x04001589 RID: 5513
		private float lastTestClick;
	}
}
