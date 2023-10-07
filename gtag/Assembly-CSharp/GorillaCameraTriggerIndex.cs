using System;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class GorillaCameraTriggerIndex : MonoBehaviour
{
	// Token: 0x06000819 RID: 2073 RVA: 0x00033016 File Offset: 0x00031216
	private void Start()
	{
		this.parentTrigger = base.GetComponentInParent<GorillaCameraSceneTrigger>();
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x00033024 File Offset: 0x00031224
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("SceneChanger"))
		{
			this.parentTrigger.mostRecentSceneTrigger = this;
			this.parentTrigger.ChangeScene(this);
		}
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x00033050 File Offset: 0x00031250
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("SceneChanger"))
		{
			this.parentTrigger.ChangeScene(this);
		}
	}

	// Token: 0x040009D7 RID: 2519
	public int sceneTriggerIndex;

	// Token: 0x040009D8 RID: 2520
	public GorillaCameraSceneTrigger parentTrigger;
}
