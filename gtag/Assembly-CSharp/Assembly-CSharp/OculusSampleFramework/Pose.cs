using System;
using UnityEngine;

namespace OculusSampleFramework
{
	public class Pose
	{
		public Pose()
		{
			this.Position = Vector3.zero;
			this.Rotation = Quaternion.identity;
		}

		public Pose(Vector3 position, Quaternion rotation)
		{
			this.Position = position;
			this.Rotation = rotation;
		}

		public Vector3 Position;

		public Quaternion Rotation;
	}
}
