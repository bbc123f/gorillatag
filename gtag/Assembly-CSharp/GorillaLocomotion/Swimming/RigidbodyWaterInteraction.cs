using System;
using System.Collections.Generic;
using AA;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x0200028B RID: 651
	[RequireComponent(typeof(Rigidbody))]
	public class RigidbodyWaterInteraction : MonoBehaviour
	{
		// Token: 0x060010F9 RID: 4345 RVA: 0x0005DA26 File Offset: 0x0005BC26
		protected void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.baseAngularDrag = this.rb.angularDrag;
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x0005DA45 File Offset: 0x0005BC45
		protected void OnEnable()
		{
			this.overlappingWaterVolumes.Clear();
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x0005DA52 File Offset: 0x0005BC52
		protected void OnDisable()
		{
			this.overlappingWaterVolumes.Clear();
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x0005DA60 File Offset: 0x0005BC60
		protected void FixedUpdate()
		{
			bool flag = this.overlappingWaterVolumes.Count > 0;
			WaterVolume.SurfaceQuery surfaceQuery = default(WaterVolume.SurfaceQuery);
			float num = float.MinValue;
			if (flag && this.enablePreciseWaterCollision)
			{
				Vector3 vector = base.transform.position + Vector3.down * 2f * this.objectRadiusForWaterCollision * this.buoyancyEquilibrium;
				bool flag2 = false;
				this.activeWaterCurrents.Clear();
				for (int i = 0; i < this.overlappingWaterVolumes.Count; i++)
				{
					WaterVolume.SurfaceQuery surfaceQuery2;
					if (this.overlappingWaterVolumes[i].GetSurfaceQueryForPoint(vector, out surfaceQuery2, false))
					{
						float num2 = Vector3.Dot(surfaceQuery2.surfacePoint - vector, surfaceQuery2.surfaceNormal);
						if (num2 > num)
						{
							num = num2;
							surfaceQuery = surfaceQuery2;
							flag2 = true;
						}
						WaterCurrent waterCurrent = this.overlappingWaterVolumes[i].Current;
						if (this.applyWaterCurrents && waterCurrent != null && num2 > 0f && !this.activeWaterCurrents.Contains(waterCurrent))
						{
							this.activeWaterCurrents.Add(waterCurrent);
						}
					}
				}
				if (flag2)
				{
					bool flag3 = num > -(1f - this.buoyancyEquilibrium) * 2f * this.objectRadiusForWaterCollision;
					float num3 = this.enablePreciseWaterCollision ? this.objectRadiusForWaterCollision : 0f;
					bool flag4 = base.transform.position.y + num3 - (surfaceQuery.surfacePoint.y - surfaceQuery.maxDepth) > 0f;
					flag = (flag3 && flag4);
				}
				else
				{
					flag = false;
				}
			}
			if (flag)
			{
				float fixedDeltaTime = Time.fixedDeltaTime;
				Vector3 vector2 = this.rb.velocity;
				Vector3 vector3 = Vector3.zero;
				if (this.applyWaterCurrents)
				{
					Vector3 a = Vector3.zero;
					for (int j = 0; j < this.activeWaterCurrents.Count; j++)
					{
						WaterCurrent waterCurrent2 = this.activeWaterCurrents[j];
						Vector3 startingVelocity = vector2 + vector3;
						Vector3 b;
						Vector3 b2;
						if (waterCurrent2.GetCurrentAtPoint(base.transform.position, startingVelocity, fixedDeltaTime, out b, out b2))
						{
							a += b;
							vector3 += b2;
						}
					}
					if (this.enablePreciseWaterCollision)
					{
						Vector3 position = (surfaceQuery.surfacePoint + (base.transform.position + Vector3.down * this.objectRadiusForWaterCollision)) * 0.5f;
						this.rb.AddForceAtPosition(vector3, position, ForceMode.VelocityChange);
					}
					else
					{
						vector2 += vector3;
					}
				}
				if (this.applyBuoyancyForce)
				{
					Vector3 vector4 = Vector3.zero;
					if (this.enablePreciseWaterCollision)
					{
						float b3 = 2f * this.objectRadiusForWaterCollision * this.buoyancyEquilibrium;
						float d = Mathf.InverseLerp(0f, b3, num);
						vector4 = -Physics.gravity * this.underWaterBuoyancyFactor * d * fixedDeltaTime;
					}
					else
					{
						vector4 = -Physics.gravity * this.underWaterBuoyancyFactor * fixedDeltaTime;
					}
					if (vector3.sqrMagnitude > 0.001f)
					{
						float magnitude = vector3.magnitude;
						Vector3 vector5 = vector3 / magnitude;
						float num4 = Vector3.Dot(vector4, vector5);
						if (num4 < 0f)
						{
							vector4 -= num4 * vector5;
						}
					}
					vector2 += vector4;
				}
				float magnitude2 = vector2.magnitude;
				if (magnitude2 > 0.001f && this.applyDamping)
				{
					Vector3 a2 = vector2 / magnitude2;
					float num5 = Spring.DamperDecayExact(magnitude2, this.underWaterDampingHalfLife, fixedDeltaTime, 1E-05f);
					if (this.enablePreciseWaterCollision)
					{
						float a3 = Spring.DamperDecayExact(magnitude2, this.waterSurfaceDampingHalfLife, fixedDeltaTime, 1E-05f);
						float t = Mathf.Clamp(-(base.transform.position.y - surfaceQuery.surfacePoint.y) / this.objectRadiusForWaterCollision, -1f, 1f) * 0.5f + 0.5f;
						vector2 = Mathf.Lerp(a3, num5, t) * a2;
					}
					else
					{
						vector2 = num5 * a2;
					}
				}
				if (this.applySurfaceTorque && this.enablePreciseWaterCollision)
				{
					float num6 = base.transform.position.y - surfaceQuery.surfacePoint.y;
					if (num6 < this.objectRadiusForWaterCollision && num6 > 0f)
					{
						Vector3 rhs = vector2 - Vector3.Dot(vector2, surfaceQuery.surfaceNormal) * surfaceQuery.surfaceNormal;
						Vector3 normalized = Vector3.Cross(surfaceQuery.surfaceNormal, rhs).normalized;
						float num7 = Vector3.Dot(this.rb.angularVelocity, normalized);
						float num8 = rhs.magnitude / this.objectRadiusForWaterCollision - num7;
						if (num8 > 0f)
						{
							this.rb.AddTorque(this.surfaceTorqueAmount * num8 * normalized, ForceMode.Acceleration);
						}
					}
				}
				this.rb.velocity = vector2;
				this.rb.angularDrag = this.angularDrag;
				return;
			}
			this.rb.angularDrag = this.baseAngularDrag;
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x0005DF80 File Offset: 0x0005C180
		protected void OnTriggerEnter(Collider other)
		{
			WaterVolume component = other.GetComponent<WaterVolume>();
			if (component != null && !this.overlappingWaterVolumes.Contains(component))
			{
				this.overlappingWaterVolumes.Add(component);
			}
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x0005DFB8 File Offset: 0x0005C1B8
		protected void OnTriggerExit(Collider other)
		{
			WaterVolume component = other.GetComponent<WaterVolume>();
			if (component != null && this.overlappingWaterVolumes.Contains(component))
			{
				this.overlappingWaterVolumes.Remove(component);
			}
		}

		// Token: 0x0400133F RID: 4927
		public bool applyDamping = true;

		// Token: 0x04001340 RID: 4928
		public bool applyBuoyancyForce = true;

		// Token: 0x04001341 RID: 4929
		public bool applyAngularDrag = true;

		// Token: 0x04001342 RID: 4930
		public bool applyWaterCurrents = true;

		// Token: 0x04001343 RID: 4931
		public bool applySurfaceTorque = true;

		// Token: 0x04001344 RID: 4932
		public float underWaterDampingHalfLife = 0.25f;

		// Token: 0x04001345 RID: 4933
		public float waterSurfaceDampingHalfLife = 1f;

		// Token: 0x04001346 RID: 4934
		public float underWaterBuoyancyFactor = 0.5f;

		// Token: 0x04001347 RID: 4935
		public float angularDrag = 0.5f;

		// Token: 0x04001348 RID: 4936
		public float surfaceTorqueAmount = 0.5f;

		// Token: 0x04001349 RID: 4937
		public bool enablePreciseWaterCollision;

		// Token: 0x0400134A RID: 4938
		public float objectRadiusForWaterCollision = 0.25f;

		// Token: 0x0400134B RID: 4939
		[Range(0f, 1f)]
		public float buoyancyEquilibrium = 0.8f;

		// Token: 0x0400134C RID: 4940
		private Rigidbody rb;

		// Token: 0x0400134D RID: 4941
		private List<WaterVolume> overlappingWaterVolumes = new List<WaterVolume>();

		// Token: 0x0400134E RID: 4942
		private List<WaterCurrent> activeWaterCurrents = new List<WaterCurrent>(16);

		// Token: 0x0400134F RID: 4943
		private float baseAngularDrag = 0.05f;
	}
}
