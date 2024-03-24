using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class LightningDispatcher : MonoBehaviour
{
	public static event LightningDispatcher.DispatchLightningEvent RequestLightningStrike
	{
		[CompilerGenerated]
		add
		{
			LightningDispatcher.DispatchLightningEvent dispatchLightningEvent = LightningDispatcher.RequestLightningStrike;
			LightningDispatcher.DispatchLightningEvent dispatchLightningEvent2;
			do
			{
				dispatchLightningEvent2 = dispatchLightningEvent;
				LightningDispatcher.DispatchLightningEvent dispatchLightningEvent3 = (LightningDispatcher.DispatchLightningEvent)Delegate.Combine(dispatchLightningEvent2, value);
				dispatchLightningEvent = Interlocked.CompareExchange<LightningDispatcher.DispatchLightningEvent>(ref LightningDispatcher.RequestLightningStrike, dispatchLightningEvent3, dispatchLightningEvent2);
			}
			while (dispatchLightningEvent != dispatchLightningEvent2);
		}
		[CompilerGenerated]
		remove
		{
			LightningDispatcher.DispatchLightningEvent dispatchLightningEvent = LightningDispatcher.RequestLightningStrike;
			LightningDispatcher.DispatchLightningEvent dispatchLightningEvent2;
			do
			{
				dispatchLightningEvent2 = dispatchLightningEvent;
				LightningDispatcher.DispatchLightningEvent dispatchLightningEvent3 = (LightningDispatcher.DispatchLightningEvent)Delegate.Remove(dispatchLightningEvent2, value);
				dispatchLightningEvent = Interlocked.CompareExchange<LightningDispatcher.DispatchLightningEvent>(ref LightningDispatcher.RequestLightningStrike, dispatchLightningEvent3, dispatchLightningEvent2);
			}
			while (dispatchLightningEvent != dispatchLightningEvent2);
		}
	}

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

	public LightningDispatcher()
	{
	}

	[SerializeField]
	private float beamWidthCM = 1f;

	[SerializeField]
	private float soundVolumeMultiplier = 1f;

	[CompilerGenerated]
	private static LightningDispatcher.DispatchLightningEvent RequestLightningStrike;

	public delegate LightningStrike DispatchLightningEvent(Vector3 p1, Vector3 p2);
}
