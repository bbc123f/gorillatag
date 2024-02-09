﻿using System;
using System.Reflection;
using UnityEngine;

public class OnExitPlay_Set : OnExitPlay_Attribute
{
	public OnExitPlay_Set(object value)
	{
		this.value = value;
	}

	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Set non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.SetValue(null, this.value);
	}

	private object value;
}
