using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x020002A8 RID: 680
	public class GorillaVelocityTracker : MonoBehaviour
	{
		// Token: 0x060011A9 RID: 4521 RVA: 0x00064920 File Offset: 0x00062B20
		public void ResetState()
		{
			this.trans = base.transform;
			this.localSpaceData = new GorillaVelocityTracker.VelocityDataPoint[this.maxDataPoints];
			this.<ResetState>g__PopulateArray|10_0(this.localSpaceData);
			this.worldSpaceData = new GorillaVelocityTracker.VelocityDataPoint[this.maxDataPoints];
			this.<ResetState>g__PopulateArray|10_0(this.worldSpaceData);
			this.isRelativeTo = (this.relativeTo != null);
			this.lastLocalSpacePos = this.GetPosition(false);
			this.lastWorldSpacePos = this.GetPosition(true);
		}

		// Token: 0x060011AA RID: 4522 RVA: 0x0006499F File Offset: 0x00062B9F
		private void Awake()
		{
			this.ResetState();
		}

		// Token: 0x060011AB RID: 4523 RVA: 0x000649A7 File Offset: 0x00062BA7
		private void OnDisable()
		{
			this.ResetState();
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x000649AF File Offset: 0x00062BAF
		private Vector3 GetPosition(bool worldSpace)
		{
			if (worldSpace)
			{
				return this.trans.position;
			}
			if (this.isRelativeTo)
			{
				return this.relativeTo.InverseTransformPoint(this.trans.position);
			}
			return this.trans.localPosition;
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x000649EA File Offset: 0x00062BEA
		private void Update()
		{
			this.Tick();
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x000649F4 File Offset: 0x00062BF4
		public void Tick()
		{
			if (Time.frameCount <= this.lastTickedFrame)
			{
				return;
			}
			Vector3 position = this.GetPosition(false);
			Vector3 position2 = this.GetPosition(true);
			GorillaVelocityTracker.VelocityDataPoint velocityDataPoint = this.localSpaceData[this.currentDataPointIndex];
			velocityDataPoint.delta = (position - this.lastLocalSpacePos) / Time.deltaTime;
			velocityDataPoint.time = Time.time;
			this.localSpaceData[this.currentDataPointIndex] = velocityDataPoint;
			GorillaVelocityTracker.VelocityDataPoint velocityDataPoint2 = this.worldSpaceData[this.currentDataPointIndex];
			velocityDataPoint2.delta = (position2 - this.lastWorldSpacePos) / Time.deltaTime;
			velocityDataPoint2.time = Time.time;
			this.worldSpaceData[this.currentDataPointIndex] = velocityDataPoint2;
			this.lastLocalSpacePos = position;
			this.lastWorldSpacePos = position2;
			this.currentDataPointIndex++;
			if (this.currentDataPointIndex >= this.maxDataPoints)
			{
				this.currentDataPointIndex = 0;
			}
			this.lastTickedFrame = Time.frameCount;
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x00064AE1 File Offset: 0x00062CE1
		private void AddToQueue(ref List<GorillaVelocityTracker.VelocityDataPoint> dataPoints, GorillaVelocityTracker.VelocityDataPoint newData)
		{
			dataPoints.Add(newData);
			if (dataPoints.Count >= this.maxDataPoints)
			{
				dataPoints.RemoveAt(0);
			}
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x00064B04 File Offset: 0x00062D04
		public Vector3 GetAverageVelocity(bool worldSpace = false, float maxTimeFromPast = 0.15f, bool doMagnitudeCheck = false)
		{
			float num = maxTimeFromPast / 2f;
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array.Length <= 1)
			{
				return Vector3.zero;
			}
			GorillaVelocityTracker.<>c__DisplayClass17_0 CS$<>8__locals1;
			CS$<>8__locals1.total = Vector3.zero;
			CS$<>8__locals1.totalMag = 0f;
			CS$<>8__locals1.added = 0;
			float num2 = Time.time - maxTimeFromPast;
			float num3 = Time.time - num;
			int i = 0;
			int num4 = this.currentDataPointIndex;
			while (i < this.maxDataPoints)
			{
				GorillaVelocityTracker.VelocityDataPoint velocityDataPoint = array[num4];
				if (doMagnitudeCheck && CS$<>8__locals1.added > 1 && velocityDataPoint.time >= num3)
				{
					if (velocityDataPoint.delta.magnitude >= CS$<>8__locals1.totalMag / (float)CS$<>8__locals1.added)
					{
						GorillaVelocityTracker.<GetAverageVelocity>g__AddPoint|17_0(velocityDataPoint, ref CS$<>8__locals1);
					}
				}
				else if (velocityDataPoint.time >= num2)
				{
					GorillaVelocityTracker.<GetAverageVelocity>g__AddPoint|17_0(velocityDataPoint, ref CS$<>8__locals1);
				}
				num4++;
				if (num4 >= this.maxDataPoints)
				{
					num4 = 0;
				}
				i++;
			}
			if (CS$<>8__locals1.added > 0)
			{
				return CS$<>8__locals1.total / (float)CS$<>8__locals1.added;
			}
			return Vector3.zero;
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x00064C14 File Offset: 0x00062E14
		public Vector3 GetLatestVelocity(bool worldSpace = false)
		{
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			return array[this.currentDataPointIndex].delta;
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x00064C44 File Offset: 0x00062E44
		public float GetAverageSpeedChangeMagnitudeInDirection(Vector3 dir, bool worldSpace = false, float maxTimeFromPast = 0.05f)
		{
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array.Length <= 1)
			{
				return 0f;
			}
			float num = 0f;
			int num2 = 0;
			float num3 = Time.time - maxTimeFromPast;
			bool flag = false;
			Vector3 b = Vector3.zero;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].time >= num3)
				{
					if (!flag)
					{
						b = array[i].delta;
						flag = true;
					}
					else
					{
						num += Mathf.Abs(Vector3.Dot(array[i].delta - b, dir));
						num2++;
					}
				}
			}
			if (num2 <= 0)
			{
				return 0f;
			}
			return num / (float)num2;
		}

		// Token: 0x060011B4 RID: 4532 RVA: 0x00064D04 File Offset: 0x00062F04
		[CompilerGenerated]
		private void <ResetState>g__PopulateArray|10_0(GorillaVelocityTracker.VelocityDataPoint[] array)
		{
			for (int i = 0; i < this.maxDataPoints; i++)
			{
				array[i] = new GorillaVelocityTracker.VelocityDataPoint();
			}
		}

		// Token: 0x060011B5 RID: 4533 RVA: 0x00064D2C File Offset: 0x00062F2C
		[CompilerGenerated]
		internal static void <GetAverageVelocity>g__AddPoint|17_0(GorillaVelocityTracker.VelocityDataPoint point, ref GorillaVelocityTracker.<>c__DisplayClass17_0 A_1)
		{
			A_1.total += point.delta;
			A_1.totalMag += point.delta.magnitude;
			int added = A_1.added;
			A_1.added = added + 1;
		}

		// Token: 0x04001468 RID: 5224
		[SerializeField]
		private int maxDataPoints = 20;

		// Token: 0x04001469 RID: 5225
		[SerializeField]
		private Transform relativeTo;

		// Token: 0x0400146A RID: 5226
		private int currentDataPointIndex;

		// Token: 0x0400146B RID: 5227
		private GorillaVelocityTracker.VelocityDataPoint[] localSpaceData;

		// Token: 0x0400146C RID: 5228
		private GorillaVelocityTracker.VelocityDataPoint[] worldSpaceData;

		// Token: 0x0400146D RID: 5229
		private Transform trans;

		// Token: 0x0400146E RID: 5230
		private Vector3 lastWorldSpacePos;

		// Token: 0x0400146F RID: 5231
		private Vector3 lastLocalSpacePos;

		// Token: 0x04001470 RID: 5232
		private bool isRelativeTo;

		// Token: 0x04001471 RID: 5233
		private int lastTickedFrame = -1;

		// Token: 0x020004AF RID: 1199
		public class VelocityDataPoint
		{
			// Token: 0x04001F73 RID: 8051
			public Vector3 delta;

			// Token: 0x04001F74 RID: 8052
			public float time = -1f;
		}
	}
}
