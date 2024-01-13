using UnityEngine;

namespace GorillaLocomotion.Swimming;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WaterParameters", order = 1)]
public class WaterParameters : ScriptableObject
{
	public float recomputeSurfaceForColliderDist = 0.2f;

	public bool sendSplashEffectRPCs;

	public GameObject rippleEffect;

	public float rippleEffectScale = 1f;

	public float defaultDistanceBetweenRipples = 0.75f;

	public float minDistanceBetweenRipples = 0.2f;

	public float minTimeBetweenRipples = 0.75f;

	public float postExitDripDuration = 1.5f;

	public float perDripTimeDelay = 0.2f;

	public float perDripTimeRandRange = 0.15f;

	public float perDripDefaultRadius = 0.01f;

	public float perDripRadiusRandRange = 0.01f;

	public GameObject splashEffect;

	public bool playSplashEffect;

	public float splashEffectScale = 1f;

	public float splashSpeedRequirement = 0.8f;

	public float bigSplashSpeedRequirement = 1.9f;
}
