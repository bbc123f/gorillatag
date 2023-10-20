using System;
using GorillaNetworking;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001BA RID: 442
public class PlayFabTitleDataTextDisplay : MonoBehaviour
{
	// Token: 0x06000B44 RID: 2884 RVA: 0x000456EC File Offset: 0x000438EC
	private void Start()
	{
		this.textBox.color = this.defaultTextColor;
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnNewTitleDataAdded));
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError));
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x0004574D File Offset: 0x0004394D
	private void OnPlayFabError(PlayFabError error)
	{
		this.textBox.text = this.fallbackText;
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x00045760 File Offset: 0x00043960
	private void OnTitleDataRequestComplete(string titleDataResult)
	{
		string text = titleDataResult.Replace("\\r", "\r").Replace("\\n", "\n");
		if (text[0] == '"' && text[text.Length - 1] == '"')
		{
			text = text.Substring(1, text.Length - 2);
		}
		this.textBox.text = text;
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x000457C6 File Offset: 0x000439C6
	private void OnNewTitleDataAdded(string key)
	{
		if (key == this.playfabKey)
		{
			this.textBox.color = this.newUpdateColor;
		}
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x000457E7 File Offset: 0x000439E7
	private void OnDestroy()
	{
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnNewTitleDataAdded));
	}

	// Token: 0x04000EA9 RID: 3753
	[SerializeField]
	private Text textBox;

	// Token: 0x04000EAA RID: 3754
	[SerializeField]
	private Color newUpdateColor = Color.magenta;

	// Token: 0x04000EAB RID: 3755
	[SerializeField]
	private Color defaultTextColor = Color.white;

	// Token: 0x04000EAC RID: 3756
	[Tooltip("PlayFab Title Data key from where to pull display text")]
	[SerializeField]
	private string playfabKey;

	// Token: 0x04000EAD RID: 3757
	[Tooltip("Text to display when error occurs during fetch")]
	[TextArea(3, 5)]
	[SerializeField]
	private string fallbackText;
}
