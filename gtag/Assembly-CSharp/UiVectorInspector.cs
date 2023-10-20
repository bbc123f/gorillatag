using System;
using TMPro;
using UnityEngine;

// Token: 0x020000D2 RID: 210
public class UiVectorInspector : MonoBehaviour
{
	// Token: 0x060004A7 RID: 1191 RVA: 0x0001D701 File Offset: 0x0001B901
	public void SetName(string name)
	{
		this.m_nameLabel.text = name;
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x0001D70F File Offset: 0x0001B90F
	public void SetValue(bool value)
	{
		this.m_valueLabel.text = string.Format("[{0}]", value);
	}

	// Token: 0x04000556 RID: 1366
	[Header("Components")]
	[SerializeField]
	private TextMeshProUGUI m_nameLabel;

	// Token: 0x04000557 RID: 1367
	[SerializeField]
	private TextMeshProUGUI m_valueLabel;
}
