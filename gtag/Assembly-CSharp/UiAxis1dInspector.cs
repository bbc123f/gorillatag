using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000CD RID: 205
public class UiAxis1dInspector : MonoBehaviour
{
	// Token: 0x0600048B RID: 1163 RVA: 0x0001D024 File Offset: 0x0001B224
	public void SetExtents(float minExtent, float maxExtent)
	{
		this.m_minExtent = minExtent;
		this.m_maxExtent = maxExtent;
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x0001D034 File Offset: 0x0001B234
	public void SetName(string name)
	{
		this.m_nameLabel.text = name;
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x0001D044 File Offset: 0x0001B244
	public void SetValue(float value)
	{
		this.m_valueLabel.text = string.Format("[{0}]", value.ToString("f2"));
		this.m_slider.minValue = Mathf.Min(value, this.m_minExtent);
		this.m_slider.maxValue = Mathf.Max(value, this.m_maxExtent);
		this.m_slider.value = value;
	}

	// Token: 0x04000534 RID: 1332
	[Header("Settings")]
	[SerializeField]
	private float m_minExtent;

	// Token: 0x04000535 RID: 1333
	[SerializeField]
	private float m_maxExtent = 1f;

	// Token: 0x04000536 RID: 1334
	[Header("Components")]
	[SerializeField]
	private TextMeshProUGUI m_nameLabel;

	// Token: 0x04000537 RID: 1335
	[SerializeField]
	private TextMeshProUGUI m_valueLabel;

	// Token: 0x04000538 RID: 1336
	[SerializeField]
	private Slider m_slider;
}
