using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000053 RID: 83
public class DevInspector : MonoBehaviour
{
	// Token: 0x0600018E RID: 398 RVA: 0x0000C163 File Offset: 0x0000A363
	private void OnEnable()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04000269 RID: 617
	public GameObject pivot;

	// Token: 0x0400026A RID: 618
	public Text outputInfo;

	// Token: 0x0400026B RID: 619
	public Component[] componentToInspect;

	// Token: 0x0400026C RID: 620
	public bool isEnabled;

	// Token: 0x0400026D RID: 621
	public bool autoFind = true;

	// Token: 0x0400026E RID: 622
	public GameObject canvas;

	// Token: 0x0400026F RID: 623
	public int sidewaysOffset;
}
