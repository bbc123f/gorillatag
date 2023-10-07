using System;

namespace GorillaTag
{
	// Token: 0x02000307 RID: 775
	public interface IGuidedRefReceiver : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x06001597 RID: 5527
		bool GuidRefResolveReference(int fieldId, IGuidedRefTarget target);
	}
}
