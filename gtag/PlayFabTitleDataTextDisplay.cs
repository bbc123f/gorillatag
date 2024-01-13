using GorillaNetworking;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabTitleDataTextDisplay : MonoBehaviour
{
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

	private void Start()
	{
		textBox.color = defaultTextColor;
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(OnNewTitleDataAdded);
		PlayFabTitleDataCache.Instance.GetTitleData(playfabKey, OnTitleDataRequestComplete, OnPlayFabError);
	}

	private void OnPlayFabError(PlayFabError error)
	{
		textBox.text = fallbackText;
	}

	private void OnTitleDataRequestComplete(string titleDataResult)
	{
		string text = titleDataResult.Replace("\\r", "\r").Replace("\\n", "\n");
		if (text[0] == '"' && text[text.Length - 1] == '"')
		{
			text = text.Substring(1, text.Length - 2);
		}
		textBox.text = text;
	}

	private void OnNewTitleDataAdded(string key)
	{
		if (key == playfabKey)
		{
			textBox.color = newUpdateColor;
		}
	}

	private void OnDestroy()
	{
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(OnNewTitleDataAdded);
	}
}
