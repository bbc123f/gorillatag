using System;

namespace OculusSampleFramework
{
	// Token: 0x020002E1 RID: 737
	public interface InteractableToolView
	{
		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060013DC RID: 5084
		InteractableTool InteractableTool { get; }

		// Token: 0x060013DD RID: 5085
		void SetFocusedInteractable(Interactable interactable);

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060013DE RID: 5086
		// (set) Token: 0x060013DF RID: 5087
		bool EnableState { get; set; }

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060013E0 RID: 5088
		// (set) Token: 0x060013E1 RID: 5089
		bool ToolActivateState { get; set; }
	}
}
