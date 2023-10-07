using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200000F RID: 15
public class ImplosionExplosionMain : MonoBehaviour
{
	// Token: 0x0600002D RID: 45 RVA: 0x0000302C File Offset: 0x0000122C
	public void Start()
	{
		this.m_aaInstancedDiamondMatrix = new Matrix4x4[(this.NumDiamonds + ImplosionExplosionMain.kNumInstancedBushesPerDrawCall - 1) / ImplosionExplosionMain.kNumInstancedBushesPerDrawCall][];
		for (int i = 0; i < this.m_aaInstancedDiamondMatrix.Length; i++)
		{
			this.m_aaInstancedDiamondMatrix[i] = new Matrix4x4[ImplosionExplosionMain.kNumInstancedBushesPerDrawCall];
		}
		for (int j = 0; j < this.NumDiamonds; j++)
		{
			float d = Random.Range(0.1f, 0.4f);
			Vector3 pos = new Vector3(Random.Range(-3.5f, 3.5f), Random.Range(0.5f, 7f), Random.Range(-3.5f, 3.5f));
			Quaternion q = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
			this.m_aaInstancedDiamondMatrix[j / ImplosionExplosionMain.kNumInstancedBushesPerDrawCall][j % ImplosionExplosionMain.kNumInstancedBushesPerDrawCall].SetTRS(pos, q, d * Vector3.one);
		}
	}

	// Token: 0x0600002E RID: 46 RVA: 0x0000313C File Offset: 0x0000133C
	public void Update()
	{
		Mesh sharedMesh = this.Diamond.GetComponent<MeshFilter>().sharedMesh;
		Material sharedMaterial = this.Diamond.GetComponent<MeshRenderer>().sharedMaterial;
		if (this.m_diamondMaterialProps == null)
		{
			this.m_diamondMaterialProps = new MaterialPropertyBlock();
		}
		if (this.ReactorField.UpdateShaderConstants(this.m_diamondMaterialProps, 1f, 1f))
		{
			foreach (Matrix4x4[] array in this.m_aaInstancedDiamondMatrix)
			{
				Graphics.DrawMeshInstanced(sharedMesh, 0, sharedMaterial, array, array.Length, this.m_diamondMaterialProps);
			}
		}
	}

	// Token: 0x04000031 RID: 49
	public BoingReactorField ReactorField;

	// Token: 0x04000032 RID: 50
	public GameObject Diamond;

	// Token: 0x04000033 RID: 51
	public int NumDiamonds;

	// Token: 0x04000034 RID: 52
	private static readonly int kNumInstancedBushesPerDrawCall = 1000;

	// Token: 0x04000035 RID: 53
	private Matrix4x4[][] m_aaInstancedDiamondMatrix;

	// Token: 0x04000036 RID: 54
	private MaterialPropertyBlock m_diamondMaterialProps;
}
