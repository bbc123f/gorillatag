using System;
using UnityEngine;

public class GorillaSkinToggle : MonoBehaviour
{
	public bool applied
	{
		get
		{
			return this._applied;
		}
	}

	private void Awake()
	{
		this._rig = base.GetComponentInParent<VRRig>();
	}

	private void OnEnable()
	{
		this.Apply();
	}

	private void OnDisable()
	{
		this.Remove();
	}

	public void Apply()
	{
		GorillaSkinToggle.ApplyToRig(this._rig, this._skin);
		this._applied = true;
	}

	public void Remove()
	{
		GorillaSkinToggle.ApplyToRig(this._rig, this._default);
		float @float = PlayerPrefs.GetFloat("redValue", 0.16f);
		float float2 = PlayerPrefs.GetFloat("greenValue", 0.16f);
		float float3 = PlayerPrefs.GetFloat("blueValue", 0.16f);
		GorillaTagger.Instance.UpdateColor(@float, float2, float3);
		this._applied = false;
	}

	private static void ApplyToRig(VRRig rig, GorillaSkin skin)
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

	[SerializeField]
	private VRRig _rig;

	[SerializeField]
	private GorillaSkin _skin;

	[SerializeField]
	private GorillaSkin _default;

	[Space]
	[SerializeField]
	private bool _applied;
}
