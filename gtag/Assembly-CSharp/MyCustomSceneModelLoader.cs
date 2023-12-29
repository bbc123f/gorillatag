using System;
using System.Collections;
using UnityEngine;

public class MyCustomSceneModelLoader : OVRSceneModelLoader
{
	private IEnumerator DelayedLoad()
	{
		yield return new WaitForSeconds(1f);
		Debug.Log("[MyCustomSceneLoader] calling OVRSceneManager.LoadSceneModel() delayed by 1 second");
		base.SceneManager.LoadSceneModel();
		yield break;
	}

	protected override void OnStart()
	{
		base.StartCoroutine(this.DelayedLoad());
	}

	protected override void OnNoSceneModelToLoad()
	{
		Debug.Log("[MyCustomSceneLoader] There is no scene to load, but we don't want to trigger scene capture.");
	}
}
