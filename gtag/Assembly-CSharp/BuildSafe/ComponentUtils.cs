using System;
using UnityEngine;

namespace BuildSafe
{
	public static class ComponentUtils
	{
		public static Hash128 GetComponentID(Component c, string k)
		{
			return ComponentUtils.GetComponentID(c, StaticHash.Calculate(k));
		}

		public static Hash128 GetComponentID(Component c, int k = 0)
		{
			return default(Hash128);
		}
	}
}
