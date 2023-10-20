using System;

namespace GorillaTag
{
	// Token: 0x02000309 RID: 777
	public interface IGuidedRefReceiver : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x060015A0 RID: 5536
		bool GuidRefResolveReference(int fieldId, IGuidedRefTarget target);
	}
}
