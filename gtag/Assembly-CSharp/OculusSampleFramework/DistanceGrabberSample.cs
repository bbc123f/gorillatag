using System;
using UnityEngine;
using UnityEngine.UI;

namespace OculusSampleFramework
{
	// Token: 0x020002E3 RID: 739
	public class DistanceGrabberSample : MonoBehaviour
	{
		// Token: 0x1700015F RID: 351
		// (get) Token: 0x060013FC RID: 5116 RVA: 0x000718CF File Offset: 0x0006FACF
		// (set) Token: 0x060013FD RID: 5117 RVA: 0x000718D8 File Offset: 0x0006FAD8
		public bool UseSpherecast
		{
			get
			{
				return this.useSpherecast;
			}
			set
			{
				this.useSpherecast = value;
				for (int i = 0; i < this.m_grabbers.Length; i++)
				{
					this.m_grabbers[i].UseSpherecast = this.useSpherecast;
				}
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x060013FE RID: 5118 RVA: 0x00071912 File Offset: 0x0006FB12
		// (set) Token: 0x060013FF RID: 5119 RVA: 0x0007191C File Offset: 0x0006FB1C
		public bool AllowGrabThroughWalls
		{
			get
			{
				return this.allowGrabThroughWalls;
			}
			set
			{
				this.allowGrabThroughWalls = value;
				for (int i = 0; i < this.m_grabbers.Length; i++)
				{
					this.m_grabbers[i].m_preventGrabThroughWalls = !this.allowGrabThroughWalls;
				}
			}
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x0007195C File Offset: 0x0006FB5C
		private void Start()
		{
			DebugUIBuilder.instance.AddLabel("Distance Grab Sample", 0);
			DebugUIBuilder.instance.AddToggle("Use Spherecasting", new DebugUIBuilder.OnToggleValueChange(this.ToggleSphereCasting), this.useSpherecast, 0);
			DebugUIBuilder.instance.AddToggle("Grab Through Walls", new DebugUIBuilder.OnToggleValueChange(this.ToggleGrabThroughWalls), this.allowGrabThroughWalls, 0);
			DebugUIBuilder.instance.Show();
			float displayFrequency = OVRManager.display.displayFrequency;
			if (displayFrequency > 0.1f)
			{
				Debug.Log("Setting Time.fixedDeltaTime to: " + (1f / displayFrequency).ToString());
				Time.fixedDeltaTime = 1f / displayFrequency;
			}
		}

		// Token: 0x06001401 RID: 5121 RVA: 0x00071A07 File Offset: 0x0006FC07
		public void ToggleSphereCasting(Toggle t)
		{
			this.UseSpherecast = !this.UseSpherecast;
		}

		// Token: 0x06001402 RID: 5122 RVA: 0x00071A18 File Offset: 0x0006FC18
		public void ToggleGrabThroughWalls(Toggle t)
		{
			this.AllowGrabThroughWalls = !this.AllowGrabThroughWalls;
		}

		// Token: 0x040016AE RID: 5806
		private bool useSpherecast;

		// Token: 0x040016AF RID: 5807
		private bool allowGrabThroughWalls;

		// Token: 0x040016B0 RID: 5808
		[SerializeField]
		private DistanceGrabber[] m_grabbers;
	}
}
