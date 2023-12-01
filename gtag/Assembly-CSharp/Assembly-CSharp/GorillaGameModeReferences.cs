using System;
using GorillaLocomotion.Swimming;
using GorillaTag;
using UnityEngine;

public class GorillaGameModeReferences : MonoBehaviour
{
	public static GorillaGameModeReferences Instance
	{
		get
		{
			return GorillaGameModeReferences.instance;
		}
	}

	protected void Awake()
	{
		if (GorillaGameModeReferences.instance != null && GorillaGameModeReferences.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		GorillaGameModeReferences.instance = this;
	}

	[OnEnterPlay_SetNull]
	private static GorillaGameModeReferences instance;

	[Header("InfectionLavaController")]
	public Transform lavaMeshTransform;

	public Transform lavaSurfacePlaneTransform;

	public WaterVolume lavaVolume;

	public MeshRenderer lavaActivationRenderer;

	public Transform lavaActivationStartPos;

	public Transform lavaActivationEndPos;

	public SlingshotProjectileHitNotifier lavaActivationProjectileHitNotifier;

	public VolcanoEffects[] volcanoEffects;
}
