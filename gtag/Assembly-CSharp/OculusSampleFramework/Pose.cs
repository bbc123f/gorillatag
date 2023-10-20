using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002F1 RID: 753
	public class Pose
	{
		// Token: 0x06001470 RID: 5232 RVA: 0x000737AE File Offset: 0x000719AE
		public Pose()
		{
			this.Position = Vector3.zero;
			this.Rotation = Quaternion.identity;
		}

		// Token: 0x06001471 RID: 5233 RVA: 0x000737CC File Offset: 0x000719CC
		public Pose(Vector3 position, Quaternion rotation)
		{
			this.Position = position;
			this.Rotation = rotation;
		}

		// Token: 0x0400172B RID: 5931
		public Vector3 Position;

		// Token: 0x0400172C RID: 5932
		public Quaternion Rotation;
	}
}
