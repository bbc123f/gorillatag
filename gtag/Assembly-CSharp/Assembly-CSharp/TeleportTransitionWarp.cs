using System;
using System.Collections;
using UnityEngine;

public class TeleportTransitionWarp : TeleportTransition
{
	protected override void LocomotionTeleportOnEnterStateTeleporting()
	{
		base.StartCoroutine(this.DoWarp());
	}

	private IEnumerator DoWarp()
	{
		base.LocomotionTeleport.IsTransitioning = true;
		Vector3 startPosition = base.LocomotionTeleport.GetCharacterPosition();
		float elapsedTime = 0f;
		while (elapsedTime < this.TransitionDuration)
		{
			elapsedTime += Time.deltaTime;
			float time = elapsedTime / this.TransitionDuration;
			float positionPercent = this.PositionLerp.Evaluate(time);
			base.LocomotionTeleport.DoWarp(startPosition, positionPercent);
			yield return null;
		}
		base.LocomotionTeleport.DoWarp(startPosition, 1f);
		base.LocomotionTeleport.IsTransitioning = false;
		yield break;
	}

	[Tooltip("How much time the warp transition takes to complete.")]
	[Range(0.01f, 1f)]
	public float TransitionDuration = 0.5f;

	[HideInInspector]
	public AnimationCurve PositionLerp = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
