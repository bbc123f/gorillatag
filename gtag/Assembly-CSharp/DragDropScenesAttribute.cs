﻿using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class DragDropScenesAttribute : Attribute
{
	public DragDropScenesAttribute()
	{
	}
}
