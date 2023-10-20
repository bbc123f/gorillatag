using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002DC RID: 732
	public class FingerTipPokeToolView : MonoBehaviour, InteractableToolView
	{
		// Token: 0x17000148 RID: 328
		// (get) Token: 0x060013BA RID: 5050 RVA: 0x00070FDA File Offset: 0x0006F1DA
		// (set) Token: 0x060013BB RID: 5051 RVA: 0x00070FE2 File Offset: 0x0006F1E2
		public InteractableTool InteractableTool { get; set; }

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x060013BC RID: 5052 RVA: 0x00070FEB File Offset: 0x0006F1EB
		// (set) Token: 0x060013BD RID: 5053 RVA: 0x00070FF8 File Offset: 0x0006F1F8
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

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x060013BE RID: 5054 RVA: 0x00071006 File Offset: 0x0006F206
		// (set) Token: 0x060013BF RID: 5055 RVA: 0x0007100E File Offset: 0x0006F20E
		public bool ToolActivateState { get; set; }

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x060013C0 RID: 5056 RVA: 0x00071017 File Offset: 0x0006F217
		// (set) Token: 0x060013C1 RID: 5057 RVA: 0x0007101F File Offset: 0x0006F21F
		public float SphereRadius { get; private set; }

		// Token: 0x060013C2 RID: 5058 RVA: 0x00071028 File Offset: 0x0006F228
		private void Awake()
		{
			this.SphereRadius = this._sphereMeshRenderer.transform.localScale.z * 0.5f;
		}

		// Token: 0x060013C3 RID: 5059 RVA: 0x0007104B File Offset: 0x0006F24B
		public void SetFocusedInteractable(Interactable interactable)
		{
		}

		// Token: 0x04001684 RID: 5764
		[SerializeField]
		private MeshRenderer _sphereMeshRenderer;
	}
}
