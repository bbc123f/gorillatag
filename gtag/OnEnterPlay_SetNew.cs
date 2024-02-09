﻿using System;
using System.Reflection;
using UnityEngine;

public class OnEnterPlay_SetNew : OnEnterPlay_Attribute
{
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't SetNew non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		object obj = field.FieldType.GetConstructor(new Type[0]).Invoke(new object[0]);
		field.SetValue(null, obj);
	}
}
