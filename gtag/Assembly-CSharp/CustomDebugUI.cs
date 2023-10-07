using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000A1 RID: 161
public class CustomDebugUI : MonoBehaviour
{
	// Token: 0x06000383 RID: 899 RVA: 0x00015B0E File Offset: 0x00013D0E
	private void Awake()
	{
		CustomDebugUI.instance = this;
	}

	// Token: 0x06000384 RID: 900 RVA: 0x00015B16 File Offset: 0x00013D16
	private void Start()
	{
	}

	// Token: 0x06000385 RID: 901 RVA: 0x00015B18 File Offset: 0x00013D18
	private void Update()
	{
	}

	// Token: 0x06000386 RID: 902 RVA: 0x00015B1C File Offset: 0x00013D1C
	public RectTransform AddTextField(string label, int targetCanvas = 0)
	{
		RectTransform component = Object.Instantiate<RectTransform>(this.textPrefab).GetComponent<RectTransform>();
		component.GetComponentInChildren<InputField>().text = label;
		DebugUIBuilder obj = DebugUIBuilder.instance;
		typeof(DebugUIBuilder).GetMethod("AddRect", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(obj, new object[]
		{
			component,
			targetCanvas
		});
		return component;
	}

	// Token: 0x06000387 RID: 903 RVA: 0x00015B80 File Offset: 0x00013D80
	public void RemoveFromCanvas(RectTransform element, int targetCanvas = 0)
	{
		DebugUIBuilder obj = DebugUIBuilder.instance;
		FieldInfo field = typeof(DebugUIBuilder).GetField("insertedElements", BindingFlags.Instance | BindingFlags.NonPublic);
		MethodInfo method = typeof(DebugUIBuilder).GetMethod("Relayout", BindingFlags.Instance | BindingFlags.NonPublic);
		List<RectTransform>[] array = (List<RectTransform>[])field.GetValue(obj);
		if (targetCanvas > -1 && targetCanvas < array.Length - 1)
		{
			array[targetCanvas].Remove(element);
			element.SetParent(null);
			method.Invoke(obj, new object[0]);
		}
	}

	// Token: 0x04000432 RID: 1074
	[SerializeField]
	private RectTransform textPrefab;

	// Token: 0x04000433 RID: 1075
	public static CustomDebugUI instance;

	// Token: 0x04000434 RID: 1076
	private const BindingFlags privateFlags = BindingFlags.Instance | BindingFlags.NonPublic;
}
