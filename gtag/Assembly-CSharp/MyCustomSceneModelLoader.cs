using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000BF RID: 191
public class MyCustomSceneModelLoader : OVRSceneModelLoader
{
	// Token: 0x06000430 RID: 1072 RVA: 0x0001B9DD File Offset: 0x00019BDD
	private IEnumerator DelayedLoad()
	{
		yield return new WaitForSeconds(1f);
		Debug.Log("[MyCustomSceneLoader] calling OVRSceneManager.LoadSceneModel() delayed by 1 second");
		base.SceneManager.LoadSceneModel();
		yield break;
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x0001B9EC File Offset: 0x00019BEC
	protected override void OnStart()
	{
		base.StartCoroutine(this.DelayedLoad());
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x0001B9FB File Offset: 0x00019BFB
	protected override void OnNoSceneModelToLoad()
	{
		Debug.Log("[MyCustomSceneLoader] There is no scene to load, but we don't want to trigger scene capture.");
	}
}
