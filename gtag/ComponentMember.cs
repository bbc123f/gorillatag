using System;
using System.Runtime.CompilerServices;

public class ComponentMember
{
	public string Name
	{
		[CompilerGenerated]
		get
		{
			return this.<Name>k__BackingField;
		}
	}

	public string Value
	{
		get
		{
			return this.getValue();
		}
	}

	public bool IsStarred
	{
		[CompilerGenerated]
		get
		{
			return this.<IsStarred>k__BackingField;
		}
	}

	public string Color
	{
		[CompilerGenerated]
		get
		{
			return this.<Color>k__BackingField;
		}
	}

	public ComponentMember(string name, Func<string> getValue, bool isStarred, string color)
	{
		this.Name = name;
		this.getValue = getValue;
		this.IsStarred = isStarred;
		this.Color = color;
	}

	[CompilerGenerated]
	private readonly string <Name>k__BackingField;

	[CompilerGenerated]
	private readonly bool <IsStarred>k__BackingField;

	[CompilerGenerated]
	private readonly string <Color>k__BackingField;

	private Func<string> getValue;

	public string computedPrefix;

	public string computedSuffix;
}
