using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002E6 RID: 742
	public class ControllerBoxController : MonoBehaviour
	{
		// Token: 0x0600140B RID: 5131 RVA: 0x00071EFD File Offset: 0x000700FD
		private void Awake()
		{
		}

		// Token: 0x0600140C RID: 5132 RVA: 0x00071EFF File Offset: 0x000700FF
		public void StartStopStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.StartStopStateChanged();
			}
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x00071F15 File Offset: 0x00070115
		public void DecreaseSpeedStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.DecreaseSpeedStateChanged();
			}
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x00071F2B File Offset: 0x0007012B
		public void IncreaseSpeedStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.IncreaseSpeedStateChanged();
			}
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x00071F41 File Offset: 0x00070141
		public void SmokeButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.SmokeButtonStateChanged();
			}
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x00071F57 File Offset: 0x00070157
		public void WhistleButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.WhistleButtonStateChanged();
			}
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x00071F6D File Offset: 0x0007016D
		public void ReverseButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.ReverseButtonStateChanged();
			}
		}

		// Token: 0x06001412 RID: 5138 RVA: 0x00071F83 File Offset: 0x00070183
		public void SwitchVisualization(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				HandsManager.Instance.SwitchVisualization();
			}
		}

		// Token: 0x06001413 RID: 5139 RVA: 0x00071F98 File Offset: 0x00070198
		public void GoMoo(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._cowController.GoMooCowGo();
			}
		}

		// Token: 0x040016BE RID: 5822
		[SerializeField]
		private TrainLocomotive _locomotive;

		// Token: 0x040016BF RID: 5823
		[SerializeField]
		private CowController _cowController;
	}
}
