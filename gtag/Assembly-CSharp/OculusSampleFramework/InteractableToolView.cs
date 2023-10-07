using System;

namespace OculusSampleFramework
{
	// Token: 0x020002DF RID: 735
	public interface InteractableToolView
	{
		// Token: 0x17000152 RID: 338
		// (get) Token: 0x060013D5 RID: 5077
		InteractableTool InteractableTool { get; }

		// Token: 0x060013D6 RID: 5078
		void SetFocusedInteractable(Interactable interactable);

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060013D7 RID: 5079
		// (set) Token: 0x060013D8 RID: 5080
		bool EnableState { get; set; }

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060013D9 RID: 5081
		// (set) Token: 0x060013DA RID: 5082
		bool ToolActivateState { get; set; }
	}
}
