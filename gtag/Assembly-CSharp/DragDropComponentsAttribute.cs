﻿using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class DragDropComponentsAttribute : Attribute
{
	public DragDropComponentsAttribute()
	{
	}
}
