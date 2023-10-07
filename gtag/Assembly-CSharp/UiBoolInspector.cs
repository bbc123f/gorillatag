using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000CF RID: 207
public class UiBoolInspector : MonoBehaviour
{
	// Token: 0x06000493 RID: 1171 RVA: 0x0001D4CF File Offset: 0x0001B6CF
	public void SetName(string name)
	{
		this.m_nameLabel.text = name;
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x0001D4DD File Offset: 0x0001B6DD
	public void SetValue(bool value)
	{
		this.m_toggle.isOn = value;
	}

	// Token: 0x0400053E RID: 1342
	[Header("Components")]
	[SerializeField]
	private TextMeshProUGUI m_nameLabel;

	// Token: 0x0400053F RID: 1343
	[SerializeField]
	private Toggle m_toggle;
}
