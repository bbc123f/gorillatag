using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200017E RID: 382
public class GorillaLevelScreen : MonoBehaviour
{
	// Token: 0x060009A5 RID: 2469 RVA: 0x0003B051 File Offset: 0x00039251
	private void Awake()
	{
		this.startingText = this.myText.text;
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x0003B064 File Offset: 0x00039264
	public void UpdateText(string newText, bool setToGoodMaterial)
	{
		this.myText.text = newText;
		Material[] materials = base.GetComponent<MeshRenderer>().materials;
		materials[0] = (setToGoodMaterial ? this.goodMaterial : this.badMaterial);
		base.GetComponent<MeshRenderer>().materials = materials;
	}

	// Token: 0x04000BD6 RID: 3030
	public string startingText;

	// Token: 0x04000BD7 RID: 3031
	public Material goodMaterial;

	// Token: 0x04000BD8 RID: 3032
	public Material badMaterial;

	// Token: 0x04000BD9 RID: 3033
	public Text myText;
}
