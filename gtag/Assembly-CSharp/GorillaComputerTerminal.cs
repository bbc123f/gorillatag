using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001AD RID: 429
public class GorillaComputerTerminal : MonoBehaviour
{
	// Token: 0x06000B02 RID: 2818 RVA: 0x00044298 File Offset: 0x00042498
	private void LateUpdate()
	{
		this.myScreenText.text = GorillaComputer.instance.screenText.Text;
		this.myFunctionText.text = GorillaComputer.instance.functionSelectText.Text;
		this.monitorMesh.materials = GorillaComputer.instance.computerScreenRenderer.materials;
	}

	// Token: 0x04000E2A RID: 3626
	public Text myScreenText;

	// Token: 0x04000E2B RID: 3627
	public Text myFunctionText;

	// Token: 0x04000E2C RID: 3628
	public MeshRenderer monitorMesh;
}
