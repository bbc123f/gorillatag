using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000174 RID: 372
public class GorillaGeoHideShowTrigger : GorillaTriggerBox
{
	// Token: 0x0600095F RID: 2399 RVA: 0x000389F4 File Offset: 0x00036BF4
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

	// Token: 0x04000B76 RID: 2934
	public GameObject[] makeSureThisIsDisabled;

	// Token: 0x04000B77 RID: 2935
	public GameObject[] makeSureThisIsEnabled;

	// Token: 0x04000B78 RID: 2936
	public bool lotsOfStuff;
}
