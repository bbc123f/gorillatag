﻿using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DevInspectorHide : Attribute
{
	public DevInspectorHide()
	{
	}
}
