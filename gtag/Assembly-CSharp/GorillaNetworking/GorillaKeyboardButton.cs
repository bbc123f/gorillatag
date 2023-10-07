using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020002B3 RID: 691
	public class GorillaKeyboardButton : GorillaTriggerBox
	{
		// Token: 0x06001285 RID: 4741 RVA: 0x0006B829 File Offset: 0x00069A29
		private void Start()
		{
			this.pressTime = 0f;
			this.computer = GorillaComputer.instance;
		}

		// Token: 0x06001286 RID: 4742 RVA: 0x0006B844 File Offset: 0x00069A44
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

		// Token: 0x06001287 RID: 4743 RVA: 0x0006B89C File Offset: 0x00069A9C
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

		// Token: 0x04001575 RID: 5493
		public string characterString;

		// Token: 0x04001576 RID: 5494
		public GorillaComputer computer;

		// Token: 0x04001577 RID: 5495
		public float pressTime;

		// Token: 0x04001578 RID: 5496
		public bool functionKey;

		// Token: 0x04001579 RID: 5497
		public bool testClick;

		// Token: 0x0400157A RID: 5498
		public bool repeatTestClick;

		// Token: 0x0400157B RID: 5499
		public float repeatCooldown = 2f;

		// Token: 0x0400157C RID: 5500
		private float lastTestClick;
	}
}
