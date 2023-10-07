using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001B1 RID: 433
public class GorillaPlayerLineButton : MonoBehaviour
{
	// Token: 0x06000B09 RID: 2825 RVA: 0x00044339 File Offset: 0x00042539
	private void OnEnable()
	{
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x0004434F File Offset: 0x0004254F
	private void OnDisable()
	{
		if (Application.isEditor)
		{
			base.StopAllCoroutines();
		}
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0004435E File Offset: 0x0004255E
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

	// Token: 0x06000B0C RID: 2828 RVA: 0x00044370 File Offset: 0x00042570
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

	// Token: 0x06000B0D RID: 2829 RVA: 0x000444CF File Offset: 0x000426CF
	private void OnTriggerExit(Collider other)
	{
		if (this.buttonType != GorillaPlayerLineButton.ButtonType.Mute && other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.parentLine.canPressNextReportButton = true;
		}
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x000444F4 File Offset: 0x000426F4
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

	// Token: 0x04000E45 RID: 3653
	public GorillaPlayerScoreboardLine parentLine;

	// Token: 0x04000E46 RID: 3654
	public GorillaPlayerLineButton.ButtonType buttonType;

	// Token: 0x04000E47 RID: 3655
	public bool isOn;

	// Token: 0x04000E48 RID: 3656
	public Material offMaterial;

	// Token: 0x04000E49 RID: 3657
	public Material onMaterial;

	// Token: 0x04000E4A RID: 3658
	public string offText;

	// Token: 0x04000E4B RID: 3659
	public string onText;

	// Token: 0x04000E4C RID: 3660
	public Text myText;

	// Token: 0x04000E4D RID: 3661
	public float debounceTime = 0.25f;

	// Token: 0x04000E4E RID: 3662
	public float touchTime;

	// Token: 0x04000E4F RID: 3663
	public bool testPress;

	// Token: 0x0200044F RID: 1103
	public enum ButtonType
	{
		// Token: 0x04001DD5 RID: 7637
		HateSpeech,
		// Token: 0x04001DD6 RID: 7638
		Cheating,
		// Token: 0x04001DD7 RID: 7639
		Toxicity,
		// Token: 0x04001DD8 RID: 7640
		Mute,
		// Token: 0x04001DD9 RID: 7641
		Report,
		// Token: 0x04001DDA RID: 7642
		Cancel
	}
}
