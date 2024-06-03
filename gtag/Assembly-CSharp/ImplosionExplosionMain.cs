using System;
using BoingKit;
using UnityEngine;

public class ImplosionExplosionMain : MonoBehaviour
{
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

	public ImplosionExplosionMain()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static ImplosionExplosionMain()
	{
	}

	public BoingReactorField ReactorField;

	public GameObject Diamond;

	public int NumDiamonds;

	private static readonly int kNumInstancedBushesPerDrawCall = 1000;

	private Matrix4x4[][] m_aaInstancedDiamondMatrix;

	private MaterialPropertyBlock m_diamondMaterialProps;
}
