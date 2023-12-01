using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BoingKit
{
	public class BoingBones : BoingReactor
	{
		protected override void Register()
		{
			BoingManager.Register(this);
		}

		protected override void Unregister()
		{
			BoingManager.Unregister(this);
		}

		protected override void OnUpgrade(Version oldVersion, Version newVersion)
		{
			base.OnUpgrade(oldVersion, newVersion);
			if (oldVersion.Revision < 33)
			{
				this.TwistPropagation = false;
			}
		}

		public void OnValidate()
		{
			this.RescanBoneChains();
			this.UpdateCollisionRadius();
		}

		public override void OnEnable()
		{
			base.OnEnable();
			this.RescanBoneChains();
			this.Reboot();
		}

		public override void OnDisable()
		{
			base.OnDisable();
			this.Restore();
		}

		public void RescanBoneChains()
		{
			if (this.BoneChains == null)
			{
				return;
			}
			int num = this.BoneChains.Length;
			if (this.BoneData == null || this.BoneData.Length != num)
			{
				BoingBones.Bone[][] array = new BoingBones.Bone[num][];
				if (this.BoneData != null)
				{
					int i = 0;
					int num2 = Mathf.Min(this.BoneData.Length, num);
					while (i < num2)
					{
						array[i] = this.BoneData[i];
						i++;
					}
				}
				this.BoneData = array;
			}
			Queue<BoingBones.RescanEntry> queue = new Queue<BoingBones.RescanEntry>();
			for (int j = 0; j < num; j++)
			{
				BoingBones.Chain chain = this.BoneChains[j];
				bool flag = false;
				if (this.BoneData[j] == null)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedRoot == null)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedRoot != chain.Root)
				{
					flag = true;
				}
				if (!flag && chain.m_scannedExclusion != null != (chain.Exclusion != null))
				{
					flag = true;
				}
				if (!flag && chain.Exclusion != null)
				{
					if (chain.m_scannedExclusion.Length != chain.Exclusion.Length)
					{
						flag = true;
					}
					else
					{
						for (int k = 0; k < chain.m_scannedExclusion.Length; k++)
						{
							if (!(chain.m_scannedExclusion[k] == chain.Exclusion[k]))
							{
								flag = true;
								break;
							}
						}
					}
				}
				Transform transform = (chain != null) ? chain.Root : null;
				int num3 = (transform != null) ? Codec.HashTransformHierarchy(transform) : -1;
				if (!flag && transform != null && chain.m_hierarchyHash != num3)
				{
					flag = true;
				}
				if (flag)
				{
					if (transform == null)
					{
						this.BoneData[j] = null;
					}
					else
					{
						chain.m_scannedRoot = chain.Root;
						chain.m_scannedExclusion = chain.Exclusion.ToArray<Transform>();
						chain.m_hierarchyHash = num3;
						chain.MaxLengthFromRoot = 0f;
						List<BoingBones.Bone> list = new List<BoingBones.Bone>();
						queue.Enqueue(new BoingBones.RescanEntry(transform, -1, 0f));
						while (queue.Count > 0)
						{
							BoingBones.RescanEntry rescanEntry = queue.Dequeue();
							if (!chain.Exclusion.Contains(rescanEntry.Transform))
							{
								int count = list.Count;
								Transform transform2 = rescanEntry.Transform;
								int[] array2 = new int[transform2.childCount];
								for (int l = 0; l < array2.Length; l++)
								{
									array2[l] = -1;
								}
								int num4 = 0;
								int m = 0;
								int childCount = transform2.childCount;
								while (m < childCount)
								{
									Transform child = transform2.GetChild(m);
									if (!chain.Exclusion.Contains(child))
									{
										float num5 = Vector3.Distance(rescanEntry.Transform.position, child.position);
										float lengthFromRoot = rescanEntry.LengthFromRoot + num5;
										queue.Enqueue(new BoingBones.RescanEntry(child, count, lengthFromRoot));
										num4++;
									}
									m++;
								}
								chain.MaxLengthFromRoot = Mathf.Max(rescanEntry.LengthFromRoot, chain.MaxLengthFromRoot);
								BoingBones.Bone bone = new BoingBones.Bone(transform2, rescanEntry.ParentIndex, rescanEntry.LengthFromRoot);
								if (num4 > 0)
								{
									bone.ChildIndices = array2;
								}
								list.Add(bone);
							}
						}
						for (int n = 0; n < list.Count; n++)
						{
							BoingBones.Bone bone2 = list[n];
							if (bone2.ParentIndex >= 0)
							{
								BoingBones.Bone bone3 = list[bone2.ParentIndex];
								int num6 = 0;
								while (bone3.ChildIndices[num6] >= 0)
								{
									num6++;
								}
								if (num6 < bone3.ChildIndices.Length)
								{
									bone3.ChildIndices[num6] = n;
								}
							}
						}
						if (list.Count != 0)
						{
							float num7 = MathUtil.InvSafe(chain.MaxLengthFromRoot);
							for (int num8 = 0; num8 < list.Count; num8++)
							{
								BoingBones.Bone bone4 = list[num8];
								float t = Mathf.Clamp01(bone4.LengthFromRoot * num7);
								bone4.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t, chain.CollisionRadiusCustomCurve);
							}
							this.BoneData[j] = list.ToArray();
							this.Reboot(j);
						}
					}
				}
			}
		}

		private void UpdateCollisionRadius()
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					float num = MathUtil.InvSafe(chain.MaxLengthFromRoot);
					foreach (BoingBones.Bone bone in array)
					{
						float t = Mathf.Clamp01(bone.LengthFromRoot * num);
						bone.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t, chain.CollisionRadiusCustomCurve);
					}
				}
			}
		}

		public override void Reboot()
		{
			base.Reboot();
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				this.Reboot(i);
			}
		}

		public void Reboot(int iChain)
		{
			BoingBones.Bone[] array = this.BoneData[iChain];
			if (array == null)
			{
				return;
			}
			foreach (BoingBones.Bone bone in array)
			{
				bone.Instance.PositionSpring.Reset(bone.Transform.position);
				bone.Instance.RotationSpring.Reset(bone.Transform.rotation);
				bone.CachedPositionWs = bone.Transform.position;
				bone.CachedPositionLs = bone.Transform.localPosition;
				bone.CachedRotationWs = bone.Transform.rotation;
				bone.CachedRotationLs = bone.Transform.localRotation;
				bone.CachedScaleLs = bone.Transform.localScale;
			}
			this.CachedTransformValid = true;
		}

		internal float MinScale
		{
			get
			{
				return this.m_minScale;
			}
		}

		public override void PrepareExecute()
		{
			base.PrepareExecute();
			this.Params.Bits.SetBit(4, false);
			float fixedDeltaTime = Time.fixedDeltaTime;
			float d = (this.UpdateMode == BoingManager.UpdateMode.FixedUpdate) ? fixedDeltaTime : Time.deltaTime;
			this.m_minScale = Mathf.Min(base.transform.localScale.x, Mathf.Min(base.transform.localScale.y, base.transform.localScale.z));
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null && !(chain.Root == null) && array.Length != 0)
				{
					Vector3 b = chain.Gravity * d;
					float num = 0f;
					foreach (BoingBones.Bone bone in array)
					{
						if (bone.ParentIndex < 0)
						{
							if (!chain.LooseRoot)
							{
								bone.Instance.PositionSpring.Reset(bone.Transform.position);
								bone.Instance.RotationSpring.Reset(bone.Transform.rotation);
							}
							bone.LengthFromRoot = 0f;
						}
						else
						{
							BoingBones.Bone bone2 = array[bone.ParentIndex];
							float num2 = Vector3.Distance(bone.Transform.position, bone2.Transform.position);
							bone.LengthFromRoot = bone2.LengthFromRoot + num2;
							num = Mathf.Max(num, bone.LengthFromRoot);
						}
					}
					float num3 = MathUtil.InvSafe(num);
					foreach (BoingBones.Bone bone3 in array)
					{
						float t = bone3.LengthFromRoot * num3;
						bone3.AnimationBlend = BoingBones.Chain.EvaluateCurve(chain.AnimationBlendCurveType, t, chain.AnimationBlendCustomCurve);
						bone3.LengthStiffness = BoingBones.Chain.EvaluateCurve(chain.LengthStiffnessCurveType, t, chain.LengthStiffnessCustomCurve);
						bone3.LengthStiffnessT = 1f - Mathf.Pow(1f - bone3.LengthStiffness, 30f * fixedDeltaTime);
						bone3.FullyStiffToParentLength = ((bone3.ParentIndex >= 0) ? Vector3.Distance(array[bone3.ParentIndex].Transform.position, bone3.Transform.position) : 0f);
						bone3.PoseStiffness = BoingBones.Chain.EvaluateCurve(chain.PoseStiffnessCurveType, t, chain.PoseStiffnessCustomCurve);
						bone3.BendAngleCap = chain.MaxBendAngleCap * MathUtil.Deg2Rad * BoingBones.Chain.EvaluateCurve(chain.BendAngleCapCurveType, t, chain.BendAngleCapCustomCurve);
						bone3.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t, chain.CollisionRadiusCustomCurve);
						bone3.SquashAndStretch = BoingBones.Chain.EvaluateCurve(chain.SquashAndStretchCurveType, t, chain.SquashAndStretchCustomCurve);
					}
					Vector3 position = array[0].Transform.position;
					for (int l = 0; l < array.Length; l++)
					{
						BoingBones.Bone bone4 = array[l];
						float t2 = bone4.LengthFromRoot * num3;
						bone4.AnimationBlend = BoingBones.Chain.EvaluateCurve(chain.AnimationBlendCurveType, t2, chain.AnimationBlendCustomCurve);
						bone4.LengthStiffness = BoingBones.Chain.EvaluateCurve(chain.LengthStiffnessCurveType, t2, chain.LengthStiffnessCustomCurve);
						bone4.PoseStiffness = BoingBones.Chain.EvaluateCurve(chain.PoseStiffnessCurveType, t2, chain.PoseStiffnessCustomCurve);
						bone4.BendAngleCap = chain.MaxBendAngleCap * MathUtil.Deg2Rad * BoingBones.Chain.EvaluateCurve(chain.BendAngleCapCurveType, t2, chain.BendAngleCapCustomCurve);
						bone4.CollisionRadius = chain.MaxCollisionRadius * BoingBones.Chain.EvaluateCurve(chain.CollisionRadiusCurveType, t2, chain.CollisionRadiusCustomCurve);
						bone4.SquashAndStretch = BoingBones.Chain.EvaluateCurve(chain.SquashAndStretchCurveType, t2, chain.SquashAndStretchCustomCurve);
						if (l > 0)
						{
							BoingBones.Bone bone5 = bone4;
							bone5.Instance.PositionSpring.Velocity = bone5.Instance.PositionSpring.Velocity + b;
						}
						bone4.RotationInverseWs = Quaternion.Inverse(bone4.Transform.rotation);
						bone4.SpringRotationWs = bone4.Instance.RotationSpring.ValueQuat;
						bone4.SpringRotationInverseWs = Quaternion.Inverse(bone4.SpringRotationWs);
						Vector3 vector = bone4.Transform.position;
						Quaternion rotation = bone4.Transform.rotation;
						Vector3 localScale = bone4.Transform.localScale;
						if (bone4.ParentIndex >= 0)
						{
							BoingBones.Bone bone6 = array[bone4.ParentIndex];
							Vector3 position2 = bone6.Transform.position;
							Vector3 value = bone6.Instance.PositionSpring.Value;
							Vector3 a = bone6.SpringRotationInverseWs * (bone4.Instance.PositionSpring.Value - value);
							Quaternion a2 = bone6.SpringRotationInverseWs * bone4.Instance.RotationSpring.ValueQuat;
							Vector3 position3 = bone4.Transform.position;
							Quaternion rotation2 = bone4.Transform.rotation;
							Vector3 b2 = bone6.RotationInverseWs * (position3 - position2);
							Quaternion b3 = bone6.RotationInverseWs * rotation2;
							float poseStiffness = bone4.PoseStiffness;
							Vector3 point = Vector3.Lerp(a, b2, poseStiffness);
							Quaternion rhs = Quaternion.Slerp(a2, b3, poseStiffness);
							vector = value + bone6.SpringRotationWs * point;
							rotation = bone6.SpringRotationWs * rhs;
							if (bone4.BendAngleCap < MathUtil.Pi - MathUtil.Epsilon)
							{
								Vector3 vector2 = vector - position;
								vector2 = VectorUtil.ClampBend(vector2, position3 - position, bone4.BendAngleCap);
								vector = position + vector2;
							}
						}
						if (chain.ParamsOverride == null)
						{
							bone4.Instance.PrepareExecute(ref this.Params, vector, rotation, localScale, true);
						}
						else
						{
							bone4.Instance.PrepareExecute(ref chain.ParamsOverride.Params, vector, rotation, localScale, true);
						}
					}
				}
			}
		}

		public void AccumulateTarget(ref BoingEffector.Params effector, float dt)
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null && chain.EffectorReaction)
				{
					foreach (BoingBones.Bone bone in array)
					{
						if (chain.ParamsOverride == null)
						{
							bone.Instance.AccumulateTarget(ref this.Params, ref effector, dt);
						}
						else
						{
							Bits32 bits = chain.ParamsOverride.Params.Bits;
							chain.ParamsOverride.Params.Bits = this.Params.Bits;
							bone.Instance.AccumulateTarget(ref chain.ParamsOverride.Params, ref effector, dt);
							chain.ParamsOverride.Params.Bits = bits;
						}
					}
				}
			}
		}

		public void EndAccumulateTargets()
		{
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					foreach (BoingBones.Bone bone in array)
					{
						if (chain.ParamsOverride == null)
						{
							bone.Instance.EndAccumulateTargets(ref this.Params);
						}
						else
						{
							bone.Instance.EndAccumulateTargets(ref chain.ParamsOverride.Params);
						}
					}
				}
			}
		}

		public override void Restore()
		{
			if (!this.CachedTransformValid)
			{
				return;
			}
			for (int i = 0; i < this.BoneData.Length; i++)
			{
				BoingBones.Chain chain = this.BoneChains[i];
				BoingBones.Bone[] array = this.BoneData[i];
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						BoingBones.Bone bone = array[j];
						if (j != 0 || chain.LooseRoot)
						{
							bone.Transform.localPosition = bone.CachedPositionLs;
							bone.Transform.localRotation = bone.CachedRotationLs;
							bone.Transform.localScale = bone.CachedScaleLs;
						}
					}
				}
			}
		}

		[SerializeField]
		internal BoingBones.Bone[][] BoneData;

		public BoingBones.Chain[] BoneChains = new BoingBones.Chain[1];

		public bool TwistPropagation = true;

		[Range(0.1f, 20f)]
		public float MaxCollisionResolutionSpeed = 3f;

		public BoingBoneCollider[] BoingColliders = new BoingBoneCollider[0];

		public Collider[] UnityColliders = new Collider[0];

		public bool DebugDrawRawBones;

		public bool DebugDrawTargetBones;

		public bool DebugDrawBoingBones;

		public bool DebugDrawFinalBones;

		public bool DebugDrawColliders;

		public bool DebugDrawChainBounds;

		public bool DebugDrawBoneNames;

		public bool DebugDrawLengthFromRoot;

		private float m_minScale = 1f;

		[Serializable]
		public class Bone
		{
			internal void UpdateBounds()
			{
				this.Bounds = new Bounds(this.Instance.PositionSpring.Value, 2f * this.CollisionRadius * Vector3.one);
			}

			internal Bone(Transform transform, int iParent, float lengthFromRoot)
			{
				this.Transform = transform;
				this.RotationInverseWs = Quaternion.identity;
				this.ParentIndex = iParent;
				this.LengthFromRoot = lengthFromRoot;
				this.Instance.Reset();
				this.CachedPositionWs = transform.position;
				this.CachedPositionLs = transform.localPosition;
				this.CachedRotationWs = transform.rotation;
				this.CachedRotationLs = transform.localRotation;
				this.CachedScaleLs = transform.localScale;
				this.AnimationBlend = 0f;
				this.LengthStiffness = 0f;
				this.PoseStiffness = 0f;
				this.BendAngleCap = 180f;
				this.CollisionRadius = 0f;
			}

			internal BoingWork.Params.InstanceData Instance;

			internal Transform Transform;

			internal Vector3 ScaleWs;

			internal Vector3 CachedScaleLs;

			internal Vector3 BlendedPositionWs;

			internal Vector3 BlendedScaleLs;

			internal Vector3 CachedPositionWs;

			internal Vector3 CachedPositionLs;

			internal Bounds Bounds;

			internal Quaternion RotationInverseWs;

			internal Quaternion SpringRotationWs;

			internal Quaternion SpringRotationInverseWs;

			internal Quaternion CachedRotationWs;

			internal Quaternion CachedRotationLs;

			internal Quaternion BlendedRotationWs;

			internal Quaternion RotationBackPropDeltaPs;

			internal int ParentIndex;

			internal int[] ChildIndices;

			internal float LengthFromRoot;

			internal float AnimationBlend;

			internal float LengthStiffness;

			internal float LengthStiffnessT;

			internal float FullyStiffToParentLength;

			internal float PoseStiffness;

			internal float BendAngleCap;

			internal float CollisionRadius;

			internal float SquashAndStretch;
		}

		[Serializable]
		public class Chain
		{
			public static float EvaluateCurve(BoingBones.Chain.CurveType type, float t, AnimationCurve curve)
			{
				switch (type)
				{
				case BoingBones.Chain.CurveType.ConstantOne:
					return 1f;
				case BoingBones.Chain.CurveType.ConstantHalf:
					return 0.5f;
				case BoingBones.Chain.CurveType.ConstantZero:
					return 0f;
				case BoingBones.Chain.CurveType.RootOneTailHalf:
					return 1f - 0.5f * Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootOneTailZero:
					return 1f - Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootHalfTailOne:
					return 0.5f + 0.5f * Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.RootZeroTailOne:
					return Mathf.Clamp01(t);
				case BoingBones.Chain.CurveType.Custom:
					return curve.Evaluate(t);
				default:
					return 0f;
				}
			}

			[Tooltip("Root Transform object from which to build a chain (or tree if a bone has multiple children) of bouncy boing bones.")]
			public Transform Root;

			[Tooltip("List of Transform objects to exclude from chain building.")]
			public Transform[] Exclusion;

			[Tooltip("Enable to allow reaction to boing effectors.")]
			public bool EffectorReaction = true;

			[Tooltip("Enable to allow root Transform object to be sprung around as well. Otherwise, no effects will be applied to the root Transform object.")]
			public bool LooseRoot;

			[Tooltip("Assign a SharedParamsOverride asset to override the parameters for this chain. Useful for chains using different parameters than that of the BoingBones component.")]
			public SharedBoingParams ParamsOverride;

			[ConditionalField(null, null, null, null, null, null, null, Label = "Animation Blend", Tooltip = "Animation blend determines each bone's final transform between the original raw transform and its corresponding boing bone. 1.0 means 100% contribution from raw (or animated) transform. 0.0 means 100% contribution from boing bone.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's animation blend:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType AnimationBlendCurveType = BoingBones.Chain.CurveType.RootOneTailZero;

			[ConditionalField("AnimationBlendCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve AnimationBlendCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

			[ConditionalField(null, null, null, null, null, null, null, Label = "Length Stiffness", Tooltip = "Length stiffness determines how much each target bone (target transform each boing bone is sprung towards) tries to maintain original distance from its parent. 1.0 means 100% distance maintenance. 0.0 means 0% distance maintenance.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's length stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType LengthStiffnessCurveType;

			[ConditionalField("LengthStiffnessCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve LengthStiffnessCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			[ConditionalField(null, null, null, null, null, null, null, Label = "Pose Stiffness", Tooltip = "Pose stiffness determines how much each target bone (target transform each boing bone is sprung towards) tries to maintain original transform. 1.0 means 100% original transform maintenance. 0.0 means 0% original transform maintenance.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's pose stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType PoseStiffnessCurveType;

			[ConditionalField("PoseStiffnessCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve PoseStiffnessCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			[ConditionalField(null, null, null, null, null, null, null, Label = "Bend Angle Cap", Tooltip = "Maximum bone bend angle cap.", Min = 0f, Max = 180f)]
			public float MaxBendAngleCap = 180f;

			[ConditionalField(null, null, null, null, null, null, null, Label = "  Curve Type", Tooltip = "Percentage(0.0 = 0 %; 1.0 = 100 %) of maximum bone bend angle cap.Bend angle cap limits how much each bone can bend relative to the root (in degrees). 1.0 means 100% maximum bend angle cap. 0.0 means 0% maximum bend angle cap.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's pose stiffness:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType BendAngleCapCurveType;

			[ConditionalField("BendAngleCapCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "    Custom Curve")]
			public AnimationCurve BendAngleCapCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			[ConditionalField(null, null, null, null, null, null, null, Label = "Collision Radius", Tooltip = "Maximum bone collision radius.")]
			public float MaxCollisionRadius = 0.1f;

			[ConditionalField(null, null, null, null, null, null, null, Label = "  Curve Type", Tooltip = "Percentage (0.0 = 0%; 1.0 = 100%) of maximum bone collision radius.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's collision radius:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType CollisionRadiusCurveType;

			[ConditionalField("CollisionRadiusCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "    Custom Curve")]
			public AnimationCurve CollisionRadiusCustomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

			[ConditionalField(null, null, null, null, null, null, null, Label = "Boing Kit Collision", Tooltip = "Enable to allow this chain to collide with Boing Kit's own implementation of lightweight colliders")]
			public bool EnableBoingKitCollision;

			[ConditionalField(null, null, null, null, null, null, null, Label = "Unity Collision", Tooltip = "Enable to allow this chain to collide with Unity colliders.")]
			public bool EnableUnityCollision;

			[ConditionalField(null, null, null, null, null, null, null, Label = "Inter-Chain Collision", Tooltip = "Enable to allow this chain to collide with other chain (under the same BoingBones component) with inter-chain collision enabled.")]
			public bool EnableInterChainCollision;

			public Vector3 Gravity = Vector3.zero;

			internal Bounds Bounds;

			[ConditionalField(null, null, null, null, null, null, null, Label = "Squash & Stretch", Tooltip = "Percentage (0.0 = 0%; 1.0 = 100%) of each bone's squash & stretch effect. Squash & stretch is the effect of volume preservation by scaling bones based on how compressed or stretched the distances between bones become.\n\nEach curve type provides a type of mapping for each bone's percentage down the chain (0.0 at root & 1.0 at maximum chain length) to the bone's squash & stretch effect amount:\n\n - Constant One: 1.0 all the way.\n - Constant Half: 0.5 all the way.\n - Constant Zero: 0.0 all the way.\n - Root One Tail Half: 1.0 at 0% chain length and 0.5 at 100% chain length.\n - Root One Tail Zero: 1.0 at 0% chain length and 0.0 at 100% chain length.\n - Root Half Tail One: 0.5 at 0% chain length and 1.0 at 100% chain length.\n - Root Zero Tail One: 0.0 at 0% chain length and 1.0 at 100% chain length.\n - Custom: Custom curve.")]
			public BoingBones.Chain.CurveType SquashAndStretchCurveType = BoingBones.Chain.CurveType.ConstantZero;

			[ConditionalField("SquashAndStretchCurveType", BoingBones.Chain.CurveType.Custom, null, null, null, null, null, Label = "  Custom Curve")]
			public AnimationCurve SquashAndStretchCustomCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

			[ConditionalField(null, null, null, null, null, null, null, Label = "  Max Squash", Tooltip = "Maximum squash amount. For example, 2.0 means a maximum scale of 200% when squashed.", Min = 1f, Max = 5f)]
			public float MaxSquash = 1.1f;

			[ConditionalField(null, null, null, null, null, null, null, Label = "  Max Stretch", Tooltip = "Maximum stretch amount. For example, 2.0 means a minimum scale of 50% when stretched (200% stretched).", Min = 1f, Max = 5f)]
			public float MaxStretch = 2f;

			internal Transform m_scannedRoot;

			internal Transform[] m_scannedExclusion;

			internal int m_hierarchyHash = -1;

			internal float MaxLengthFromRoot;

			public enum CurveType
			{
				ConstantOne,
				ConstantHalf,
				ConstantZero,
				RootOneTailHalf,
				RootOneTailZero,
				RootHalfTailOne,
				RootZeroTailOne,
				Custom
			}
		}

		private class RescanEntry
		{
			internal RescanEntry(Transform transform, int iParent, float lengthFromRoot)
			{
				this.Transform = transform;
				this.ParentIndex = iParent;
				this.LengthFromRoot = lengthFromRoot;
			}

			internal Transform Transform;

			internal int ParentIndex;

			internal float LengthFromRoot;
		}
	}
}
