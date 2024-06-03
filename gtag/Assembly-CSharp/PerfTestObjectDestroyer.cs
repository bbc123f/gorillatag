using System;
using GorillaTag;
using UnityEngine;

[GTStripGameObjectFromBuild("!PERFTESTING")]
public class PerfTestObjectDestroyer : MonoBehaviour
{
	private void Start()
	{
		Object.DestroyImmediate(base.gameObject, true);
	}

	public PerfTestObjectDestroyer()
	{
	}
}
