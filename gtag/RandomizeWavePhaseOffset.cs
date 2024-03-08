using System;
using UnityEngine;

public class RandomizeWavePhaseOffset : MonoBehaviour
{
	private void Start()
	{
		base.GetComponent<MeshRenderer>().material.SetFloat(this._VertexWavePhaseOffset, Random.Range(this.minPhaseOffset, this.maxPhaseOffset));
	}

	private ShaderHashId _VertexWavePhaseOffset = "_VertexWavePhaseOffset";

	[SerializeField]
	private float minPhaseOffset;

	[SerializeField]
	private float maxPhaseOffset;
}
