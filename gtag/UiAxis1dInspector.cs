using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiAxis1dInspector : MonoBehaviour
{
	public void SetExtents(float minExtent, float maxExtent)
	{
		this.m_minExtent = minExtent;
		this.m_maxExtent = maxExtent;
	}

	public void SetName(string name)
	{
		this.m_nameLabel.text = name;
	}

	public void SetValue(float value)
	{
		this.m_valueLabel.text = string.Format("[{0}]", value.ToString("f2"));
		this.m_slider.minValue = Mathf.Min(value, this.m_minExtent);
		this.m_slider.maxValue = Mathf.Max(value, this.m_maxExtent);
		this.m_slider.value = value;
	}

	public UiAxis1dInspector()
	{
	}

	[Header("Settings")]
	[SerializeField]
	private float m_minExtent;

	[SerializeField]
	private float m_maxExtent = 1f;

	[Header("Components")]
	[SerializeField]
	private TextMeshProUGUI m_nameLabel;

	[SerializeField]
	private TextMeshProUGUI m_valueLabel;

	[SerializeField]
	private Slider m_slider;
}
