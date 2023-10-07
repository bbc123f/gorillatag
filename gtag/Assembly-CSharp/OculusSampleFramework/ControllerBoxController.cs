using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002E4 RID: 740
	public class ControllerBoxController : MonoBehaviour
	{
		// Token: 0x06001404 RID: 5124 RVA: 0x00071A31 File Offset: 0x0006FC31
		private void Awake()
		{
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x00071A33 File Offset: 0x0006FC33
		public void StartStopStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.StartStopStateChanged();
			}
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x00071A49 File Offset: 0x0006FC49
		public void DecreaseSpeedStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.DecreaseSpeedStateChanged();
			}
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x00071A5F File Offset: 0x0006FC5F
		public void IncreaseSpeedStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.IncreaseSpeedStateChanged();
			}
		}

		// Token: 0x06001408 RID: 5128 RVA: 0x00071A75 File Offset: 0x0006FC75
		public void SmokeButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.SmokeButtonStateChanged();
			}
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x00071A8B File Offset: 0x0006FC8B
		public void WhistleButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.WhistleButtonStateChanged();
			}
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x00071AA1 File Offset: 0x0006FCA1
		public void ReverseButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.ReverseButtonStateChanged();
			}
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x00071AB7 File Offset: 0x0006FCB7
		public void SwitchVisualization(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				HandsManager.Instance.SwitchVisualization();
			}
		}

		// Token: 0x0600140C RID: 5132 RVA: 0x00071ACC File Offset: 0x0006FCCC
		public void GoMoo(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._cowController.GoMooCowGo();
			}
		}

		// Token: 0x040016B1 RID: 5809
		[SerializeField]
		private TrainLocomotive _locomotive;

		// Token: 0x040016B2 RID: 5810
		[SerializeField]
		private CowController _cowController;
	}
}
