using System;
using UnityEngine;

// Token: 0x02000143 RID: 323
public class GorillaSceneCamera : MonoBehaviour
{
	// Token: 0x06000831 RID: 2097 RVA: 0x000332AA File Offset: 0x000314AA
	public void SetSceneCamera(int sceneIndex)
	{
		base.transform.position = this.sceneTransforms[sceneIndex].scenePosition;
		base.transform.eulerAngles = this.sceneTransforms[sceneIndex].sceneRotation;
	}

	// Token: 0x04000A17 RID: 2583
	public GorillaSceneTransform[] sceneTransforms;
}
