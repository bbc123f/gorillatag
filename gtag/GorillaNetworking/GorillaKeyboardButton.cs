using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	public class GorillaKeyboardButton : GorillaTriggerBox
	{
		private void Start()
		{
			if (this.ButtonRenderer == null)
			{
				this.ButtonRenderer = base.GetComponent<Renderer>();
			}
			this.propBlock = new MaterialPropertyBlock();
			this.pressTime = 0f;
		}

		private void OnTriggerEnter(Collider collider)
		{
			Debug.Log("GorillaKeyboardButton.OnTriggerEnter: collision detected " + ((collider != null) ? collider.ToString() : null), collider);
			if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
			{
				GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
				Debug.Log("buttan press");
				GameEvents.OnGorrillaKeyboardButtonPressedEvent.Invoke(this.Binding);
				this.PressButtonColourUpdate();
				if (component != null)
				{
					GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
					GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, component.isLeftHand, 0.1f);
					if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
					{
						GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.Others, new object[] { 66, component.isLeftHand, 0.1f });
					}
				}
			}
		}

		public void PressButtonColourUpdate()
		{
			this.propBlock.SetColor("_BaseColor", this.ButtonColorSettings.PressedColor);
			this.propBlock.SetColor("_Color", this.ButtonColorSettings.PressedColor);
			this.ButtonRenderer.SetPropertyBlock(this.propBlock);
			this.pressTime = Time.time;
			base.StartCoroutine(this.<PressButtonColourUpdate>g__ButtonColorUpdate_Local|13_0());
		}

		[CompilerGenerated]
		private IEnumerator <PressButtonColourUpdate>g__ButtonColorUpdate_Local|13_0()
		{
			yield return new WaitForSeconds(this.ButtonColorSettings.PressedTime);
			if (this.pressTime != 0f && Time.time > this.ButtonColorSettings.PressedTime + this.pressTime)
			{
				this.propBlock.SetColor("_BaseColor", this.ButtonColorSettings.UnpressedColor);
				this.propBlock.SetColor("_Color", this.ButtonColorSettings.UnpressedColor);
				this.ButtonRenderer.SetPropertyBlock(this.propBlock);
				this.pressTime = 0f;
			}
			yield break;
		}

		public string characterString;

		public GorillaKeyboardBindings Binding;

		public float pressTime;

		public bool functionKey;

		public bool testClick;

		public bool repeatTestClick;

		public float repeatCooldown = 2f;

		public Renderer ButtonRenderer;

		public ButtonColorSettings ButtonColorSettings;

		private float lastTestClick;

		private MaterialPropertyBlock propBlock;
	}
}
