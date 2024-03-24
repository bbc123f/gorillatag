using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class CustomDebugUI : MonoBehaviour
{
	private void Awake()
	{
		CustomDebugUI.instance = this;
	}

	public RectTransform AddTextField(string label, int targetCanvas = 0)
	{
		RectTransform component = Object.Instantiate<RectTransform>(this.textPrefab).GetComponent<RectTransform>();
		component.GetComponentInChildren<InputField>().text = label;
		DebugUIBuilder debugUIBuilder = DebugUIBuilder.instance;
		typeof(DebugUIBuilder).GetMethod("AddRect", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(debugUIBuilder, new object[] { component, targetCanvas });
		return component;
	}

	public void RemoveFromCanvas(RectTransform element, int targetCanvas = 0)
	{
		DebugUIBuilder debugUIBuilder = DebugUIBuilder.instance;
		FieldInfo field = typeof(DebugUIBuilder).GetField("insertedElements", BindingFlags.Instance | BindingFlags.NonPublic);
		MethodInfo method = typeof(DebugUIBuilder).GetMethod("Relayout", BindingFlags.Instance | BindingFlags.NonPublic);
		List<RectTransform>[] array = (List<RectTransform>[])field.GetValue(debugUIBuilder);
		if (targetCanvas > -1 && targetCanvas < array.Length - 1)
		{
			array[targetCanvas].Remove(element);
			element.SetParent(null);
			method.Invoke(debugUIBuilder, new object[0]);
		}
	}

	public CustomDebugUI()
	{
	}

	[SerializeField]
	private RectTransform textPrefab;

	public static CustomDebugUI instance;

	private const BindingFlags privateFlags = BindingFlags.Instance | BindingFlags.NonPublic;
}
