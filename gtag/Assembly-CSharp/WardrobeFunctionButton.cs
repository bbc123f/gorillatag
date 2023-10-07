using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000126 RID: 294
public class WardrobeFunctionButton : GorillaPressableButton
{
	// Token: 0x060007BB RID: 1979 RVA: 0x00031265 File Offset: 0x0002F465
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressWardrobeFunctionButton(this.function);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x0003128C File Offset: 0x0002F48C
	public override void UpdateColor()
	{
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x0003128E File Offset: 0x0002F48E
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04000955 RID: 2389
	public string function;

	// Token: 0x04000956 RID: 2390
	public float buttonFadeTime = 0.25f;
}
