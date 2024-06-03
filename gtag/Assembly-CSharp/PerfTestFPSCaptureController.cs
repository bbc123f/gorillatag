using System;
using GorillaTag;
using UnityEngine;

[GTStripGameObjectFromBuild("!PERFTESTING")]
public class PerfTestFPSCaptureController : MonoBehaviour
{
	public PerfTestFPSCaptureController()
	{
	}

	[SerializeField]
	private SerializablePerformanceReport<ScenePerformanceData> performanceSummary;

	public float captureWaitTime = 0.25f;
}
