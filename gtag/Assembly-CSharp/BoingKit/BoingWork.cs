﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200036F RID: 879
	public static class BoingWork
	{
		// Token: 0x060019E5 RID: 6629 RVA: 0x00090B48 File Offset: 0x0008ED48
		internal static Vector3 ComputeTranslationalResults(Transform t, Vector3 src, Vector3 dst, BoingBehavior b)
		{
			if (!b.LockTranslationX && !b.LockTranslationY && !b.LockTranslationZ)
			{
				return dst;
			}
			Vector3 vector = dst - src;
			BoingManager.TranslationLockSpace translationLockSpace = b.TranslationLockSpace;
			if (translationLockSpace != BoingManager.TranslationLockSpace.Global)
			{
				if (translationLockSpace == BoingManager.TranslationLockSpace.Local)
				{
					if (b.LockTranslationX)
					{
						vector -= Vector3.Project(vector, t.right);
					}
					if (b.LockTranslationY)
					{
						vector -= Vector3.Project(vector, t.up);
					}
					if (b.LockTranslationZ)
					{
						vector -= Vector3.Project(vector, t.forward);
					}
				}
			}
			else
			{
				if (b.LockTranslationX)
				{
					vector.x = 0f;
				}
				if (b.LockTranslationY)
				{
					vector.y = 0f;
				}
				if (b.LockTranslationZ)
				{
					vector.z = 0f;
				}
			}
			return src + vector;
		}

		// Token: 0x02000537 RID: 1335
		public enum EffectorFlags
		{
			// Token: 0x040021DE RID: 8670
			ContinuousMotion
		}

		// Token: 0x02000538 RID: 1336
		public enum ReactorFlags
		{
			// Token: 0x040021E0 RID: 8672
			TwoDDistanceCheck,
			// Token: 0x040021E1 RID: 8673
			TwoDPositionInfluence,
			// Token: 0x040021E2 RID: 8674
			TwoDRotationInfluence,
			// Token: 0x040021E3 RID: 8675
			EnablePositionEffect,
			// Token: 0x040021E4 RID: 8676
			EnableRotationEffect,
			// Token: 0x040021E5 RID: 8677
			EnableScaleEffect,
			// Token: 0x040021E6 RID: 8678
			GlobalReactionUpVector,
			// Token: 0x040021E7 RID: 8679
			EnablePropagation,
			// Token: 0x040021E8 RID: 8680
			AnchorPropagationAtBorder,
			// Token: 0x040021E9 RID: 8681
			FixedUpdate,
			// Token: 0x040021EA RID: 8682
			EarlyUpdate,
			// Token: 0x040021EB RID: 8683
			LateUpdate
		}

		// Token: 0x02000539 RID: 1337
		[Serializable]
		public struct Params
		{
			// Token: 0x06001FAB RID: 8107 RVA: 0x000A2590 File Offset: 0x000A0790
			public static void Copy(ref BoingWork.Params from, ref BoingWork.Params to)
			{
				to.PositionParameterMode = from.PositionParameterMode;
				to.RotationParameterMode = from.RotationParameterMode;
				to.PositionExponentialHalfLife = from.PositionExponentialHalfLife;
				to.PositionOscillationHalfLife = from.PositionOscillationHalfLife;
				to.PositionOscillationFrequency = from.PositionOscillationFrequency;
				to.PositionOscillationDampingRatio = from.PositionOscillationDampingRatio;
				to.MoveReactionMultiplier = from.MoveReactionMultiplier;
				to.LinearImpulseMultiplier = from.LinearImpulseMultiplier;
				to.RotationExponentialHalfLife = from.RotationExponentialHalfLife;
				to.RotationOscillationHalfLife = from.RotationOscillationHalfLife;
				to.RotationOscillationFrequency = from.RotationOscillationFrequency;
				to.RotationOscillationDampingRatio = from.RotationOscillationDampingRatio;
				to.RotationReactionMultiplier = from.RotationReactionMultiplier;
				to.AngularImpulseMultiplier = from.AngularImpulseMultiplier;
				to.ScaleExponentialHalfLife = from.ScaleExponentialHalfLife;
				to.ScaleOscillationHalfLife = from.ScaleOscillationHalfLife;
				to.ScaleOscillationFrequency = from.ScaleOscillationFrequency;
				to.ScaleOscillationDampingRatio = from.ScaleOscillationDampingRatio;
			}

			// Token: 0x06001FAC RID: 8108 RVA: 0x000A2678 File Offset: 0x000A0878
			public void Init()
			{
				this.InstanceID = -1;
				this.Bits.Clear();
				this.TwoDPlane = TwoDPlaneEnum.XZ;
				this.PositionParameterMode = ParameterMode.OscillationByHalfLife;
				this.RotationParameterMode = ParameterMode.OscillationByHalfLife;
				this.ScaleParameterMode = ParameterMode.OscillationByHalfLife;
				this.PositionExponentialHalfLife = 0.02f;
				this.PositionOscillationHalfLife = 0.1f;
				this.PositionOscillationFrequency = 5f;
				this.PositionOscillationDampingRatio = 0.5f;
				this.MoveReactionMultiplier = 1f;
				this.LinearImpulseMultiplier = 1f;
				this.RotationExponentialHalfLife = 0.02f;
				this.RotationOscillationHalfLife = 0.1f;
				this.RotationOscillationFrequency = 5f;
				this.RotationOscillationDampingRatio = 0.5f;
				this.RotationReactionMultiplier = 1f;
				this.AngularImpulseMultiplier = 1f;
				this.ScaleExponentialHalfLife = 0.02f;
				this.ScaleOscillationHalfLife = 0.1f;
				this.ScaleOscillationFrequency = 5f;
				this.ScaleOscillationDampingRatio = 0.5f;
				this.Instance.Reset();
			}

			// Token: 0x06001FAD RID: 8109 RVA: 0x000A276E File Offset: 0x000A096E
			public void AccumulateTarget(ref BoingEffector.Params effector, float dt)
			{
				this.Instance.AccumulateTarget(ref this, ref effector, dt);
			}

			// Token: 0x06001FAE RID: 8110 RVA: 0x000A277E File Offset: 0x000A097E
			public void EndAccumulateTargets()
			{
				this.Instance.EndAccumulateTargets(ref this);
			}

			// Token: 0x06001FAF RID: 8111 RVA: 0x000A278C File Offset: 0x000A098C
			public void Execute(float dt)
			{
				this.Instance.Execute(ref this, dt);
			}

			// Token: 0x06001FB0 RID: 8112 RVA: 0x000A279C File Offset: 0x000A099C
			public void Execute(BoingBones bones, float dt)
			{
				float maxLen = bones.MaxCollisionResolutionSpeed * dt;
				for (int i = 0; i < bones.BoneData.Length; i++)
				{
					BoingBones.Chain chain = bones.BoneChains[i];
					BoingBones.Bone[] array = bones.BoneData[i];
					if (array != null)
					{
						foreach (BoingBones.Bone bone in array)
						{
							if (chain.ParamsOverride == null)
							{
								bone.Instance.Execute(ref bones.Params, dt);
							}
							else
							{
								bone.Instance.Execute(ref chain.ParamsOverride.Params, dt);
							}
						}
						BoingBones.Bone bone2 = array[0];
						bone2.ScaleWs = (bone2.BlendedScaleLs = bone2.CachedScaleLs);
						bone2.UpdateBounds();
						chain.Bounds = bone2.Bounds;
						Vector3 position = bone2.Transform.position;
						for (int k = 1; k < array.Length; k++)
						{
							BoingBones.Bone bone3 = array[k];
							BoingBones.Bone bone4 = array[bone3.ParentIndex];
							Vector3 v = bone4.Instance.PositionSpring.Value - bone3.Instance.PositionSpring.Value;
							Vector3 vector = VectorUtil.NormalizeSafe(v, Vector3.zero);
							float magnitude = v.magnitude;
							float num = magnitude - bone3.FullyStiffToParentLength;
							float d = bone3.LengthStiffnessT * num;
							BoingBones.Bone bone5 = bone3;
							bone5.Instance.PositionSpring.Value = bone5.Instance.PositionSpring.Value + d * vector;
							Vector3 a = Vector3.Project(bone3.Instance.PositionSpring.Velocity, vector);
							BoingBones.Bone bone6 = bone3;
							bone6.Instance.PositionSpring.Velocity = bone6.Instance.PositionSpring.Velocity - bone3.LengthStiffnessT * a;
							if (bone3.BendAngleCap < MathUtil.Pi - MathUtil.Epsilon)
							{
								Vector3 position2 = bone3.Transform.position;
								Vector3 vector2 = bone3.Instance.PositionSpring.Value - position;
								vector2 = VectorUtil.ClampBend(vector2, position2 - position, bone3.BendAngleCap);
								bone3.Instance.PositionSpring.Value = position + vector2;
							}
							if (bone3.SquashAndStretch > 0f)
							{
								float num2 = magnitude * MathUtil.InvSafe(bone3.FullyStiffToParentLength);
								Vector3 b = VectorUtil.ComponentWiseDivSafe(Mathf.Clamp(Mathf.Sqrt(1f / num2), 1f / Mathf.Max(1f, chain.MaxStretch), Mathf.Max(1f, chain.MaxSquash)) * Vector3.one, bone4.ScaleWs);
								bone3.BlendedScaleLs = Vector3.Lerp(Vector3.Lerp(bone3.CachedScaleLs, b, bone3.SquashAndStretch), bone3.CachedScaleLs, bone3.AnimationBlend);
							}
							else
							{
								bone3.BlendedScaleLs = bone3.CachedScaleLs;
							}
							bone3.ScaleWs = VectorUtil.ComponentWiseMult(bone4.ScaleWs, bone3.BlendedScaleLs);
							bone3.UpdateBounds();
							chain.Bounds.Encapsulate(bone3.Bounds);
						}
						chain.Bounds.Expand(0.2f * Vector3.one);
						if (chain.EnableBoingKitCollision)
						{
							foreach (BoingBoneCollider boingBoneCollider in bones.BoingColliders)
							{
								if (!(boingBoneCollider == null) && chain.Bounds.Intersects(boingBoneCollider.Bounds))
								{
									foreach (BoingBones.Bone bone7 in array)
									{
										Vector3 vector3;
										if (bone7.Bounds.Intersects(boingBoneCollider.Bounds) && boingBoneCollider.Collide(bone7.Instance.PositionSpring.Value, bones.MinScale * bone7.CollisionRadius, out vector3))
										{
											BoingBones.Bone bone8 = bone7;
											bone8.Instance.PositionSpring.Value = bone8.Instance.PositionSpring.Value + VectorUtil.ClampLength(vector3, 0f, maxLen);
											BoingBones.Bone bone9 = bone7;
											bone9.Instance.PositionSpring.Velocity = bone9.Instance.PositionSpring.Velocity - Vector3.Project(bone7.Instance.PositionSpring.Velocity, vector3);
										}
									}
								}
							}
						}
						SphereCollider sharedSphereCollider = BoingManager.SharedSphereCollider;
						if (chain.EnableUnityCollision && sharedSphereCollider != null)
						{
							sharedSphereCollider.enabled = true;
							foreach (Collider collider in bones.UnityColliders)
							{
								if (!(collider == null) && chain.Bounds.Intersects(collider.bounds))
								{
									foreach (BoingBones.Bone bone10 in array)
									{
										if (bone10.Bounds.Intersects(collider.bounds))
										{
											sharedSphereCollider.center = bone10.Instance.PositionSpring.Value;
											sharedSphereCollider.radius = bone10.CollisionRadius;
											Vector3 vector4;
											float d2;
											if (Physics.ComputePenetration(sharedSphereCollider, Vector3.zero, Quaternion.identity, collider, collider.transform.position, collider.transform.rotation, out vector4, out d2))
											{
												BoingBones.Bone bone11 = bone10;
												bone11.Instance.PositionSpring.Value = bone11.Instance.PositionSpring.Value + VectorUtil.ClampLength(vector4 * d2, 0f, maxLen);
												BoingBones.Bone bone12 = bone10;
												bone12.Instance.PositionSpring.Velocity = bone12.Instance.PositionSpring.Velocity - Vector3.Project(bone10.Instance.PositionSpring.Velocity, vector4);
											}
										}
									}
								}
							}
							sharedSphereCollider.enabled = false;
						}
						if (chain.EnableInterChainCollision)
						{
							foreach (BoingBones.Bone bone13 in array)
							{
								for (int n = i + 1; n < bones.BoneData.Length; n++)
								{
									BoingBones.Chain chain2 = bones.BoneChains[n];
									BoingBones.Bone[] array3 = bones.BoneData[n];
									if (array3 != null && chain2.EnableInterChainCollision && chain.Bounds.Intersects(chain2.Bounds))
									{
										foreach (BoingBones.Bone bone14 in array3)
										{
											Vector3 vector5;
											if (Collision.SphereSphere(bone13.Instance.PositionSpring.Value, bones.MinScale * bone13.CollisionRadius, bone14.Instance.PositionSpring.Value, bones.MinScale * bone14.CollisionRadius, out vector5))
											{
												vector5 = VectorUtil.ClampLength(vector5, 0f, maxLen);
												float num3 = bone14.CollisionRadius * MathUtil.InvSafe(bone13.CollisionRadius + bone14.CollisionRadius);
												BoingBones.Bone bone15 = bone13;
												bone15.Instance.PositionSpring.Value = bone15.Instance.PositionSpring.Value + num3 * vector5;
												BoingBones.Bone bone16 = bone14;
												bone16.Instance.PositionSpring.Value = bone16.Instance.PositionSpring.Value - (1f - num3) * vector5;
												BoingBones.Bone bone17 = bone13;
												bone17.Instance.PositionSpring.Velocity = bone17.Instance.PositionSpring.Velocity - Vector3.Project(bone13.Instance.PositionSpring.Velocity, vector5);
												BoingBones.Bone bone18 = bone14;
												bone18.Instance.PositionSpring.Velocity = bone18.Instance.PositionSpring.Velocity - Vector3.Project(bone14.Instance.PositionSpring.Velocity, vector5);
											}
										}
									}
								}
							}
						}
					}
				}
			}

			// Token: 0x06001FB1 RID: 8113 RVA: 0x000A2F79 File Offset: 0x000A1179
			public void PullResults(BoingBones bones)
			{
				this.Instance.PullResults(bones);
			}

			// Token: 0x06001FB2 RID: 8114 RVA: 0x000A2F87 File Offset: 0x000A1187
			private void SuppressWarnings()
			{
				this.m_padding0 = 0;
				this.m_padding1 = 0;
				this.m_padding2 = 0f;
				this.m_padding0 = this.m_padding1;
				this.m_padding1 = this.m_padding0;
				this.m_padding2 = (float)this.m_padding0;
			}

			// Token: 0x040021EC RID: 8684
			public static readonly int Stride = 112 + BoingWork.Params.InstanceData.Stride;

			// Token: 0x040021ED RID: 8685
			public int InstanceID;

			// Token: 0x040021EE RID: 8686
			public Bits32 Bits;

			// Token: 0x040021EF RID: 8687
			public TwoDPlaneEnum TwoDPlane;

			// Token: 0x040021F0 RID: 8688
			private int m_padding0;

			// Token: 0x040021F1 RID: 8689
			public ParameterMode PositionParameterMode;

			// Token: 0x040021F2 RID: 8690
			public ParameterMode RotationParameterMode;

			// Token: 0x040021F3 RID: 8691
			public ParameterMode ScaleParameterMode;

			// Token: 0x040021F4 RID: 8692
			private int m_padding1;

			// Token: 0x040021F5 RID: 8693
			[Range(0f, 5f)]
			public float PositionExponentialHalfLife;

			// Token: 0x040021F6 RID: 8694
			[Range(0f, 5f)]
			public float PositionOscillationHalfLife;

			// Token: 0x040021F7 RID: 8695
			[Range(0f, 10f)]
			public float PositionOscillationFrequency;

			// Token: 0x040021F8 RID: 8696
			[Range(0f, 1f)]
			public float PositionOscillationDampingRatio;

			// Token: 0x040021F9 RID: 8697
			[Range(0f, 10f)]
			public float MoveReactionMultiplier;

			// Token: 0x040021FA RID: 8698
			[Range(0f, 10f)]
			public float LinearImpulseMultiplier;

			// Token: 0x040021FB RID: 8699
			[Range(0f, 5f)]
			public float RotationExponentialHalfLife;

			// Token: 0x040021FC RID: 8700
			[Range(0f, 5f)]
			public float RotationOscillationHalfLife;

			// Token: 0x040021FD RID: 8701
			[Range(0f, 10f)]
			public float RotationOscillationFrequency;

			// Token: 0x040021FE RID: 8702
			[Range(0f, 1f)]
			public float RotationOscillationDampingRatio;

			// Token: 0x040021FF RID: 8703
			[Range(0f, 10f)]
			public float RotationReactionMultiplier;

			// Token: 0x04002200 RID: 8704
			[Range(0f, 10f)]
			public float AngularImpulseMultiplier;

			// Token: 0x04002201 RID: 8705
			[Range(0f, 5f)]
			public float ScaleExponentialHalfLife;

			// Token: 0x04002202 RID: 8706
			[Range(0f, 5f)]
			public float ScaleOscillationHalfLife;

			// Token: 0x04002203 RID: 8707
			[Range(0f, 10f)]
			public float ScaleOscillationFrequency;

			// Token: 0x04002204 RID: 8708
			[Range(0f, 1f)]
			public float ScaleOscillationDampingRatio;

			// Token: 0x04002205 RID: 8709
			public Vector3 RotationReactionUp;

			// Token: 0x04002206 RID: 8710
			private float m_padding2;

			// Token: 0x04002207 RID: 8711
			public BoingWork.Params.InstanceData Instance;

			// Token: 0x02000552 RID: 1362
			public struct InstanceData
			{
				// Token: 0x06001FCC RID: 8140 RVA: 0x000A32C0 File Offset: 0x000A14C0
				public void Reset()
				{
					this.PositionSpring.Reset();
					this.RotationSpring.Reset();
					this.ScaleSpring.Reset(Vector3.one, Vector3.zero);
					this.PositionPropagationWorkData = Vector3.zero;
					this.RotationPropagationWorkData = Vector3.zero;
				}

				// Token: 0x06001FCD RID: 8141 RVA: 0x000A3314 File Offset: 0x000A1514
				public void Reset(Vector3 position, bool instantAccumulation)
				{
					this.PositionSpring.Reset(position);
					this.RotationSpring.Reset();
					this.ScaleSpring.Reset(Vector3.one, Vector3.zero);
					this.PositionPropagationWorkData = Vector3.zero;
					this.RotationPropagationWorkData = Vector3.zero;
					this.m_instantAccumulation = (instantAccumulation ? 1 : 0);
				}

				// Token: 0x06001FCE RID: 8142 RVA: 0x000A3378 File Offset: 0x000A1578
				public void PrepareExecute(ref BoingWork.Params p, Vector3 position, Quaternion rotation, Vector3 scale, bool accumulateEffectors)
				{
					this.PositionOrigin = position;
					this.PositionTarget = position;
					this.RotationTarget = (this.RotationOrigin = QuaternionUtil.ToVector4(rotation));
					this.ScaleTarget = scale;
					this.m_minScale = VectorUtil.MinComponent(scale);
					if (accumulateEffectors)
					{
						this.PositionTarget = Vector3.zero;
						this.RotationTarget = Vector4.zero;
						this.m_numEffectors = 0;
						this.m_upWs = (p.Bits.IsBitSet(6) ? p.RotationReactionUp : (rotation * VectorUtil.NormalizeSafe(p.RotationReactionUp, Vector3.up)));
						return;
					}
					this.m_numEffectors = -1;
					this.m_upWs = Vector3.zero;
				}

				// Token: 0x06001FCF RID: 8143 RVA: 0x000A3434 File Offset: 0x000A1634
				public void PrepareExecute(ref BoingWork.Params p, Vector3 gridCenter, Quaternion gridRotation, Vector3 cellOffset)
				{
					this.PositionOrigin = gridCenter + cellOffset;
					this.RotationOrigin = QuaternionUtil.ToVector4(Quaternion.identity);
					this.PositionTarget = Vector3.zero;
					this.RotationTarget = Vector4.zero;
					this.m_numEffectors = 0;
					this.m_upWs = (p.Bits.IsBitSet(6) ? p.RotationReactionUp : (gridRotation * VectorUtil.NormalizeSafe(p.RotationReactionUp, Vector3.up)));
					this.m_minScale = 1f;
				}

				// Token: 0x06001FD0 RID: 8144 RVA: 0x000A34C8 File Offset: 0x000A16C8
				public void AccumulateTarget(ref BoingWork.Params p, ref BoingEffector.Params effector, float dt)
				{
					Vector3 b = effector.Bits.IsBitSet(0) ? VectorUtil.GetClosestPointOnSegment(this.PositionOrigin, effector.PrevPosition, effector.CurrPosition) : effector.CurrPosition;
					Vector3 vector = this.PositionOrigin - b;
					Vector3 v = vector;
					if (p.Bits.IsBitSet(0))
					{
						switch (p.TwoDPlane)
						{
						case TwoDPlaneEnum.XY:
							vector.z = 0f;
							break;
						case TwoDPlaneEnum.XZ:
							vector.y = 0f;
							break;
						case TwoDPlaneEnum.YZ:
							vector.x = 0f;
							break;
						}
					}
					if (Mathf.Abs(vector.x) > effector.Radius || Mathf.Abs(vector.y) > effector.Radius || Mathf.Abs(vector.z) > effector.Radius || vector.sqrMagnitude > effector.Radius * effector.Radius)
					{
						return;
					}
					float magnitude = vector.magnitude;
					float num = (effector.Radius - effector.FullEffectRadius > MathUtil.Epsilon) ? (1f - Mathf.Clamp01((magnitude - effector.FullEffectRadius) / (effector.Radius - effector.FullEffectRadius))) : 1f;
					Vector3 v2 = this.m_upWs;
					Vector3 vector2 = this.m_upWs;
					Vector3 vector3 = VectorUtil.NormalizeSafe(v, this.m_upWs);
					Vector3 vector4 = vector3;
					if (p.Bits.IsBitSet(1))
					{
						switch (p.TwoDPlane)
						{
						case TwoDPlaneEnum.XY:
							vector3.z = 0f;
							v2.z = 0f;
							break;
						case TwoDPlaneEnum.XZ:
							vector3.y = 0f;
							v2.y = 0f;
							break;
						case TwoDPlaneEnum.YZ:
							vector3.x = 0f;
							v2.x = 0f;
							break;
						}
						if (v2.sqrMagnitude < MathUtil.Epsilon)
						{
							switch (p.TwoDPlane)
							{
							case TwoDPlaneEnum.XY:
								v2 = Vector3.up;
								break;
							case TwoDPlaneEnum.XZ:
								v2 = Vector3.forward;
								break;
							case TwoDPlaneEnum.YZ:
								v2 = Vector3.up;
								break;
							}
						}
						else
						{
							v2.Normalize();
						}
						vector3 = VectorUtil.NormalizeSafe(vector3, v2);
					}
					if (p.Bits.IsBitSet(2))
					{
						switch (p.TwoDPlane)
						{
						case TwoDPlaneEnum.XY:
							vector4.z = 0f;
							vector2.z = 0f;
							break;
						case TwoDPlaneEnum.XZ:
							vector4.y = 0f;
							vector2.y = 0f;
							break;
						case TwoDPlaneEnum.YZ:
							vector4.x = 0f;
							vector2.x = 0f;
							break;
						}
						if (vector2.sqrMagnitude < MathUtil.Epsilon)
						{
							switch (p.TwoDPlane)
							{
							case TwoDPlaneEnum.XY:
								vector2 = Vector3.up;
								break;
							case TwoDPlaneEnum.XZ:
								vector2 = Vector3.forward;
								break;
							case TwoDPlaneEnum.YZ:
								vector2 = Vector3.up;
								break;
							}
						}
						else
						{
							vector2.Normalize();
						}
						vector4 = VectorUtil.NormalizeSafe(vector4, vector2);
					}
					if (p.Bits.IsBitSet(3))
					{
						Vector3 b2 = num * p.MoveReactionMultiplier * effector.MoveDistance * vector3;
						this.PositionTarget += b2;
						this.PositionSpring.Velocity = this.PositionSpring.Velocity + num * p.LinearImpulseMultiplier * effector.LinearImpulse * effector.LinearVelocityDir * (60f * dt);
					}
					if (p.Bits.IsBitSet(4))
					{
						Vector3 vector5 = VectorUtil.NormalizeSafe(Vector3.Cross(vector2, vector4), VectorUtil.FindOrthogonal(vector2));
						Vector3 v3 = num * p.RotationReactionMultiplier * effector.RotateAngle * vector5;
						this.RotationTarget += QuaternionUtil.ToVector4(QuaternionUtil.FromAngularVector(v3));
						Vector3 v4 = VectorUtil.NormalizeSafe(Vector3.Cross(effector.LinearVelocityDir, vector4 - 0.01f * Vector3.up), vector5);
						float d = num * p.AngularImpulseMultiplier * effector.AngularImpulse * (60f * dt);
						Vector4 a = QuaternionUtil.ToVector4(QuaternionUtil.FromAngularVector(v4));
						this.RotationSpring.VelocityVec = this.RotationSpring.VelocityVec + d * a;
					}
					this.m_numEffectors++;
				}

				// Token: 0x06001FD1 RID: 8145 RVA: 0x000A3970 File Offset: 0x000A1B70
				public void EndAccumulateTargets(ref BoingWork.Params p)
				{
					if (this.m_numEffectors > 0)
					{
						this.PositionTarget *= this.m_minScale / (float)this.m_numEffectors;
						this.PositionTarget += this.PositionOrigin;
						this.RotationTarget /= (float)this.m_numEffectors;
						this.RotationTarget = QuaternionUtil.ToVector4(QuaternionUtil.FromVector4(this.RotationTarget, true) * QuaternionUtil.FromVector4(this.RotationOrigin, true));
						return;
					}
					this.PositionTarget = this.PositionOrigin;
					this.RotationTarget = this.RotationOrigin;
				}

				// Token: 0x06001FD2 RID: 8146 RVA: 0x000A3A18 File Offset: 0x000A1C18
				public void Execute(ref BoingWork.Params p, float dt)
				{
					bool flag = this.m_numEffectors >= 0;
					bool flag2 = flag ? (this.PositionSpring.Velocity.sqrMagnitude > MathUtil.Epsilon || (this.PositionSpring.Value - this.PositionTarget).sqrMagnitude > MathUtil.Epsilon) : p.Bits.IsBitSet(3);
					bool flag3 = flag ? (this.RotationSpring.VelocityVec.sqrMagnitude > MathUtil.Epsilon || (this.RotationSpring.ValueVec - this.RotationTarget).sqrMagnitude > MathUtil.Epsilon) : p.Bits.IsBitSet(4);
					bool flag4 = p.Bits.IsBitSet(5) && (this.ScaleSpring.Value - this.ScaleTarget).sqrMagnitude > MathUtil.Epsilon;
					if (this.m_numEffectors == 0)
					{
						bool flag5 = true;
						if (flag2)
						{
							flag5 = false;
						}
						else
						{
							this.PositionSpring.Reset(this.PositionTarget);
						}
						if (flag3)
						{
							flag5 = false;
						}
						else
						{
							this.RotationSpring.Reset(QuaternionUtil.FromVector4(this.RotationTarget, true));
						}
						if (flag5)
						{
							return;
						}
					}
					if (this.m_instantAccumulation != 0)
					{
						this.PositionSpring.Value = this.PositionTarget;
						this.RotationSpring.ValueVec = this.RotationTarget;
						this.ScaleSpring.Value = this.ScaleTarget;
						this.m_instantAccumulation = 0;
					}
					else
					{
						if (flag2)
						{
							switch (p.PositionParameterMode)
							{
							case ParameterMode.Exponential:
								this.PositionSpring.TrackExponential(this.PositionTarget, p.PositionExponentialHalfLife, dt);
								break;
							case ParameterMode.OscillationByHalfLife:
								this.PositionSpring.TrackHalfLife(this.PositionTarget, p.PositionOscillationFrequency, p.PositionOscillationHalfLife, dt);
								break;
							case ParameterMode.OscillationByDampingRatio:
								this.PositionSpring.TrackDampingRatio(this.PositionTarget, p.PositionOscillationFrequency * MathUtil.TwoPi, p.PositionOscillationDampingRatio, dt);
								break;
							}
						}
						else
						{
							this.PositionSpring.Value = this.PositionTarget;
							this.PositionSpring.Velocity = Vector3.zero;
						}
						if (flag3)
						{
							switch (p.RotationParameterMode)
							{
							case ParameterMode.Exponential:
								this.RotationSpring.TrackExponential(this.RotationTarget, p.RotationExponentialHalfLife, dt);
								break;
							case ParameterMode.OscillationByHalfLife:
								this.RotationSpring.TrackHalfLife(this.RotationTarget, p.RotationOscillationFrequency, p.RotationOscillationHalfLife, dt);
								break;
							case ParameterMode.OscillationByDampingRatio:
								this.RotationSpring.TrackDampingRatio(this.RotationTarget, p.RotationOscillationFrequency * MathUtil.TwoPi, p.RotationOscillationDampingRatio, dt);
								break;
							}
						}
						else
						{
							this.RotationSpring.ValueVec = this.RotationTarget;
							this.RotationSpring.VelocityVec = Vector4.zero;
						}
						if (flag4)
						{
							switch (p.ScaleParameterMode)
							{
							case ParameterMode.Exponential:
								this.ScaleSpring.TrackExponential(this.ScaleTarget, p.ScaleExponentialHalfLife, dt);
								break;
							case ParameterMode.OscillationByHalfLife:
								this.ScaleSpring.TrackHalfLife(this.ScaleTarget, p.ScaleOscillationFrequency, p.ScaleOscillationHalfLife, dt);
								break;
							case ParameterMode.OscillationByDampingRatio:
								this.ScaleSpring.TrackDampingRatio(this.ScaleTarget, p.ScaleOscillationFrequency * MathUtil.TwoPi, p.ScaleOscillationDampingRatio, dt);
								break;
							}
						}
						else
						{
							this.ScaleSpring.Value = this.ScaleTarget;
							this.ScaleSpring.Velocity = Vector3.zero;
						}
					}
					if (!flag)
					{
						if (!flag2)
						{
							this.PositionSpring.Reset(this.PositionTarget);
						}
						if (!flag3)
						{
							this.RotationSpring.Reset(this.RotationTarget);
						}
					}
				}

				// Token: 0x06001FD3 RID: 8147 RVA: 0x000A3DC8 File Offset: 0x000A1FC8
				public void PullResults(BoingBones bones)
				{
					for (int i = 0; i < bones.BoneData.Length; i++)
					{
						BoingBones.Chain chain = bones.BoneChains[i];
						BoingBones.Bone[] array = bones.BoneData[i];
						if (array != null)
						{
							foreach (BoingBones.Bone bone in array)
							{
								bone.CachedPositionWs = bone.Transform.position;
								bone.CachedPositionLs = bone.Transform.localPosition;
								bone.CachedRotationWs = bone.Transform.rotation;
								bone.CachedRotationLs = bone.Transform.localRotation;
								bone.CachedScaleLs = bone.Transform.localScale;
							}
							for (int k = 0; k < array.Length; k++)
							{
								BoingBones.Bone bone2 = array[k];
								if (k == 0 && !chain.LooseRoot)
								{
									bone2.BlendedPositionWs = bone2.CachedPositionWs;
								}
								else
								{
									bone2.BlendedPositionWs = Vector3.Lerp(bone2.Instance.PositionSpring.Value, bone2.CachedPositionWs, bone2.AnimationBlend);
								}
							}
							for (int l = 0; l < array.Length; l++)
							{
								BoingBones.Bone bone3 = array[l];
								if (l == 0 && !chain.LooseRoot)
								{
									bone3.BlendedRotationWs = bone3.CachedRotationWs;
								}
								else if (bone3.ChildIndices == null)
								{
									if (bone3.ParentIndex >= 0)
									{
										BoingBones.Bone bone4 = array[bone3.ParentIndex];
										bone3.BlendedRotationWs = bone4.BlendedRotationWs * (bone4.RotationInverseWs * bone3.CachedRotationWs);
									}
								}
								else
								{
									Vector3 cachedPositionWs = bone3.CachedPositionWs;
									Vector3 b = BoingWork.ComputeTranslationalResults(bone3.Transform, cachedPositionWs, bone3.BlendedPositionWs, bones);
									Quaternion quaternion = bones.TwistPropagation ? bone3.SpringRotationWs : bone3.CachedRotationWs;
									Quaternion lhs = Quaternion.Inverse(quaternion);
									if (bones.EnableRotationEffect)
									{
										Vector4 a = Vector3.zero;
										float num = 0f;
										foreach (int num2 in bone3.ChildIndices)
										{
											if (num2 >= 0)
											{
												BoingBones.Bone bone5 = array[num2];
												Vector3 cachedPositionWs2 = bone5.CachedPositionWs;
												Vector3 fromDirection = VectorUtil.NormalizeSafe(cachedPositionWs2 - cachedPositionWs, Vector3.zero);
												Vector3 toDirection = VectorUtil.NormalizeSafe(BoingWork.ComputeTranslationalResults(bone5.Transform, cachedPositionWs2, bone5.BlendedPositionWs, bones) - b, Vector3.zero);
												Quaternion rhs = Quaternion.FromToRotation(fromDirection, toDirection);
												Vector4 a2 = QuaternionUtil.ToVector4(lhs * rhs);
												float num3 = Mathf.Max(MathUtil.Epsilon, chain.MaxLengthFromRoot - bone5.LengthFromRoot);
												a += num3 * a2;
												num += num3;
											}
										}
										if (num > 0f)
										{
											Vector4 v = a / num;
											bone3.RotationBackPropDeltaPs = QuaternionUtil.FromVector4(v, true);
											bone3.BlendedRotationWs = quaternion * bone3.RotationBackPropDeltaPs * quaternion;
										}
										else if (bone3.ParentIndex >= 0)
										{
											BoingBones.Bone bone6 = array[bone3.ParentIndex];
											bone3.BlendedRotationWs = bone6.BlendedRotationWs * (bone6.RotationInverseWs * quaternion);
										}
									}
								}
							}
							for (int m = 0; m < array.Length; m++)
							{
								BoingBones.Bone bone7 = array[m];
								if (m == 0 && !chain.LooseRoot)
								{
									bone7.Instance.PositionSpring.Reset(bone7.CachedPositionWs);
									bone7.Instance.RotationSpring.Reset(bone7.CachedRotationWs);
								}
								else
								{
									bone7.Transform.position = BoingWork.ComputeTranslationalResults(bone7.Transform, bone7.Transform.position, bone7.BlendedPositionWs, bones);
									bone7.Transform.rotation = bone7.BlendedRotationWs;
									bone7.Transform.localScale = bone7.BlendedScaleLs;
								}
							}
						}
					}
				}

				// Token: 0x06001FD4 RID: 8148 RVA: 0x000A41C8 File Offset: 0x000A23C8
				private void SuppressWarnings()
				{
					this.m_padding0 = 0f;
					this.m_padding1 = 0f;
					this.m_padding2 = 0f;
					this.m_padding3 = 0;
					this.m_padding4 = 0;
					this.m_padding5 = 0f;
					this.m_padding0 = this.m_padding1;
					this.m_padding1 = this.m_padding2;
					this.m_padding2 = (float)this.m_padding3;
					this.m_padding3 = this.m_padding4;
					this.m_padding4 = (int)this.m_padding0;
					this.m_padding5 = this.m_padding0;
				}

				// Token: 0x0400222F RID: 8751
				public static readonly int Stride = 144 + 2 * Vector3Spring.Stride + QuaternionSpring.Stride;

				// Token: 0x04002230 RID: 8752
				public Vector3 PositionTarget;

				// Token: 0x04002231 RID: 8753
				private float m_padding0;

				// Token: 0x04002232 RID: 8754
				public Vector3 PositionOrigin;

				// Token: 0x04002233 RID: 8755
				private float m_padding1;

				// Token: 0x04002234 RID: 8756
				public Vector4 RotationTarget;

				// Token: 0x04002235 RID: 8757
				public Vector4 RotationOrigin;

				// Token: 0x04002236 RID: 8758
				public Vector3 ScaleTarget;

				// Token: 0x04002237 RID: 8759
				private float m_padding2;

				// Token: 0x04002238 RID: 8760
				private int m_numEffectors;

				// Token: 0x04002239 RID: 8761
				private int m_instantAccumulation;

				// Token: 0x0400223A RID: 8762
				private int m_padding3;

				// Token: 0x0400223B RID: 8763
				private int m_padding4;

				// Token: 0x0400223C RID: 8764
				private Vector3 m_upWs;

				// Token: 0x0400223D RID: 8765
				private float m_minScale;

				// Token: 0x0400223E RID: 8766
				public Vector3Spring PositionSpring;

				// Token: 0x0400223F RID: 8767
				public QuaternionSpring RotationSpring;

				// Token: 0x04002240 RID: 8768
				public Vector3Spring ScaleSpring;

				// Token: 0x04002241 RID: 8769
				public Vector3 PositionPropagationWorkData;

				// Token: 0x04002242 RID: 8770
				private float m_padding5;

				// Token: 0x04002243 RID: 8771
				public Vector4 RotationPropagationWorkData;
			}
		}

		// Token: 0x0200053A RID: 1338
		public struct Output
		{
			// Token: 0x06001FB4 RID: 8116 RVA: 0x000A2FD8 File Offset: 0x000A11D8
			public Output(int instanceID, ref Vector3Spring positionSpring, ref QuaternionSpring rotationSpring, ref Vector3Spring scaleSpring)
			{
				this.InstanceID = instanceID;
				this.m_padding0 = (this.m_padding1 = (this.m_padding2 = 0));
				this.PositionSpring = positionSpring;
				this.RotationSpring = rotationSpring;
				this.ScaleSpring = scaleSpring;
			}

			// Token: 0x06001FB5 RID: 8117 RVA: 0x000A302C File Offset: 0x000A122C
			public void GatherOutput(Dictionary<int, BoingBehavior> behaviorMap, BoingManager.UpdateMode updateMode)
			{
				BoingBehavior boingBehavior;
				if (!behaviorMap.TryGetValue(this.InstanceID, out boingBehavior))
				{
					return;
				}
				if (!boingBehavior.isActiveAndEnabled)
				{
					return;
				}
				if (boingBehavior.UpdateMode != updateMode)
				{
					return;
				}
				boingBehavior.GatherOutput(ref this);
			}

			// Token: 0x06001FB6 RID: 8118 RVA: 0x000A3064 File Offset: 0x000A1264
			public void GatherOutput(Dictionary<int, BoingReactor> reactorMap, BoingManager.UpdateMode updateMode)
			{
				BoingReactor boingReactor;
				if (!reactorMap.TryGetValue(this.InstanceID, out boingReactor))
				{
					return;
				}
				if (!boingReactor.isActiveAndEnabled)
				{
					return;
				}
				if (boingReactor.UpdateMode != updateMode)
				{
					return;
				}
				boingReactor.GatherOutput(ref this);
			}

			// Token: 0x06001FB7 RID: 8119 RVA: 0x000A309C File Offset: 0x000A129C
			private void SuppressWarnings()
			{
				this.m_padding0 = 0;
				this.m_padding1 = 0;
				this.m_padding2 = 0;
				this.m_padding0 = this.m_padding1;
				this.m_padding1 = this.m_padding2;
				this.m_padding2 = this.m_padding0;
			}

			// Token: 0x04002208 RID: 8712
			public static readonly int Stride = 16 + Vector3Spring.Stride + QuaternionSpring.Stride;

			// Token: 0x04002209 RID: 8713
			public int InstanceID;

			// Token: 0x0400220A RID: 8714
			public int m_padding0;

			// Token: 0x0400220B RID: 8715
			public int m_padding1;

			// Token: 0x0400220C RID: 8716
			public int m_padding2;

			// Token: 0x0400220D RID: 8717
			public Vector3Spring PositionSpring;

			// Token: 0x0400220E RID: 8718
			public QuaternionSpring RotationSpring;

			// Token: 0x0400220F RID: 8719
			public Vector3Spring ScaleSpring;
		}
	}
}
