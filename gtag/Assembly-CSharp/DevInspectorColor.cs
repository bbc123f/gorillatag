using System;
using System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DevInspectorColor : Attribute
{
	public string Color
	{
		[CompilerGenerated]
		get
		{
			return this.<Color>k__BackingField;
		}
	}

	public DevInspectorColor(string color)
	{
		this.Color = color;
	}

	[CompilerGenerated]
	private readonly string <Color>k__BackingField;
}
