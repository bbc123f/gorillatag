using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000FA RID: 250
public class BetaButton : GorillaPressableButton
{
	// Token: 0x0600062B RID: 1579 RVA: 0x00026F18 File Offset: 0x00025118
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.count++;
		base.StartCoroutine(this.ButtonColorUpdate());
		if (this.count >= 10)
		{
			this.betaParent.SetActive(false);
			PlayerPrefs.SetString("CheckedBox2", "true");
			PlayerPrefs.Save();
		}
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x00026F70 File Offset: 0x00025170
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04000782 RID: 1922
	public GameObject betaParent;

	// Token: 0x04000783 RID: 1923
	public int count;

	// Token: 0x04000784 RID: 1924
	public float buttonFadeTime = 0.25f;

	// Token: 0x04000785 RID: 1925
	public Text messageText;
}
