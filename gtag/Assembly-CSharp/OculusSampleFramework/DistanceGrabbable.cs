using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002C6 RID: 710
	public class DistanceGrabbable : OVRGrabbable
	{
		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06001328 RID: 4904 RVA: 0x0006EA13 File Offset: 0x0006CC13
		// (set) Token: 0x06001329 RID: 4905 RVA: 0x0006EA1B File Offset: 0x0006CC1B
		public bool InRange
		{
			get
			{
				return this.m_inRange;
			}
			set
			{
				this.m_inRange = value;
				this.RefreshCrosshair();
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x0600132A RID: 4906 RVA: 0x0006EA2A File Offset: 0x0006CC2A
		// (set) Token: 0x0600132B RID: 4907 RVA: 0x0006EA32 File Offset: 0x0006CC32
		public bool Targeted
		{
			get
			{
				return this.m_targeted;
			}
			set
			{
				this.m_targeted = value;
				this.RefreshCrosshair();
			}
		}

		// Token: 0x0600132C RID: 4908 RVA: 0x0006EA44 File Offset: 0x0006CC44
		protected override void Start()
		{
			base.Start();
			this.m_crosshair = base.gameObject.GetComponentInChildren<GrabbableCrosshair>();
			this.m_renderer = base.gameObject.GetComponent<Renderer>();
			this.m_crosshairManager = Object.FindObjectOfType<GrabManager>();
			this.m_mpb = new MaterialPropertyBlock();
			this.RefreshCrosshair();
			this.m_renderer.SetPropertyBlock(this.m_mpb);
		}

		// Token: 0x0600132D RID: 4909 RVA: 0x0006EAA8 File Offset: 0x0006CCA8
		private void RefreshCrosshair()
		{
			if (this.m_crosshair)
			{
				if (base.isGrabbed)
				{
					this.m_crosshair.SetState(GrabbableCrosshair.CrosshairState.Disabled);
				}
				else if (!this.InRange)
				{
					this.m_crosshair.SetState(GrabbableCrosshair.CrosshairState.Disabled);
				}
				else
				{
					this.m_crosshair.SetState(this.Targeted ? GrabbableCrosshair.CrosshairState.Targeted : GrabbableCrosshair.CrosshairState.Enabled);
				}
			}
			if (this.m_materialColorField != null)
			{
				this.m_renderer.GetPropertyBlock(this.m_mpb);
				if (base.isGrabbed || !this.InRange)
				{
					this.m_mpb.SetColor(this.m_materialColorField, this.m_crosshairManager.OutlineColorOutOfRange);
				}
				else if (this.Targeted)
				{
					this.m_mpb.SetColor(this.m_materialColorField, this.m_crosshairManager.OutlineColorHighlighted);
				}
				else
				{
					this.m_mpb.SetColor(this.m_materialColorField, this.m_crosshairManager.OutlineColorInRange);
				}
				this.m_renderer.SetPropertyBlock(this.m_mpb);
			}
		}

		// Token: 0x04001604 RID: 5636
		public string m_materialColorField;

		// Token: 0x04001605 RID: 5637
		private GrabbableCrosshair m_crosshair;

		// Token: 0x04001606 RID: 5638
		private GrabManager m_crosshairManager;

		// Token: 0x04001607 RID: 5639
		private Renderer m_renderer;

		// Token: 0x04001608 RID: 5640
		private MaterialPropertyBlock m_mpb;

		// Token: 0x04001609 RID: 5641
		private bool m_inRange;

		// Token: 0x0400160A RID: 5642
		private bool m_targeted;
	}
}
