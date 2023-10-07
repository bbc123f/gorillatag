using System;
using UnityEngine;

// Token: 0x0200013A RID: 314
public class GorillaCameraSceneTrigger : MonoBehaviour
{
	// Token: 0x06000817 RID: 2071 RVA: 0x00032FA8 File Offset: 0x000311A8
	public void ChangeScene(GorillaCameraTriggerIndex triggerLeft)
	{
		if (triggerLeft == this.currentSceneTrigger || this.currentSceneTrigger == null)
		{
			if (this.mostRecentSceneTrigger != this.currentSceneTrigger)
			{
				this.sceneCamera.SetSceneCamera(this.mostRecentSceneTrigger.sceneTriggerIndex);
				this.currentSceneTrigger = this.mostRecentSceneTrigger;
				return;
			}
			this.currentSceneTrigger = null;
		}
	}

	// Token: 0x040009D4 RID: 2516
	public GorillaSceneCamera sceneCamera;

	// Token: 0x040009D5 RID: 2517
	public GorillaCameraTriggerIndex currentSceneTrigger;

	// Token: 0x040009D6 RID: 2518
	public GorillaCameraTriggerIndex mostRecentSceneTrigger;
}
