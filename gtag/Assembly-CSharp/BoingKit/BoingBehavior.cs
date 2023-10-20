using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000364 RID: 868
	public class BoingBehavior : BoingBase
	{
		// Token: 0x17000198 RID: 408
		// (get) Token: 0x0600193D RID: 6461 RVA: 0x0008BA27 File Offset: 0x00089C27
		// (set) Token: 0x0600193E RID: 6462 RVA: 0x0008BA39 File Offset: 0x00089C39
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

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x0600193F RID: 6463 RVA: 0x0008BA53 File Offset: 0x00089C53
		// (set) Token: 0x06001940 RID: 6464 RVA: 0x0008BA65 File Offset: 0x00089C65
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

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06001941 RID: 6465 RVA: 0x0008BA7F File Offset: 0x00089C7F
		// (set) Token: 0x06001942 RID: 6466 RVA: 0x0008BA91 File Offset: 0x00089C91
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

		// Token: 0x06001943 RID: 6467 RVA: 0x0008BAAB File Offset: 0x00089CAB
		public BoingBehavior()
		{
			this.Params.Init();
		}

		// Token: 0x06001944 RID: 6468 RVA: 0x0008BAD4 File Offset: 0x00089CD4
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

		// Token: 0x06001945 RID: 6469 RVA: 0x0008BB9D File Offset: 0x00089D9D
		public virtual void OnEnable()
		{
			this.CachedTransformValid = false;
			this.InitRebooted = false;
			this.Register();
		}

		// Token: 0x06001946 RID: 6470 RVA: 0x0008BBB3 File Offset: 0x00089DB3
		public void Start()
		{
			this.InitRebooted = false;
		}

		// Token: 0x06001947 RID: 6471 RVA: 0x0008BBBC File Offset: 0x00089DBC
		public virtual void OnDisable()
		{
			this.Unregister();
		}

		// Token: 0x06001948 RID: 6472 RVA: 0x0008BBC4 File Offset: 0x00089DC4
		protected virtual void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x06001949 RID: 6473 RVA: 0x0008BBCC File Offset: 0x00089DCC
		protected virtual void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x0600194A RID: 6474 RVA: 0x0008BBD4 File Offset: 0x00089DD4
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

		// Token: 0x0600194B RID: 6475 RVA: 0x0008BCD3 File Offset: 0x00089ED3
		public virtual void PrepareExecute()
		{
			this.PrepareExecute(false);
		}

		// Token: 0x0600194C RID: 6476 RVA: 0x0008BCDC File Offset: 0x00089EDC
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

		// Token: 0x0600194D RID: 6477 RVA: 0x0008BD52 File Offset: 0x00089F52
		public void Execute(float dt)
		{
			this.Params.Execute(dt);
		}

		// Token: 0x0600194E RID: 6478 RVA: 0x0008BD60 File Offset: 0x00089F60
		public void PullResults()
		{
			this.PullResults(ref this.Params);
		}

		// Token: 0x0600194F RID: 6479 RVA: 0x0008BD70 File Offset: 0x00089F70
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

		// Token: 0x06001950 RID: 6480 RVA: 0x0008BE3C File Offset: 0x0008A03C
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

		// Token: 0x06001951 RID: 6481 RVA: 0x0008BF34 File Offset: 0x0008A134
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

		// Token: 0x04001A00 RID: 6656
		public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

		// Token: 0x04001A01 RID: 6657
		public bool TwoDDistanceCheck;

		// Token: 0x04001A02 RID: 6658
		public bool TwoDPositionInfluence;

		// Token: 0x04001A03 RID: 6659
		public bool TwoDRotationInfluence;

		// Token: 0x04001A04 RID: 6660
		public bool EnablePositionEffect = true;

		// Token: 0x04001A05 RID: 6661
		public bool EnableRotationEffect = true;

		// Token: 0x04001A06 RID: 6662
		public bool EnableScaleEffect;

		// Token: 0x04001A07 RID: 6663
		public bool GlobalReactionUpVector;

		// Token: 0x04001A08 RID: 6664
		public BoingManager.TranslationLockSpace TranslationLockSpace;

		// Token: 0x04001A09 RID: 6665
		public bool LockTranslationX;

		// Token: 0x04001A0A RID: 6666
		public bool LockTranslationY;

		// Token: 0x04001A0B RID: 6667
		public bool LockTranslationZ;

		// Token: 0x04001A0C RID: 6668
		public BoingWork.Params Params;

		// Token: 0x04001A0D RID: 6669
		public SharedBoingParams SharedParams;

		// Token: 0x04001A0E RID: 6670
		internal bool PositionSpringDirty;

		// Token: 0x04001A0F RID: 6671
		internal bool RotationSpringDirty;

		// Token: 0x04001A10 RID: 6672
		internal bool ScaleSpringDirty;

		// Token: 0x04001A11 RID: 6673
		internal bool CachedTransformValid;

		// Token: 0x04001A12 RID: 6674
		internal Vector3 CachedPositionLs;

		// Token: 0x04001A13 RID: 6675
		internal Vector3 CachedPositionWs;

		// Token: 0x04001A14 RID: 6676
		internal Vector3 RenderPositionWs;

		// Token: 0x04001A15 RID: 6677
		internal Quaternion CachedRotationLs;

		// Token: 0x04001A16 RID: 6678
		internal Quaternion CachedRotationWs;

		// Token: 0x04001A17 RID: 6679
		internal Quaternion RenderRotationWs;

		// Token: 0x04001A18 RID: 6680
		internal Vector3 CachedScaleLs;

		// Token: 0x04001A19 RID: 6681
		internal Vector3 RenderScaleLs;

		// Token: 0x04001A1A RID: 6682
		internal bool InitRebooted;
	}
}
