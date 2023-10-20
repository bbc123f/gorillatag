using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002C8 RID: 712
	public class DistanceGrabbable : OVRGrabbable
	{
		// Token: 0x17000127 RID: 295
		// (get) Token: 0x0600132F RID: 4911 RVA: 0x0006EEDF File Offset: 0x0006D0DF
		// (set) Token: 0x06001330 RID: 4912 RVA: 0x0006EEE7 File Offset: 0x0006D0E7
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

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06001331 RID: 4913 RVA: 0x0006EEF6 File Offset: 0x0006D0F6
		// (set) Token: 0x06001332 RID: 4914 RVA: 0x0006EEFE File Offset: 0x0006D0FE
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

		// Token: 0x06001333 RID: 4915 RVA: 0x0006EF10 File Offset: 0x0006D110
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

		// Token: 0x06001334 RID: 4916 RVA: 0x0006EF74 File Offset: 0x0006D174
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

		// Token: 0x04001611 RID: 5649
		public string m_materialColorField;

		// Token: 0x04001612 RID: 5650
		private GrabbableCrosshair m_crosshair;

		// Token: 0x04001613 RID: 5651
		private GrabManager m_crosshairManager;

		// Token: 0x04001614 RID: 5652
		private Renderer m_renderer;

		// Token: 0x04001615 RID: 5653
		private MaterialPropertyBlock m_mpb;

		// Token: 0x04001616 RID: 5654
		private bool m_inRange;

		// Token: 0x04001617 RID: 5655
		private bool m_targeted;
	}
}
