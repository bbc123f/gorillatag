using System;
using GorillaExtensions;
using UnityEngine;

public class GorillaGeoHideShowTrigger : GorillaTriggerBox
{
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

	public GameObject[] makeSureThisIsDisabled;

	public GameObject[] makeSureThisIsEnabled;

	public bool lotsOfStuff;
}
