using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiAxis2dInspector : MonoBehaviour
{
	public void SetExtents(Vector2 xExtent, Vector2 yExtent)
	{
		this.m_xExtent = xExtent;
		this.m_yExtent = yExtent;
	}

	public void SetName(string name)
	{
		this.m_nameLabel.text = name;
	}

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
		Vector2 vector2 = ((component != null) ? new Vector2(Mathf.Abs(component.sizeDelta.x), Mathf.Abs(component.sizeDelta.y)) : new Vector2(Mathf.Abs(this.m_xExtent.y - this.m_xExtent.x), Mathf.Abs(this.m_yExtent.y - this.m_yExtent.x)));
		this.m_handle.transform.localPosition = new Vector3(vector.x * vector2.x * 0.5f, vector.y * vector2.y * 0.5f, 0f);
	}

	public UiAxis2dInspector()
	{
	}

	[Header("Settings")]
	[SerializeField]
	private Vector2 m_xExtent = new Vector2(-1f, 1f);

	[SerializeField]
	private Vector2 m_yExtent = new Vector2(-1f, 1f);

	[Header("Components")]
	[SerializeField]
	private TextMeshProUGUI m_nameLabel;

	[SerializeField]
	private TextMeshProUGUI m_valueLabel;

	[SerializeField]
	private Image m_handle;
}
