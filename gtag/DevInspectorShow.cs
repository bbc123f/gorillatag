﻿using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DevInspectorShow : Attribute
{
	public DevInspectorShow()
	{
	}
}
