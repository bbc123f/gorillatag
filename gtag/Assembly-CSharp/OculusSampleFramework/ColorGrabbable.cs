using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002C5 RID: 709
	public class ColorGrabbable : OVRGrabbable
	{
		// Token: 0x17000124 RID: 292
		// (get) Token: 0x0600131F RID: 4895 RVA: 0x0006E854 File Offset: 0x0006CA54
		// (set) Token: 0x06001320 RID: 4896 RVA: 0x0006E85C File Offset: 0x0006CA5C
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

		// Token: 0x06001321 RID: 4897 RVA: 0x0006E86B File Offset: 0x0006CA6B
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

		// Token: 0x06001322 RID: 4898 RVA: 0x0006E8A1 File Offset: 0x0006CAA1
		public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
		{
			base.GrabBegin(hand, grabPoint);
			this.UpdateColor();
		}

		// Token: 0x06001323 RID: 4899 RVA: 0x0006E8B1 File Offset: 0x0006CAB1
		public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
		{
			base.GrabEnd(linearVelocity, angularVelocity);
			this.UpdateColor();
		}

		// Token: 0x06001324 RID: 4900 RVA: 0x0006E8C4 File Offset: 0x0006CAC4
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

		// Token: 0x06001325 RID: 4901 RVA: 0x0006E978 File Offset: 0x0006CB78
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

		// Token: 0x040015FF RID: 5631
		public static readonly Color COLOR_GRAB = new Color(1f, 0.5f, 0f, 1f);

		// Token: 0x04001600 RID: 5632
		public static readonly Color COLOR_HIGHLIGHT = new Color(1f, 0f, 1f, 1f);

		// Token: 0x04001601 RID: 5633
		private Color m_color = Color.black;

		// Token: 0x04001602 RID: 5634
		private MeshRenderer[] m_meshRenderers;

		// Token: 0x04001603 RID: 5635
		private bool m_highlight;
	}
}
