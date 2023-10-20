using System;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class GorillaCameraTriggerIndex : MonoBehaviour
{
	// Token: 0x0600081A RID: 2074 RVA: 0x00032E56 File Offset: 0x00031056
	private void Start()
	{
		this.parentTrigger = base.GetComponentInParent<GorillaCameraSceneTrigger>();
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x00032E64 File Offset: 0x00031064
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("SceneChanger"))
		{
			this.parentTrigger.mostRecentSceneTrigger = this;
			this.parentTrigger.ChangeScene(this);
		}
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x00032E90 File Offset: 0x00031090
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
