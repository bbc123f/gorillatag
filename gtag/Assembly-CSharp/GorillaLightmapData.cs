using System;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class GorillaLightmapData : MonoBehaviour
{
	// Token: 0x060009A8 RID: 2472 RVA: 0x0003B0B4 File Offset: 0x000392B4
	public void Awake()
	{
		this.lights = new Color[this.lightTextures.Length][];
		this.dirs = new Color[this.dirTextures.Length][];
		for (int i = 0; i < this.dirTextures.Length; i++)
		{
			float value = Random.value;
			Debug.Log(value.ToString() + " before load " + Time.realtimeSinceStartup.ToString());
			this.dirs[i] = this.dirTextures[i].GetPixels();
			this.lights[i] = this.lightTextures[i].GetPixels();
			Debug.Log(value.ToString() + " after load " + Time.realtimeSinceStartup.ToString());
		}
	}

	// Token: 0x04000BDA RID: 3034
	[SerializeField]
	public Texture2D[] dirTextures;

	// Token: 0x04000BDB RID: 3035
	[SerializeField]
	public Texture2D[] lightTextures;

	// Token: 0x04000BDC RID: 3036
	public Color[][] lights;

	// Token: 0x04000BDD RID: 3037
	public Color[][] dirs;

	// Token: 0x04000BDE RID: 3038
	public bool done;
}
