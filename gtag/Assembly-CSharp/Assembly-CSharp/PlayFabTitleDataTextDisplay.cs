using System;
using GorillaNetworking;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayFabTitleDataTextDisplay : MonoBehaviour
{
	private void Start()
	{
		this.textBox.color = this.defaultTextColor;
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnNewTitleDataAdded));
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError));
	}

	private void OnPlayFabError(PlayFabError error)
	{
		this.textBox.text = this.fallbackText;
	}

	private void OnTitleDataRequestComplete(string titleDataResult)
	{
		string text = titleDataResult.Replace("\\r", "\r").Replace("\\n", "\n");
		if (text[0] == '"' && text[text.Length - 1] == '"')
		{
			text = text.Substring(1, text.Length - 2);
		}
		this.textBox.text = text;
	}

	private void OnNewTitleDataAdded(string key)
	{
		if (key == this.playfabKey)
		{
			this.textBox.color = this.newUpdateColor;
		}
	}

	private void OnDestroy()
	{
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnNewTitleDataAdded));
	}

	[SerializeField]
	private Text textBox;

	[SerializeField]
	private Color newUpdateColor = Color.magenta;

	[SerializeField]
	private Color defaultTextColor = Color.white;

	[Tooltip("PlayFab Title Data key from where to pull display text")]
	[SerializeField]
	private string playfabKey;

	[Tooltip("Text to display when error occurs during fetch")]
	[TextArea(3, 5)]
	[SerializeField]
	private string fallbackText;
}
