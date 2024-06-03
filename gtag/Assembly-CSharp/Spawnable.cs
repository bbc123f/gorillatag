using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class Spawnable : ISerializationCallbackReceiver
{
	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		if (this.ClassificationLabel != "")
		{
			this._editorClassificationIndex = Spawnable.<OnAfterDeserialize>g__IndexOf|4_0(this.ClassificationLabel, OVRSceneManager.Classification.List);
			if (this._editorClassificationIndex < 0)
			{
				Debug.LogError("[Spawnable] OnAfterDeserialize() " + this.ClassificationLabel + " not found. The Classification list in OVRSceneManager has likely changed");
				return;
			}
		}
		else
		{
			this._editorClassificationIndex = 0;
		}
	}

	public Spawnable()
	{
	}

	[CompilerGenerated]
	internal static int <OnAfterDeserialize>g__IndexOf|4_0(string label, IEnumerable<string> collection)
	{
		int num = 0;
		using (IEnumerator<string> enumerator = collection.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == label)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	public SimpleResizable ResizablePrefab;

	public string ClassificationLabel = "";

	[SerializeField]
	private int _editorClassificationIndex;
}
