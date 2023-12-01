using System;
using TMPro;
using UnityEngine;

public class UiVectorInspector : MonoBehaviour
{
	public void SetName(string name)
	{
		this.m_nameLabel.text = name;
	}

	public void SetValue(bool value)
	{
		this.m_valueLabel.text = string.Format("[{0}]", value);
	}

	[Header("Components")]
	[SerializeField]
	private TextMeshProUGUI m_nameLabel;

	[SerializeField]
	private TextMeshProUGUI m_valueLabel;
}
