using System;
using UnityEngine;
using UnityEngine.UI;

namespace OculusSampleFramework
{
	// Token: 0x020002E5 RID: 741
	public class DistanceGrabberSample : MonoBehaviour
	{
		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06001403 RID: 5123 RVA: 0x00071D9B File Offset: 0x0006FF9B
		// (set) Token: 0x06001404 RID: 5124 RVA: 0x00071DA4 File Offset: 0x0006FFA4
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

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06001405 RID: 5125 RVA: 0x00071DDE File Offset: 0x0006FFDE
		// (set) Token: 0x06001406 RID: 5126 RVA: 0x00071DE8 File Offset: 0x0006FFE8
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

		// Token: 0x06001407 RID: 5127 RVA: 0x00071E28 File Offset: 0x00070028
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

		// Token: 0x06001408 RID: 5128 RVA: 0x00071ED3 File Offset: 0x000700D3
		public void ToggleSphereCasting(Toggle t)
		{
			this.UseSpherecast = !this.UseSpherecast;
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x00071EE4 File Offset: 0x000700E4
		public void ToggleGrabThroughWalls(Toggle t)
		{
			this.AllowGrabThroughWalls = !this.AllowGrabThroughWalls;
		}

		// Token: 0x040016BB RID: 5819
		private bool useSpherecast;

		// Token: 0x040016BC RID: 5820
		private bool allowGrabThroughWalls;

		// Token: 0x040016BD RID: 5821
		[SerializeField]
		private DistanceGrabber[] m_grabbers;
	}
}
