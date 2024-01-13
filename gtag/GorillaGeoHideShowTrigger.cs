using UnityEngine;

public class GorillaGeoHideShowTrigger : GorillaTriggerBox
{
	public GameObject[] makeSureThisIsDisabled;

	public GameObject[] makeSureThisIsEnabled;

	public bool lotsOfStuff;

	public override void OnBoxTriggered()
	{
		if (makeSureThisIsDisabled != null)
		{
			GameObject[] array = makeSureThisIsDisabled;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
		if (makeSureThisIsEnabled != null)
		{
			GameObject[] array = makeSureThisIsEnabled;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
		}
	}
}
