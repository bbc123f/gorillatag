using System.Collections;
using UnityEngine;

public class GorillaTriggerBox : MonoBehaviour
{
	public bool triggerBoxOnce;

	private static WaitForSeconds testDelay;

	private void Awake()
	{
		if (testDelay == null)
		{
			testDelay = new WaitForSeconds(0.1f);
		}
	}

	private void OnEnable()
	{
		if (Application.isEditor)
		{
			StartCoroutine(TestTrigger());
		}
	}

	private void OnDisable()
	{
		if (Application.isEditor)
		{
			StopAllCoroutines();
		}
	}

	private IEnumerator TestTrigger()
	{
		while (true)
		{
			if (triggerBoxOnce)
			{
				triggerBoxOnce = false;
				OnBoxTriggered();
			}
			yield return testDelay;
		}
	}

	public virtual void OnBoxTriggered()
	{
	}
}
