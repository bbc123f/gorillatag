using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200017D RID: 381
public class GorillaLevelScreen : MonoBehaviour
{
	// Token: 0x060009A1 RID: 2465 RVA: 0x0003B099 File Offset: 0x00039299
	private void Awake()
	{
		this.startingText = this.myText.text;
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x0003B0AC File Offset: 0x000392AC
	public void UpdateText(string newText, bool setToGoodMaterial)
	{
		this.myText.text = newText;
		Material[] materials = base.GetComponent<MeshRenderer>().materials;
		materials[0] = (setToGoodMaterial ? this.goodMaterial : this.badMaterial);
		base.GetComponent<MeshRenderer>().materials = materials;
	}

	// Token: 0x04000BD2 RID: 3026
	public string startingText;

	// Token: 0x04000BD3 RID: 3027
	public Material goodMaterial;

	// Token: 0x04000BD4 RID: 3028
	public Material badMaterial;

	// Token: 0x04000BD5 RID: 3029
	public Text myText;
}
