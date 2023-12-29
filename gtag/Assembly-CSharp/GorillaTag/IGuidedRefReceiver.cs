using System;

namespace GorillaTag
{
	public interface IGuidedRefReceiver : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		bool GuidRefResolveReference(int fieldId, IGuidedRefTarget target);
	}
}
