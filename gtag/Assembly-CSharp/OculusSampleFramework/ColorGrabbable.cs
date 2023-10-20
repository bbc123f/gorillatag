using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002C7 RID: 711
	public class ColorGrabbable : OVRGrabbable
	{
		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06001326 RID: 4902 RVA: 0x0006ED20 File Offset: 0x0006CF20
		// (set) Token: 0x06001327 RID: 4903 RVA: 0x0006ED28 File Offset: 0x0006CF28
		public bool Highlight
		{
			get
			{
				return this.m_highlight;
			}
			set
			{
				this.m_highlight = value;
				this.UpdateColor();
			}
		}

		// Token: 0x06001328 RID: 4904 RVA: 0x0006ED37 File Offset: 0x0006CF37
		protected void UpdateColor()
		{
			if (base.isGrabbed)
			{
				this.SetColor(ColorGrabbable.COLOR_GRAB);
				return;
			}
			if (this.Highlight)
			{
				this.SetColor(ColorGrabbable.COLOR_HIGHLIGHT);
				return;
			}
			this.SetColor(this.m_color);
		}

		// Token: 0x06001329 RID: 4905 RVA: 0x0006ED6D File Offset: 0x0006CF6D
		public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
		{
			base.GrabBegin(hand, grabPoint);
			this.UpdateColor();
		}

		// Token: 0x0600132A RID: 4906 RVA: 0x0006ED7D File Offset: 0x0006CF7D
		public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
		{
			base.GrabEnd(linearVelocity, angularVelocity);
			this.UpdateColor();
		}

		// Token: 0x0600132B RID: 4907 RVA: 0x0006ED90 File Offset: 0x0006CF90
		private void Awake()
		{
			if (this.m_grabPoints.Length == 0)
			{
				Collider component = base.GetComponent<Collider>();
				if (component == null)
				{
					throw new ArgumentException("Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
				}
				this.m_grabPoints = new Collider[]
				{
					component
				};
				this.m_meshRenderers = new MeshRenderer[1];
				this.m_meshRenderers[0] = base.GetComponent<MeshRenderer>();
			}
			else
			{
				this.m_meshRenderers = base.GetComponentsInChildren<MeshRenderer>();
			}
			this.m_color = new Color(Random.Range(0.1f, 0.95f), Random.Range(0.1f, 0.95f), Random.Range(0.1f, 0.95f), 1f);
			this.SetColor(this.m_color);
		}

		// Token: 0x0600132C RID: 4908 RVA: 0x0006EE44 File Offset: 0x0006D044
		private void SetColor(Color color)
		{
			for (int i = 0; i < this.m_meshRenderers.Length; i++)
			{
				MeshRenderer meshRenderer = this.m_meshRenderers[i];
				for (int j = 0; j < meshRenderer.materials.Length; j++)
				{
					meshRenderer.materials[j].color = color;
				}
			}
		}

		// Token: 0x0400160C RID: 5644
		public static readonly Color COLOR_GRAB = new Color(1f, 0.5f, 0f, 1f);

		// Token: 0x0400160D RID: 5645
		public static readonly Color COLOR_HIGHLIGHT = new Color(1f, 0f, 1f, 1f);

		// Token: 0x0400160E RID: 5646
		private Color m_color = Color.black;

		// Token: 0x0400160F RID: 5647
		private MeshRenderer[] m_meshRenderers;

		// Token: 0x04001610 RID: 5648
		private bool m_highlight;
	}
}
