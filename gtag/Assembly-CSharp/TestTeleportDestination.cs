using System;
using GorillaTag;
using UnityEngine;

[GTStripGameObjectFromBuild("!PERFTESTING")]
public class TestTeleportDestination : MonoBehaviour
{
	public TestTeleportDestination()
	{
	}

	public GTZone[] zones;

	public GameObject teleportTransform;
}
