using System;
using GorillaNetworking;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001B9 RID: 441
public class PlayFabTitleDataTextDisplay : MonoBehaviour
{
	// Token: 0x06000B3E RID: 2878 RVA: 0x00045484 File Offset: 0x00043684
	private void Start()
	{
		this.textBox.color = this.defaultTextColor;
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnNewTitleDataAdded));
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError));
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x000454E5 File Offset: 0x000436E5
	private void OnPlayFabError(PlayFabError error)
	{
		this.textBox.text = this.fallbackText;
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x000454F8 File Offset: 0x000436F8
	private void OnTitleDataRequestComplete(string titleDataResult)
	{
		string text = titleDataResult.Replace("\\r", "\r").Replace("\\n", "\n");
		if (text[0] == '"' && text[text.Length - 1] == '"')
		{
			text = text.Substring(1, text.Length - 2);
		}
		this.textBox.text = text;
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x0004555E File Offset: 0x0004375E
	private void OnNewTitleDataAdded(string key)
	{
		if (key == this.playfabKey)
		{
			this.textBox.color = this.newUpdateColor;
		}
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x0004557F File Offset: 0x0004377F
	private void OnDestroy()
	{
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnNewTitleDataAdded));
	}

	// Token: 0x04000EA5 RID: 3749
	[SerializeField]
	private Text textBox;

	// Token: 0x04000EA6 RID: 3750
	[SerializeField]
	private Color newUpdateColor = Color.magenta;

	// Token: 0x04000EA7 RID: 3751
	[SerializeField]
	private Color defaultTextColor = Color.white;

	// Token: 0x04000EA8 RID: 3752
	[Tooltip("PlayFab Title Data key from where to pull display text")]
	[SerializeField]
	private string playfabKey;

	// Token: 0x04000EA9 RID: 3753
	[Tooltip("Text to display when error occurs during fetch")]
	[TextArea(3, 5)]
	[SerializeField]
	private string fallbackText;
}
