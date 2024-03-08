using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GorillaSkin : ScriptableObject
{
	public Material faceMaterial
	{
		get
		{
			if (Application.isPlaying)
			{
				if (this._faceRuntime == null)
				{
					this._faceRuntime = new Material(this._faceMaterial);
				}
				return this._faceRuntime;
			}
			return this._faceMaterial;
		}
	}

	public Material bodyMaterial
	{
		get
		{
			if (Application.isPlaying)
			{
				if (this._bodyRuntime == null)
				{
					this._bodyRuntime = new Material(this._bodyMaterial);
				}
				return this._bodyRuntime;
			}
			return this._bodyMaterial;
		}
	}

	public Material chestMaterial
	{
		get
		{
			if (Application.isPlaying)
			{
				if (this._chestRuntime == null)
				{
					this._chestRuntime = new Material(this._chestMaterial);
				}
				return this._chestRuntime;
			}
			return this._chestMaterial;
		}
	}

	public static void ApplyToRig(VRRig rig, GorillaSkin skin)
	{
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
	}

	[DebugReadOnly]
	private int _id;

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

	[Space]
	[DebugReadOnly]
	[NonSerialized]
	private Material _bodyRuntime;

	[DebugReadOnly]
	[NonSerialized]
	private Material _chestRuntime;

	[DebugReadOnly]
	[NonSerialized]
	private Material _faceRuntime;
}
