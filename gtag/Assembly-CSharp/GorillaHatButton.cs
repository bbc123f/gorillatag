using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000176 RID: 374
public class GorillaHatButton : MonoBehaviour
{
	// Token: 0x06000966 RID: 2406 RVA: 0x00038B84 File Offset: 0x00036D84
	public void Update()
	{
		if (this.testPress)
		{
			this.testPress = false;
			if (this.touchTime + this.debounceTime < Time.time)
			{
				this.touchTime = Time.time;
				this.isOn = !this.isOn;
				this.buttonParent.PressButton(this.isOn, this.buttonType, this.cosmeticName);
			}
		}
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x00038BEC File Offset: 0x00036DEC
	private void OnTriggerEnter(Collider collider)
	{
		if (this.touchTime + this.debounceTime < Time.time && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.touchTime = Time.time;
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			this.isOn = !this.isOn;
			this.buttonParent.PressButton(this.isOn, this.buttonType, this.cosmeticName);
			if (component != null)
			{
				GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			}
		}
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x00038C8C File Offset: 0x00036E8C
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

	// Token: 0x04000B7A RID: 2938
	public GorillaHatButtonParent buttonParent;

	// Token: 0x04000B7B RID: 2939
	public GorillaHatButton.HatButtonType buttonType;

	// Token: 0x04000B7C RID: 2940
	public bool isOn;

	// Token: 0x04000B7D RID: 2941
	public Material offMaterial;

	// Token: 0x04000B7E RID: 2942
	public Material onMaterial;

	// Token: 0x04000B7F RID: 2943
	public string offText;

	// Token: 0x04000B80 RID: 2944
	public string onText;

	// Token: 0x04000B81 RID: 2945
	public Text myText;

	// Token: 0x04000B82 RID: 2946
	public float debounceTime = 0.25f;

	// Token: 0x04000B83 RID: 2947
	public float touchTime;

	// Token: 0x04000B84 RID: 2948
	public string cosmeticName;

	// Token: 0x04000B85 RID: 2949
	public bool testPress;

	// Token: 0x02000423 RID: 1059
	public enum HatButtonType
	{
		// Token: 0x04001D25 RID: 7461
		Hat,
		// Token: 0x04001D26 RID: 7462
		Face,
		// Token: 0x04001D27 RID: 7463
		Badge
	}
}
