using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000A1 RID: 161
public class CustomDebugUI : MonoBehaviour
{
	// Token: 0x06000383 RID: 899 RVA: 0x000158EA File Offset: 0x00013AEA
	private void Awake()
	{
		CustomDebugUI.instance = this;
	}

	// Token: 0x06000384 RID: 900 RVA: 0x000158F2 File Offset: 0x00013AF2
	private void Start()
	{
	}

	// Token: 0x06000385 RID: 901 RVA: 0x000158F4 File Offset: 0x00013AF4
	private void Update()
	{
	}

	// Token: 0x06000386 RID: 902 RVA: 0x000158F8 File Offset: 0x00013AF8
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

	// Token: 0x06000387 RID: 903 RVA: 0x0001595C File Offset: 0x00013B5C
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
