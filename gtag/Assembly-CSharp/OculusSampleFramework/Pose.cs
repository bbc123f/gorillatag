using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002EF RID: 751
	public class Pose
	{
		// Token: 0x06001469 RID: 5225 RVA: 0x000732E2 File Offset: 0x000714E2
		public Pose()
		{
			this.Position = Vector3.zero;
			this.Rotation = Quaternion.identity;
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x00073300 File Offset: 0x00071500
		public Pose(Vector3 position, Quaternion rotation)
		{
			this.Position = position;
			this.Rotation = rotation;
		}

		// Token: 0x0400171E RID: 5918
		public Vector3 Position;

		// Token: 0x0400171F RID: 5919
		public Quaternion Rotation;
	}
}
