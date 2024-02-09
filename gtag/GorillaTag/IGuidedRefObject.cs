using System;

namespace GorillaTag
{
	public interface IGuidedRefObject
	{
		int GetInstanceID();

		void GuidedRefInitialize();
	}
}
