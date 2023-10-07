using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000362 RID: 866
	public class BoingBehavior : BoingBase
	{
		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06001934 RID: 6452 RVA: 0x0008B53F File Offset: 0x0008973F
		// (set) Token: 0x06001935 RID: 6453 RVA: 0x0008B551 File Offset: 0x00089751
		public Vector3Spring PositionSpring
		{
			get
			{
				return this.Params.Instance.PositionSpring;
			}
			set
			{
				this.Params.Instance.PositionSpring = value;
				this.PositionSpringDirty = true;
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06001936 RID: 6454 RVA: 0x0008B56B File Offset: 0x0008976B
		// (set) Token: 0x06001937 RID: 6455 RVA: 0x0008B57D File Offset: 0x0008977D
		public QuaternionSpring RotationSpring
		{
			get
			{
				return this.Params.Instance.RotationSpring;
			}
			set
			{
				this.Params.Instance.RotationSpring = value;
				this.RotationSpringDirty = true;
			}
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06001938 RID: 6456 RVA: 0x0008B597 File Offset: 0x00089797
		// (set) Token: 0x06001939 RID: 6457 RVA: 0x0008B5A9 File Offset: 0x000897A9
		public Vector3Spring ScaleSpring
		{
			get
			{
				return this.Params.Instance.ScaleSpring;
			}
			set
			{
				this.Params.Instance.ScaleSpring = value;
				this.ScaleSpringDirty = true;
			}
		}

		// Token: 0x0600193A RID: 6458 RVA: 0x0008B5C3 File Offset: 0x000897C3
		public BoingBehavior()
		{
			this.Params.Init();
		}

		// Token: 0x0600193B RID: 6459 RVA: 0x0008B5EC File Offset: 0x000897EC
		public virtual void Reboot()
		{
			this.Params.Instance.PositionSpring.Reset(base.transform.position);
			this.Params.Instance.RotationSpring.Reset(base.transform.rotation);
			this.Params.Instance.ScaleSpring.Reset(base.transform.localScale);
			this.CachedPositionLs = base.transform.localPosition;
			this.CachedRotationLs = base.transform.localRotation;
			this.CachedPositionWs = base.transform.position;
			this.CachedRotationWs = base.transform.rotation;
			this.CachedScaleLs = base.transform.localScale;
			this.CachedTransformValid = true;
		}

		// Token: 0x0600193C RID: 6460 RVA: 0x0008B6B5 File Offset: 0x000898B5
		public virtual void OnEnable()
		{
			this.CachedTransformValid = false;
			this.InitRebooted = false;
			this.Register();
		}

		// Token: 0x0600193D RID: 6461 RVA: 0x0008B6CB File Offset: 0x000898CB
		public void Start()
		{
			this.InitRebooted = false;
		}

		// Token: 0x0600193E RID: 6462 RVA: 0x0008B6D4 File Offset: 0x000898D4
		public virtual void OnDisable()
		{
			this.Unregister();
		}

		// Token: 0x0600193F RID: 6463 RVA: 0x0008B6DC File Offset: 0x000898DC
		protected virtual void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x06001940 RID: 6464 RVA: 0x0008B6E4 File Offset: 0x000898E4
		protected virtual void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06001941 RID: 6465 RVA: 0x0008B6EC File Offset: 0x000898EC
		public void UpdateFlags()
		{
			this.Params.Bits.SetBit(0, this.TwoDDistanceCheck);
			this.Params.Bits.SetBit(1, this.TwoDPositionInfluence);
			this.Params.Bits.SetBit(2, this.TwoDRotationInfluence);
			this.Params.Bits.SetBit(3, this.EnablePositionEffect);
			this.Params.Bits.SetBit(4, this.EnableRotationEffect);
			this.Params.Bits.SetBit(5, this.EnableScaleEffect);
			this.Params.Bits.SetBit(6, this.GlobalReactionUpVector);
			this.Params.Bits.SetBit(9, this.UpdateMode == BoingManager.UpdateMode.FixedUpdate);
			this.Params.Bits.SetBit(10, this.UpdateMode == BoingManager.UpdateMode.EarlyUpdate);
			this.Params.Bits.SetBit(11, this.UpdateMode == BoingManager.UpdateMode.LateUpdate);
		}

		// Token: 0x06001942 RID: 6466 RVA: 0x0008B7EB File Offset: 0x000899EB
		public virtual void PrepareExecute()
		{
			this.PrepareExecute(false);
		}

		// Token: 0x06001943 RID: 6467 RVA: 0x0008B7F4 File Offset: 0x000899F4
		protected void PrepareExecute(bool accumulateEffectors)
		{
			if (this.SharedParams != null)
			{
				BoingWork.Params.Copy(ref this.SharedParams.Params, ref this.Params);
			}
			this.UpdateFlags();
			this.Params.InstanceID = base.GetInstanceID();
			this.Params.Instance.PrepareExecute(ref this.Params, this.CachedPositionWs, this.CachedRotationWs, base.transform.localScale, accumulateEffectors);
		}

		// Token: 0x06001944 RID: 6468 RVA: 0x0008B86A File Offset: 0x00089A6A
		public void Execute(float dt)
		{
			this.Params.Execute(dt);
		}

		// Token: 0x06001945 RID: 6469 RVA: 0x0008B878 File Offset: 0x00089A78
		public void PullResults()
		{
			this.PullResults(ref this.Params);
		}

		// Token: 0x06001946 RID: 6470 RVA: 0x0008B888 File Offset: 0x00089A88
		public void GatherOutput(ref BoingWork.Output o)
		{
			if (!BoingManager.UseAsynchronousJobs)
			{
				this.Params.Instance.PositionSpring = o.PositionSpring;
				this.Params.Instance.RotationSpring = o.RotationSpring;
				this.Params.Instance.ScaleSpring = o.ScaleSpring;
				return;
			}
			if (this.PositionSpringDirty)
			{
				this.PositionSpringDirty = false;
			}
			else
			{
				this.Params.Instance.PositionSpring = o.PositionSpring;
			}
			if (this.RotationSpringDirty)
			{
				this.RotationSpringDirty = false;
			}
			else
			{
				this.Params.Instance.RotationSpring = o.RotationSpring;
			}
			if (this.ScaleSpringDirty)
			{
				this.ScaleSpringDirty = false;
				return;
			}
			this.Params.Instance.ScaleSpring = o.ScaleSpring;
		}

		// Token: 0x06001947 RID: 6471 RVA: 0x0008B954 File Offset: 0x00089B54
		private void PullResults(ref BoingWork.Params p)
		{
			this.CachedPositionLs = base.transform.localPosition;
			this.CachedPositionWs = base.transform.position;
			this.RenderPositionWs = BoingWork.ComputeTranslationalResults(base.transform, base.transform.position, p.Instance.PositionSpring.Value, this);
			base.transform.position = this.RenderPositionWs;
			this.CachedRotationLs = base.transform.localRotation;
			this.CachedRotationWs = base.transform.rotation;
			this.RenderRotationWs = p.Instance.RotationSpring.ValueQuat;
			base.transform.rotation = this.RenderRotationWs;
			this.CachedScaleLs = base.transform.localScale;
			this.RenderScaleLs = p.Instance.ScaleSpring.Value;
			base.transform.localScale = this.RenderScaleLs;
			this.CachedTransformValid = true;
		}

		// Token: 0x06001948 RID: 6472 RVA: 0x0008BA4C File Offset: 0x00089C4C
		public virtual void Restore()
		{
			if (!this.CachedTransformValid)
			{
				return;
			}
			if (Application.isEditor)
			{
				if ((base.transform.position - this.RenderPositionWs).sqrMagnitude < 0.0001f)
				{
					base.transform.localPosition = this.CachedPositionLs;
				}
				if (QuaternionUtil.GetAngle(base.transform.rotation * Quaternion.Inverse(this.RenderRotationWs)) < 0.01f)
				{
					base.transform.localRotation = this.CachedRotationLs;
				}
				if ((base.transform.localScale - this.RenderScaleLs).sqrMagnitude < 0.0001f)
				{
					base.transform.localScale = this.CachedScaleLs;
					return;
				}
			}
			else
			{
				base.transform.localPosition = this.CachedPositionLs;
				base.transform.localRotation = this.CachedRotationLs;
				base.transform.localScale = this.CachedScaleLs;
			}
		}

		// Token: 0x040019F3 RID: 6643
		public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

		// Token: 0x040019F4 RID: 6644
		public bool TwoDDistanceCheck;

		// Token: 0x040019F5 RID: 6645
		public bool TwoDPositionInfluence;

		// Token: 0x040019F6 RID: 6646
		public bool TwoDRotationInfluence;

		// Token: 0x040019F7 RID: 6647
		public bool EnablePositionEffect = true;

		// Token: 0x040019F8 RID: 6648
		public bool EnableRotationEffect = true;

		// Token: 0x040019F9 RID: 6649
		public bool EnableScaleEffect;

		// Token: 0x040019FA RID: 6650
		public bool GlobalReactionUpVector;

		// Token: 0x040019FB RID: 6651
		public BoingManager.TranslationLockSpace TranslationLockSpace;

		// Token: 0x040019FC RID: 6652
		public bool LockTranslationX;

		// Token: 0x040019FD RID: 6653
		public bool LockTranslationY;

		// Token: 0x040019FE RID: 6654
		public bool LockTranslationZ;

		// Token: 0x040019FF RID: 6655
		public BoingWork.Params Params;

		// Token: 0x04001A00 RID: 6656
		public SharedBoingParams SharedParams;

		// Token: 0x04001A01 RID: 6657
		internal bool PositionSpringDirty;

		// Token: 0x04001A02 RID: 6658
		internal bool RotationSpringDirty;

		// Token: 0x04001A03 RID: 6659
		internal bool ScaleSpringDirty;

		// Token: 0x04001A04 RID: 6660
		internal bool CachedTransformValid;

		// Token: 0x04001A05 RID: 6661
		internal Vector3 CachedPositionLs;

		// Token: 0x04001A06 RID: 6662
		internal Vector3 CachedPositionWs;

		// Token: 0x04001A07 RID: 6663
		internal Vector3 RenderPositionWs;

		// Token: 0x04001A08 RID: 6664
		internal Quaternion CachedRotationLs;

		// Token: 0x04001A09 RID: 6665
		internal Quaternion CachedRotationWs;

		// Token: 0x04001A0A RID: 6666
		internal Quaternion RenderRotationWs;

		// Token: 0x04001A0B RID: 6667
		internal Vector3 CachedScaleLs;

		// Token: 0x04001A0C RID: 6668
		internal Vector3 RenderScaleLs;

		// Token: 0x04001A0D RID: 6669
		internal bool InitRebooted;
	}
}
