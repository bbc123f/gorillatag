using System;
using System.Reflection;
using UnityEngine;

public class OnEnterPlay_Clear : OnEnterPlay_Attribute
{
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Clear non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.FieldType.GetMethod("Clear").Invoke(field.GetValue(null), new object[0]);
	}

	public OnEnterPlay_Clear()
	{
	}
}
