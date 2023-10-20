using System;
using System.Collections.Generic;
using AA;
using CjLib;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000290 RID: 656
	public class WaterCurrent : MonoBehaviour
	{
		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06001115 RID: 4373 RVA: 0x0005F30D File Offset: 0x0005D50D
		public float Speed
		{
			get
			{
				return this.currentSpeed;
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06001116 RID: 4374 RVA: 0x0005F315 File Offset: 0x0005D515
		public float Accel
		{
			get
			{
				return this.currentAccel;
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06001117 RID: 4375 RVA: 0x0005F31D File Offset: 0x0005D51D
		public float InwardSpeed
		{
			get
			{
				return this.inwardCurrentSpeed;
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06001118 RID: 4376 RVA: 0x0005F325 File Offset: 0x0005D525
		public float InwardAccel
		{
			get
			{
				return this.inwardCurrentAccel;
			}
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x0005F330 File Offset: 0x0005D530
		public bool GetCurrentAtPoint(Vector3 worldPoint, Vector3 startingVelocity, float dt, out Vector3 currentVelocity, out Vector3 velocityChange)
		{
			float num = (this.fullEffectDistance + this.fadeDistance) * (this.fullEffectDistance + this.fadeDistance);
			bool result = false;
			velocityChange = Vector3.zero;
			currentVelocity = Vector3.zero;
			float num2 = 0.0001f;
			float magnitude = startingVelocity.magnitude;
			if (magnitude > num2)
			{
				Vector3 a = startingVelocity / magnitude;
				float d = Spring.DamperDecayExact(magnitude, this.dampingHalfLife, dt, 1E-05f);
				Vector3 a2 = a * d;
				velocityChange += a2 - startingVelocity;
			}
			for (int i = 0; i < this.splines.Count; i++)
			{
				CatmullRomSpline catmullRomSpline = this.splines[i];
				Vector3 vector;
				float closestEvaluationOnSpline = catmullRomSpline.GetClosestEvaluationOnSpline(worldPoint, out vector);
				Vector3 a3 = catmullRomSpline.Evaluate(closestEvaluationOnSpline);
				Vector3 vector2 = a3 - worldPoint;
				if (vector2.sqrMagnitude < num)
				{
					result = true;
					float magnitude2 = vector2.magnitude;
					float num3 = (magnitude2 > this.fullEffectDistance) ? (1f - Mathf.Clamp01((magnitude2 - this.fullEffectDistance) / this.fadeDistance)) : 1f;
					float t = Mathf.Clamp01(closestEvaluationOnSpline + this.velocityAnticipationAdjustment);
					Vector3 forwardTangent = catmullRomSpline.GetForwardTangent(t, 0.01f);
					if (this.currentSpeed > num2 && Vector3.Dot(startingVelocity, forwardTangent) < num3 * this.currentSpeed)
					{
						velocityChange += forwardTangent * (this.currentAccel * dt);
					}
					else if (this.currentSpeed < num2 && Vector3.Dot(startingVelocity, forwardTangent) > num3 * this.currentSpeed)
					{
						velocityChange -= forwardTangent * (this.currentAccel * dt);
					}
					currentVelocity += forwardTangent * num3 * this.currentSpeed;
					float num4 = Mathf.InverseLerp(this.inwardCurrentNoEffectRadius, this.inwardCurrentFullEffectRadius, magnitude2);
					if (num4 > num2)
					{
						vector = Vector3.ProjectOnPlane(vector2, forwardTangent);
						Vector3 normalized = vector.normalized;
						if (this.inwardCurrentSpeed > num2 && Vector3.Dot(startingVelocity, normalized) < num4 * this.inwardCurrentSpeed)
						{
							velocityChange += normalized * (this.InwardAccel * dt);
						}
						else if (this.inwardCurrentSpeed < num2 && Vector3.Dot(startingVelocity, normalized) > num4 * this.inwardCurrentSpeed)
						{
							velocityChange -= normalized * (this.InwardAccel * dt);
						}
					}
					this.debugSplinePoint = a3;
				}
			}
			this.debugCurrentVelocity = velocityChange.normalized;
			return result;
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x0005F5E4 File Offset: 0x0005D7E4
		private void Update()
		{
			if (this.debugDrawCurrentQueries)
			{
				DebugUtil.DrawSphere(this.debugSplinePoint, 0.15f, 12, 12, Color.green, false, DebugUtil.Style.Wireframe);
				DebugUtil.DrawArrow(this.debugSplinePoint, this.debugSplinePoint + this.debugCurrentVelocity, 0.1f, 0.1f, 12, 0.1f, Color.yellow, false, DebugUtil.Style.Wireframe);
			}
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x0005F648 File Offset: 0x0005D848
		private void OnDrawGizmosSelected()
		{
			int num = 16;
			for (int i = 0; i < this.splines.Count; i++)
			{
				CatmullRomSpline catmullRomSpline = this.splines[i];
				Vector3 b = catmullRomSpline.Evaluate(0f);
				for (int j = 1; j <= num; j++)
				{
					float t = (float)j / (float)num;
					Vector3 vector = catmullRomSpline.Evaluate(t);
					vector - b;
					Quaternion rotation = Quaternion.LookRotation(catmullRomSpline.GetForwardTangent(t, 0.01f), Vector3.up);
					Gizmos.color = new Color(0f, 0.5f, 0.75f);
					this.DrawGizmoCircle(vector, rotation, this.fullEffectDistance);
					Gizmos.color = new Color(0f, 0.25f, 0.5f);
					this.DrawGizmoCircle(vector, rotation, this.fullEffectDistance + this.fadeDistance);
				}
			}
		}

		// Token: 0x0600111C RID: 4380 RVA: 0x0005F730 File Offset: 0x0005D930
		private void DrawGizmoCircle(Vector3 center, Quaternion rotation, float radius)
		{
			Vector3 point = Vector3.right * radius;
			int num = 16;
			for (int i = 1; i <= num; i++)
			{
				float f = (float)i / (float)num * 2f * 3.1415927f;
				Vector3 vector = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f) * radius;
				Gizmos.DrawLine(center + rotation * point, center + rotation * vector);
				point = vector;
			}
		}

		// Token: 0x04001376 RID: 4982
		[SerializeField]
		private List<CatmullRomSpline> splines = new List<CatmullRomSpline>();

		// Token: 0x04001377 RID: 4983
		[SerializeField]
		private float fullEffectDistance = 1f;

		// Token: 0x04001378 RID: 4984
		[SerializeField]
		private float fadeDistance = 0.5f;

		// Token: 0x04001379 RID: 4985
		[SerializeField]
		private float currentSpeed = 1f;

		// Token: 0x0400137A RID: 4986
		[SerializeField]
		private float currentAccel = 10f;

		// Token: 0x0400137B RID: 4987
		[SerializeField]
		private float velocityAnticipationAdjustment = 0.05f;

		// Token: 0x0400137C RID: 4988
		[SerializeField]
		private float inwardCurrentFullEffectRadius = 1f;

		// Token: 0x0400137D RID: 4989
		[SerializeField]
		private float inwardCurrentNoEffectRadius = 0.25f;

		// Token: 0x0400137E RID: 4990
		[SerializeField]
		private float inwardCurrentSpeed = 1f;

		// Token: 0x0400137F RID: 4991
		[SerializeField]
		private float inwardCurrentAccel = 10f;

		// Token: 0x04001380 RID: 4992
		[SerializeField]
		private float dampingHalfLife = 0.25f;

		// Token: 0x04001381 RID: 4993
		[SerializeField]
		private bool debugDrawCurrentQueries;

		// Token: 0x04001382 RID: 4994
		private Vector3 debugCurrentVelocity = Vector3.zero;

		// Token: 0x04001383 RID: 4995
		private Vector3 debugSplinePoint = Vector3.zero;
	}
}
