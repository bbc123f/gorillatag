using System;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class GTStripGameObjectFromBuildAttribute : Attribute
	{
		public string Condition
		{
			[CompilerGenerated]
			get
			{
				return this.<Condition>k__BackingField;
			}
		}

		public GTStripGameObjectFromBuildAttribute(string condition = "")
		{
			this.Condition = condition;
		}

		[CompilerGenerated]
		private readonly string <Condition>k__BackingField;
	}
}
