using System;

namespace GorillaLocomotion.Gameplay
{
	internal interface IGorillaGrabable
	{
		void OnGrabbed();

		void OnGrabReleased();
	}
}
