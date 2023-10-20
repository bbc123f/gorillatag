using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001B2 RID: 434
public class GorillaPlayerLineButton : MonoBehaviour
{
	// Token: 0x06000B0E RID: 2830 RVA: 0x00044471 File Offset: 0x00042671
	private void OnEnable()
	{
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x00044487 File Offset: 0x00042687
	private void OnDisable()
	{
		if (Application.isEditor)
		{
			base.StopAllCoroutines();
		}
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x00044496 File Offset: 0x00042696
	private IEnumerator TestPressCheck()
	{
		for (;;)
		{
			if (this.testPress)
			{
				this.testPress = false;
				if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute)
				{
					this.isOn = !this.isOn;
				}
				this.parentLine.PressButton(this.isOn, this.buttonType);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x000444A8 File Offset: 0x000426A8
	private void OnTriggerEnter(Collider collider)
	{
		if (base.enabled && this.touchTime + this.debounceTime < Time.time && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.touchTime = Time.time;
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute)
			{
				this.isOn = !this.isOn;
			}
			if (this.buttonType == GorillaPlayerLineButton.ButtonType.Mute || this.buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech || this.buttonType == GorillaPlayerLineButton.ButtonType.Cheating || this.buttonType == GorillaPlayerLineButton.ButtonType.Cancel || this.parentLine.canPressNextReportButton)
			{
				this.parentLine.PressButton(this.isOn, this.buttonType);
				if (component != null)
				{
					GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
					GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, component.isLeftHand, 0.05f);
					if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
					{
						GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.Others, new object[]
						{
							67,
							component.isLeftHand,
							0.05f
						});
					}
				}
			}
		}
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x00044607 File Offset: 0x00042807
	private void OnTriggerExit(Collider other)
	{
		if (this.buttonType != GorillaPlayerLineButton.ButtonType.Mute && other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.parentLine.canPressNextReportButton = true;
		}
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x0004462C File Offset: 0x0004282C
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			this.myText.text = this.onText;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
		this.myText.text = this.offText;
	}

	// Token: 0x04000E49 RID: 3657
	public GorillaPlayerScoreboardLine parentLine;

	// Token: 0x04000E4A RID: 3658
	public GorillaPlayerLineButton.ButtonType buttonType;

	// Token: 0x04000E4B RID: 3659
	public bool isOn;

	// Token: 0x04000E4C RID: 3660
	public Material offMaterial;

	// Token: 0x04000E4D RID: 3661
	public Material onMaterial;

	// Token: 0x04000E4E RID: 3662
	public string offText;

	// Token: 0x04000E4F RID: 3663
	public string onText;

	// Token: 0x04000E50 RID: 3664
	public Text myText;

	// Token: 0x04000E51 RID: 3665
	public float debounceTime = 0.25f;

	// Token: 0x04000E52 RID: 3666
	public float touchTime;

	// Token: 0x04000E53 RID: 3667
	public bool testPress;

	// Token: 0x02000451 RID: 1105
	public enum ButtonType
	{
		// Token: 0x04001DE2 RID: 7650
		HateSpeech,
		// Token: 0x04001DE3 RID: 7651
		Cheating,
		// Token: 0x04001DE4 RID: 7652
		Toxicity,
		// Token: 0x04001DE5 RID: 7653
		Mute,
		// Token: 0x04001DE6 RID: 7654
		Report,
		// Token: 0x04001DE7 RID: 7655
		Cancel
	}
}
