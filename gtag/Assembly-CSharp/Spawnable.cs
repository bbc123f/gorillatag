using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020000C4 RID: 196
[Serializable]
public class Spawnable : ISerializationCallbackReceiver
{
	// Token: 0x06000446 RID: 1094 RVA: 0x0001BDE9 File Offset: 0x00019FE9
	public void OnBeforeSerialize()
	{
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x0001BDEC File Offset: 0x00019FEC
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

	// Token: 0x06000449 RID: 1097 RVA: 0x0001BE60 File Offset: 0x0001A060
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

	// Token: 0x040004F4 RID: 1268
	public SimpleResizable ResizablePrefab;

	// Token: 0x040004F5 RID: 1269
	public string ClassificationLabel = "";

	// Token: 0x040004F6 RID: 1270
	[SerializeField]
	private int _editorClassificationIndex;
}
