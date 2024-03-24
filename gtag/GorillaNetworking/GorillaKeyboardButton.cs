using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
			if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
			{
				GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
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

		public GorillaKeyboardButton()
		{
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

		[CompilerGenerated]
		private sealed class <<PressButtonColourUpdate>g__ButtonColorUpdate_Local|13_0>d : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <<PressButtonColourUpdate>g__ButtonColorUpdate_Local|13_0>d(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				GorillaKeyboardButton gorillaKeyboardButton = this;
				if (num == 0)
				{
					this.<>1__state = -1;
					this.<>2__current = new WaitForSeconds(gorillaKeyboardButton.ButtonColorSettings.PressedTime);
					this.<>1__state = 1;
					return true;
				}
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				if (gorillaKeyboardButton.pressTime != 0f && Time.time > gorillaKeyboardButton.ButtonColorSettings.PressedTime + gorillaKeyboardButton.pressTime)
				{
					gorillaKeyboardButton.propBlock.SetColor("_BaseColor", gorillaKeyboardButton.ButtonColorSettings.UnpressedColor);
					gorillaKeyboardButton.propBlock.SetColor("_Color", gorillaKeyboardButton.ButtonColorSettings.UnpressedColor);
					gorillaKeyboardButton.ButtonRenderer.SetPropertyBlock(gorillaKeyboardButton.propBlock);
					gorillaKeyboardButton.pressTime = 0f;
				}
				return false;
			}

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			private int <>1__state;

			private object <>2__current;

			public GorillaKeyboardButton <>4__this;
		}
	}
}
