using System;
using UnityEngine;

// Token: 0x0200017E RID: 382
public class GorillaLightmapData : MonoBehaviour
{
	// Token: 0x060009A4 RID: 2468 RVA: 0x0003B0FC File Offset: 0x000392FC
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

	// Token: 0x04000BD6 RID: 3030
	[SerializeField]
	public Texture2D[] dirTextures;

	// Token: 0x04000BD7 RID: 3031
	[SerializeField]
	public Texture2D[] lightTextures;

	// Token: 0x04000BD8 RID: 3032
	public Color[][] lights;

	// Token: 0x04000BD9 RID: 3033
	public Color[][] dirs;

	// Token: 0x04000BDA RID: 3034
	public bool done;
}
