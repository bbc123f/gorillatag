using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000BF RID: 191
public class MyCustomSceneModelLoader : OVRSceneModelLoader
{
	// Token: 0x06000430 RID: 1072 RVA: 0x0001B7B9 File Offset: 0x000199B9
	private IEnumerator DelayedLoad()
	{
		yield return new WaitForSeconds(1f);
		Debug.Log("[MyCustomSceneLoader] calling OVRSceneManager.LoadSceneModel() delayed by 1 second");
		base.SceneManager.LoadSceneModel();
		yield break;
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x0001B7C8 File Offset: 0x000199C8
	protected override void OnStart()
	{
		base.StartCoroutine(this.DelayedLoad());
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x0001B7D7 File Offset: 0x000199D7
	protected override void OnNoSceneModelToLoad()
	{
		Debug.Log("[MyCustomSceneLoader] There is no scene to load, but we don't want to trigger scene capture.");
	}
}
