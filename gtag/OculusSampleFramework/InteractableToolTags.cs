using System;

namespace OculusSampleFramework;

[Flags]
public enum InteractableToolTags
{
	None = 0,
	Ray = 1,
	Poke = 4,
	All = -1
}
