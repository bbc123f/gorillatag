using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GorillaSkin : ScriptableObject
{
	public Mesh bodyMesh
	{
		get
		{
			return this._bodyMesh;
		}
	}

	public Material faceMaterial
	{
		get
		{
			return this._faceMaterial;
		}
	}

	public Material bodyMaterial
	{
		get
		{
			return this._bodyMaterial;
		}
	}

	public Material chestMaterial
	{
		get
		{
			return this._chestMaterial;
		}
	}

	public Material scoreboardMaterial
	{
		get
		{
			return this._scoreboardMaterial;
		}
	}

	public static void ApplyToRig(VRRig rig, GorillaSkin skin)
	{
		if (skin.bodyMesh != null)
		{
			rig.mainSkin.sharedMesh = skin.bodyMesh;
		}
		if (skin == rig.defaultSkin)
		{
			rig.materialsToChangeTo[0] = rig.myDefaultSkinMaterialInstance;
		}
		else
		{
			rig.materialsToChangeTo[0] = skin.bodyMaterial;
		}
		Material[] sharedMaterials = rig.mainSkin.sharedMaterials;
		sharedMaterials[0] = rig.materialsToChangeTo[rig.setMatIndex];
		sharedMaterials[1] = skin.chestMaterial;
		rig.mainSkin.sharedMaterials = sharedMaterials;
		rig.scoreboardMaterial = skin.scoreboardMaterial;
	}

	public GorillaSkin()
	{
	}

	[FormerlySerializedAs("faceMaterial")]
	[Space]
	[SerializeField]
	private Material _faceMaterial;

	[FormerlySerializedAs("chestMaterial")]
	[FormerlySerializedAs("chestEarsMaterial")]
	[SerializeField]
	private Material _chestMaterial;

	[FormerlySerializedAs("bodyMaterial")]
	[SerializeField]
	private Material _bodyMaterial;

	[SerializeField]
	private Material _scoreboardMaterial;

	[Space]
	[SerializeField]
	private Mesh _bodyMesh;

	[Space]
	[NonSerialized]
	private Material _bodyRuntime;

	[NonSerialized]
	private Material _chestRuntime;

	[NonSerialized]
	private Material _faceRuntime;

	[NonSerialized]
	private Material _scoreRuntime;
}
