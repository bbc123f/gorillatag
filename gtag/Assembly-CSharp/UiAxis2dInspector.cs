using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000CE RID: 206
public class UiAxis2dInspector : MonoBehaviour
{
	// Token: 0x0600048F RID: 1167 RVA: 0x0001D0BF File Offset: 0x0001B2BF
	public void SetExtents(Vector2 xExtent, Vector2 yExtent)
	{
		this.m_xExtent = xExtent;
		this.m_yExtent = yExtent;
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x0001D0CF File Offset: 0x0001B2CF
	public void SetName(string name)
	{
		this.m_nameLabel.text = name;
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x0001D0E0 File Offset: 0x0001B2E0
	public void SetValue(bool isTouching, Vector2 value)
	{
		this.m_handle.color = (isTouching ? Color.white : new Color(0.2f, 0.2f, 0.2f));
		Vector2 vector = new Vector2(Mathf.Clamp(value.x, this.m_xExtent.x, this.m_xExtent.y), Mathf.Clamp(value.y, this.m_yExtent.x, this.m_yExtent.y));
		this.m_valueLabel.text = string.Concat(new string[]
		{
			"[",
			vector.x.ToString("f2"),
			", ",
			vector.y.ToString("f2"),
			"]"
		});
		RectTransform component = this.m_handle.transform.parent.GetComponent<RectTransform>();
		Vector2 vector2 = (component != null) ? new Vector2(Mathf.Abs(component.sizeDelta.x), Mathf.Abs(component.sizeDelta.y)) : new Vector2(Mathf.Abs(this.m_xExtent.y - this.m_xExtent.x), Mathf.Abs(this.m_yExtent.y - this.m_yExtent.x));
		this.m_handle.transform.localPosition = new Vector3(vector.x * vector2.x * 0.5f, vector.y * vector2.y * 0.5f, 0f);
	}

	// Token: 0x04000539 RID: 1337
	[Header("Settings")]
	[SerializeField]
	private Vector2 m_xExtent = new Vector2(-1f, 1f);

	// Token: 0x0400053A RID: 1338
	[SerializeField]
	private Vector2 m_yExtent = new Vector2(-1f, 1f);

	// Token: 0x0400053B RID: 1339
	[Header("Components")]
	[SerializeField]
	private TextMeshProUGUI m_nameLabel;

	// Token: 0x0400053C RID: 1340
	[SerializeField]
	private TextMeshProUGUI m_valueLabel;

	// Token: 0x0400053D RID: 1341
	[SerializeField]
	private Image m_handle;
}
