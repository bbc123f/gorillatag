using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002DA RID: 730
	public class FingerTipPokeToolView : MonoBehaviour, InteractableToolView
	{
		// Token: 0x17000146 RID: 326
		// (get) Token: 0x060013B3 RID: 5043 RVA: 0x00070B0E File Offset: 0x0006ED0E
		// (set) Token: 0x060013B4 RID: 5044 RVA: 0x00070B16 File Offset: 0x0006ED16
		public InteractableTool InteractableTool { get; set; }

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x060013B5 RID: 5045 RVA: 0x00070B1F File Offset: 0x0006ED1F
		// (set) Token: 0x060013B6 RID: 5046 RVA: 0x00070B2C File Offset: 0x0006ED2C
		public bool EnableState
		{
			get
			{
				return this._sphereMeshRenderer.enabled;
			}
			set
			{
				this._sphereMeshRenderer.enabled = value;
			}
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x060013B7 RID: 5047 RVA: 0x00070B3A File Offset: 0x0006ED3A
		// (set) Token: 0x060013B8 RID: 5048 RVA: 0x00070B42 File Offset: 0x0006ED42
		public bool ToolActivateState { get; set; }

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x060013B9 RID: 5049 RVA: 0x00070B4B File Offset: 0x0006ED4B
		// (set) Token: 0x060013BA RID: 5050 RVA: 0x00070B53 File Offset: 0x0006ED53
		public float SphereRadius { get; private set; }

		// Token: 0x060013BB RID: 5051 RVA: 0x00070B5C File Offset: 0x0006ED5C
		private void Awake()
		{
			this.SphereRadius = this._sphereMeshRenderer.transform.localScale.z * 0.5f;
		}

		// Token: 0x060013BC RID: 5052 RVA: 0x00070B7F File Offset: 0x0006ED7F
		public void SetFocusedInteractable(Interactable interactable)
		{
		}

		// Token: 0x04001677 RID: 5751
		[SerializeField]
		private MeshRenderer _sphereMeshRenderer;
	}
}
