using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000097 RID: 151
public class TeleportTransitionBlink : TeleportTransition
{
	// Token: 0x0600033D RID: 829 RVA: 0x00013570 File Offset: 0x00011770
	protected override void LocomotionTeleportOnEnterStateTeleporting()
	{
		base.StartCoroutine(this.BlinkCoroutine());
	}

	// Token: 0x0600033E RID: 830 RVA: 0x0001357F File Offset: 0x0001177F
	protected IEnumerator BlinkCoroutine()
	{
		base.LocomotionTeleport.IsTransitioning = true;
		float elapsedTime = 0f;
		float teleportTime = this.TransitionDuration * this.TeleportDelay;
		bool teleported = false;
		while (elapsedTime < this.TransitionDuration)
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
		yield break;
	}

	// Token: 0x040003E4 RID: 996
	[Tooltip("How long the transition takes. Usually this is greater than Teleport Delay.")]
	[Range(0.01f, 2f)]
	public float TransitionDuration = 0.5f;

	// Token: 0x040003E5 RID: 997
	[Tooltip("At what percentage of the elapsed transition time does the teleport occur?")]
	[Range(0f, 1f)]
	public float TeleportDelay = 0.5f;

	// Token: 0x040003E6 RID: 998
	[Tooltip("Fade to black over the duration of the transition")]
	public AnimationCurve FadeLevels = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.5f, 1f),
		new Keyframe(1f, 0f)
	});
}
