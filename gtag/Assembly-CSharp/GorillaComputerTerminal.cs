using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001AE RID: 430
public class GorillaComputerTerminal : MonoBehaviour
{
	// Token: 0x06000B07 RID: 2823 RVA: 0x000443D0 File Offset: 0x000425D0
	private void LateUpdate()
	{
		this.myScreenText.text = GorillaComputer.instance.screenText.Text;
		this.myFunctionText.text = GorillaComputer.instance.functionSelectText.Text;
		this.monitorMesh.materials = GorillaComputer.instance.computerScreenRenderer.materials;
	}

	// Token: 0x04000E2E RID: 3630
	public Text myScreenText;

	// Token: 0x04000E2F RID: 3631
	public Text myFunctionText;

	// Token: 0x04000E30 RID: 3632
	public MeshRenderer monitorMesh;
}
