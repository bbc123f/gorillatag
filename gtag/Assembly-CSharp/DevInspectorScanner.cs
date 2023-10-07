using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000055 RID: 85
public class DevInspectorScanner : MonoBehaviour
{
	// Token: 0x04000271 RID: 625
	public Text hintTextOutput;

	// Token: 0x04000272 RID: 626
	public float scanDistance = 10f;

	// Token: 0x04000273 RID: 627
	public float scanAngle = 30f;

	// Token: 0x04000274 RID: 628
	public LayerMask scanLayerMask;

	// Token: 0x04000275 RID: 629
	public string targetComponentName;

	// Token: 0x04000276 RID: 630
	public float rayPerDegree = 10f;
}
