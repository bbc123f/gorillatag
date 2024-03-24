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
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.Remove();
	}

	public void Apply()
	{
		GorillaSkin.ApplyToRig(this._rig, this._skin);
		this._applied = true;
	}

	public void Remove()
	{
		GorillaSkin.ApplyToRig(this._rig, this._default);
		float @float = PlayerPrefs.GetFloat("redValue", 0.0627451f);
		float float2 = PlayerPrefs.GetFloat("greenValue", 0.0627451f);
		float float3 = PlayerPrefs.GetFloat("blueValue", 0.0627451f);
		GorillaTagger.Instance.UpdateColor(@float, float2, float3);
		this._applied = false;
	}

	public GorillaSkinToggle()
	{
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
