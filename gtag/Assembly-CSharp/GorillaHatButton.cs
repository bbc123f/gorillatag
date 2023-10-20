using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000177 RID: 375
public class GorillaHatButton : MonoBehaviour
{
	// Token: 0x0600096A RID: 2410 RVA: 0x00038B3C File Offset: 0x00036D3C
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

	// Token: 0x0600096B RID: 2411 RVA: 0x00038BA4 File Offset: 0x00036DA4
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

	// Token: 0x0600096C RID: 2412 RVA: 0x00038C44 File Offset: 0x00036E44
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

	// Token: 0x04000B7E RID: 2942
	public GorillaHatButtonParent buttonParent;

	// Token: 0x04000B7F RID: 2943
	public GorillaHatButton.HatButtonType buttonType;

	// Token: 0x04000B80 RID: 2944
	public bool isOn;

	// Token: 0x04000B81 RID: 2945
	public Material offMaterial;

	// Token: 0x04000B82 RID: 2946
	public Material onMaterial;

	// Token: 0x04000B83 RID: 2947
	public string offText;

	// Token: 0x04000B84 RID: 2948
	public string onText;

	// Token: 0x04000B85 RID: 2949
	public Text myText;

	// Token: 0x04000B86 RID: 2950
	public float debounceTime = 0.25f;

	// Token: 0x04000B87 RID: 2951
	public float touchTime;

	// Token: 0x04000B88 RID: 2952
	public string cosmeticName;

	// Token: 0x04000B89 RID: 2953
	public bool testPress;

	// Token: 0x02000425 RID: 1061
	public enum HatButtonType
	{
		// Token: 0x04001D32 RID: 7474
		Hat,
		// Token: 0x04001D33 RID: 7475
		Face,
		// Token: 0x04001D34 RID: 7476
		Badge
	}
}
