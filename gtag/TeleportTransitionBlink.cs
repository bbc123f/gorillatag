using System.Collections;
using UnityEngine;

public class TeleportTransitionBlink : TeleportTransition
{
	[Tooltip("How long the transition takes. Usually this is greater than Teleport Delay.")]
	[Range(0.01f, 2f)]
	public float TransitionDuration = 0.5f;

	[Tooltip("At what percentage of the elapsed transition time does the teleport occur?")]
	[Range(0f, 1f)]
	public float TeleportDelay = 0.5f;

	[Tooltip("Fade to black over the duration of the transition")]
	public AnimationCurve FadeLevels = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	protected override void LocomotionTeleportOnEnterStateTeleporting()
	{
		StartCoroutine(BlinkCoroutine());
	}

	protected IEnumerator BlinkCoroutine()
	{
		base.LocomotionTeleport.IsTransitioning = true;
		float elapsedTime = 0f;
		float teleportTime = TransitionDuration * TeleportDelay;
		bool teleported = false;
		while (elapsedTime < TransitionDuration)
		{
			yield return null;
			elapsedTime += Time.deltaTime;
			if (!teleported && elapsedTime >= teleportTime)
			{
				teleported = true;
				base.LocomotionTeleport.DoTeleport();
			}
		}
		base.LocomotionTeleport.IsTransitioning = false;
	}
}
