using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	internal interface IGorillaGrabable
	{
		Transform OnGrabbed(GorillaGrabber grabber);

		Transform OnGrabReleased(GorillaGrabber grabber);
	}
}
