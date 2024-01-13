using System.Collections;
using UnityEngine;

public class TeleportTransitionWarp : TeleportTransition
{
	[Tooltip("How much time the warp transition takes to complete.")]
	[Range(0.01f, 1f)]
	public float TransitionDuration = 0.5f;

	[HideInInspector]
	public AnimationCurve PositionLerp = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	protected override void LocomotionTeleportOnEnterStateTeleporting()
	{
		StartCoroutine(DoWarp());
	}

	private IEnumerator DoWarp()
	{
		base.LocomotionTeleport.IsTransitioning = true;
		Vector3 startPosition = base.LocomotionTeleport.GetCharacterPosition();
		float elapsedTime = 0f;
		while (elapsedTime < TransitionDuration)
		{
			elapsedTime += Time.deltaTime;
			float time = elapsedTime / TransitionDuration;
			float positionPercent = PositionLerp.Evaluate(time);
			base.LocomotionTeleport.DoWarp(startPosition, positionPercent);
			yield return null;
		}
		base.LocomotionTeleport.DoWarp(startPosition, 1f);
		base.LocomotionTeleport.IsTransitioning = false;
	}
}
