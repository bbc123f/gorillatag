using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000175 RID: 373
public class GorillaGeoHideShowTrigger : GorillaTriggerBox
{
	// Token: 0x06000963 RID: 2403 RVA: 0x000389AC File Offset: 0x00036BAC
	public override void OnBoxTriggered()
	{
		if (this.makeSureThisIsDisabled != null)
		{
			foreach (GameObject gameObject in this.makeSureThisIsDisabled)
			{
				if (gameObject == null)
				{
					Debug.LogError("GorillaGeoHideShowTrigger: null item in makeSureThisIsDisabled. \"" + base.transform.GetPath() + "\"", this);
					return;
				}
				gameObject.SetActive(false);
			}
		}
		if (this.makeSureThisIsEnabled != null)
		{
			foreach (GameObject gameObject2 in this.makeSureThisIsEnabled)
			{
				if (gameObject2 == null)
				{
					Debug.LogError("GorillaGeoHideShowTrigger: null item in makeSureThisIsDisabled. \"" + base.transform.GetPath() + "\"", this);
					return;
				}
				gameObject2.SetActive(true);
			}
		}
	}

	// Token: 0x04000B7A RID: 2938
	public GameObject[] makeSureThisIsDisabled;

	// Token: 0x04000B7B RID: 2939
	public GameObject[] makeSureThisIsEnabled;

	// Token: 0x04000B7C RID: 2940
	public bool lotsOfStuff;
}
