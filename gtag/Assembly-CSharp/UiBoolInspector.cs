using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiBoolInspector : MonoBehaviour
{
	public void SetName(string name)
	{
		this.m_nameLabel.text = name;
	}

	public void SetValue(bool value)
	{
		this.m_toggle.isOn = value;
	}

	public UiBoolInspector()
	{
	}

	[Header("Components")]
	[SerializeField]
	private TextMeshProUGUI m_nameLabel;

	[SerializeField]
	private Toggle m_toggle;
}
