﻿using System;
using UnityEngine;

public class SlingshotTestScenarioBodyShot : SlingshotTestScenario
{
	public SlingshotTestScenarioBodyShot()
	{
	}

	public GameObject projectilePrefab;

	public VRRig targetRig;

	public Collider[] targetColliders;

	public GameObject anchor;
}
