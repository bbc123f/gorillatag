using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x0200031A RID: 794
	public class DayNightWatchWearable : MonoBehaviour
	{
		// Token: 0x060015DF RID: 5599 RVA: 0x00078B88 File Offset: 0x00076D88
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

		// Token: 0x060015E0 RID: 5600 RVA: 0x00078BD8 File Offset: 0x00076DD8
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

		// Token: 0x040017EA RID: 6122
		[Tooltip("The transform that will be rotated to indicate the current time.")]
		public Transform clockNeedle;

		// Token: 0x040017EB RID: 6123
		[FormerlySerializedAs("dialRotationAxis")]
		[Tooltip("The axis that the needle will rotate around.")]
		public Vector3 needleRotationAxis = Vector3.right;

		// Token: 0x040017EC RID: 6124
		private BetterDayNightManager dayNightManager;

		// Token: 0x040017ED RID: 6125
		[DebugOption]
		private float rotationDegree;

		// Token: 0x040017EE RID: 6126
		private string currentTimeOfDay;

		// Token: 0x040017EF RID: 6127
		private Quaternion initialRotation;
	}
}
