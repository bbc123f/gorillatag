using System;
using UnityEngine;

namespace OculusSampleFramework
{
	public class DistanceGrabbable : OVRGrabbable
	{
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

		public string m_materialColorField;

		private GrabbableCrosshair m_crosshair;

		private GrabManager m_crosshairManager;

		private Renderer m_renderer;

		private MaterialPropertyBlock m_mpb;

		private bool m_inRange;

		private bool m_targeted;
	}
}
