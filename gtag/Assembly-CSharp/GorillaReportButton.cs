using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000D5 RID: 213
public class GorillaReportButton : MonoBehaviour
{
	// Token: 0x060004BE RID: 1214 RVA: 0x0001E54D File Offset: 0x0001C74D
	public void AssignParentLine(GorillaPlayerScoreboardLine parent)
	{
		this.parentLine = parent;
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0001E558 File Offset: 0x0001C758
	private void OnTriggerEnter(Collider collider)
	{
		if (base.enabled && this.touchTime + this.debounceTime < Time.time)
		{
			this.isOn = !this.isOn;
			this.UpdateColor();
			this.selected = !this.selected;
			this.touchTime = Time.time;
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, false, 0.05f);
			if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
			{
				PhotonView.Get(GorillaTagger.Instance.myVRRig).RPC("PlayHandTap", RpcTarget.Others, new object[]
				{
					67,
					false,
					0.05f
				});
			}
		}
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0001E64B File Offset: 0x0001C84B
	private void OnTriggerExit(Collider other)
	{
		if (this.metaReportType != GorillaReportButton.MetaReportReason.Cancel)
		{
			other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null;
		}
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x0001E663 File Offset: 0x0001C863
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
	}

	// Token: 0x04000579 RID: 1401
	public GorillaReportButton.MetaReportReason metaReportType;

	// Token: 0x0400057A RID: 1402
	public GorillaPlayerLineButton.ButtonType buttonType;

	// Token: 0x0400057B RID: 1403
	public GorillaPlayerScoreboardLine parentLine;

	// Token: 0x0400057C RID: 1404
	public bool isOn;

	// Token: 0x0400057D RID: 1405
	public Material offMaterial;

	// Token: 0x0400057E RID: 1406
	public Material onMaterial;

	// Token: 0x0400057F RID: 1407
	public string offText;

	// Token: 0x04000580 RID: 1408
	public string onText;

	// Token: 0x04000581 RID: 1409
	public Text myText;

	// Token: 0x04000582 RID: 1410
	public float debounceTime = 0.25f;

	// Token: 0x04000583 RID: 1411
	public float touchTime;

	// Token: 0x04000584 RID: 1412
	public bool testPress;

	// Token: 0x04000585 RID: 1413
	public bool selected;

	// Token: 0x020003E2 RID: 994
	[SerializeField]
	public enum MetaReportReason
	{
		// Token: 0x04001C4C RID: 7244
		HateSpeech,
		// Token: 0x04001C4D RID: 7245
		Cheating,
		// Token: 0x04001C4E RID: 7246
		Toxicity,
		// Token: 0x04001C4F RID: 7247
		Bullying,
		// Token: 0x04001C50 RID: 7248
		Doxing,
		// Token: 0x04001C51 RID: 7249
		Impersonation,
		// Token: 0x04001C52 RID: 7250
		Submit,
		// Token: 0x04001C53 RID: 7251
		Cancel
	}
}
