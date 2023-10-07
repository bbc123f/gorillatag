using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02000318 RID: 792
	public class DayNightWatchWearable : MonoBehaviour
	{
		// Token: 0x060015D6 RID: 5590 RVA: 0x000786A0 File Offset: 0x000768A0
		private void Start()
		{
			if (!this.dayNightManager)
			{
				this.dayNightManager = BetterDayNightManager.instance;
			}
			this.rotationDegree = 0f;
			if (this.clockNeedle)
			{
				this.initialRotation = this.clockNeedle.localRotation;
			}
		}

		// Token: 0x060015D7 RID: 5591 RVA: 0x000786F0 File Offset: 0x000768F0
		private void Update()
		{
			this.currentTimeOfDay = this.dayNightManager.currentTimeOfDay;
			double currentTimeInSeconds = ((ITimeOfDaySystem)this.dayNightManager).currentTimeInSeconds;
			double totalTimeInSeconds = ((ITimeOfDaySystem)this.dayNightManager).totalTimeInSeconds;
			this.rotationDegree = (float)(360.0 * currentTimeInSeconds / totalTimeInSeconds);
			this.rotationDegree = Mathf.Floor(this.rotationDegree);
			if (this.clockNeedle)
			{
				this.clockNeedle.localRotation = this.initialRotation * Quaternion.AngleAxis(this.rotationDegree, this.needleRotationAxis);
			}
		}

		// Token: 0x040017DD RID: 6109
		[Tooltip("The transform that will be rotated to indicate the current time.")]
		public Transform clockNeedle;

		// Token: 0x040017DE RID: 6110
		[FormerlySerializedAs("dialRotationAxis")]
		[Tooltip("The axis that the needle will rotate around.")]
		public Vector3 needleRotationAxis = Vector3.right;

		// Token: 0x040017DF RID: 6111
		private BetterDayNightManager dayNightManager;

		// Token: 0x040017E0 RID: 6112
		[DebugOption]
		private float rotationDegree;

		// Token: 0x040017E1 RID: 6113
		private string currentTimeOfDay;

		// Token: 0x040017E2 RID: 6114
		private Quaternion initialRotation;
	}
}
