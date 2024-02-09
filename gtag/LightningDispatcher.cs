using System;
using UnityEngine;

public class LightningDispatcher : MonoBehaviour
{
	public static event LightningDispatcher.DispatchLightningEvent RequestLightningStrike;

	public void DispatchLightning(Vector3 p1, Vector3 p2)
	{
		if (LightningDispatcher.RequestLightningStrike != null)
		{
			LightningStrike lightningStrike = LightningDispatcher.RequestLightningStrike(p1, p2);
			float num = Mathf.Max(new float[]
			{
				base.transform.lossyScale.x,
				base.transform.lossyScale.y,
				base.transform.lossyScale.z
			});
			lightningStrike.Play(p1, p2, this.beamWidthCM * 0.01f * num, this.soundVolumeMultiplier / num);
		}
	}

	[SerializeField]
	private float beamWidthCM = 1f;

	[SerializeField]
	private float soundVolumeMultiplier = 1f;

	public delegate LightningStrike DispatchLightningEvent(Vector3 p1, Vector3 p2);
}
