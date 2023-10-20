using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000158 RID: 344
public class GorillaPlayerCounter : MonoBehaviour
{
	// Token: 0x06000884 RID: 2180 RVA: 0x00034A22 File Offset: 0x00032C22
	private void Awake()
	{
		this.text = base.gameObject.GetComponent<Text>();
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x00034A38 File Offset: 0x00032C38
	private void Update()
	{
		if (PhotonNetwork.CurrentRoom != null)
		{
			int num = 0;
			foreach (KeyValuePair<int, Player> keyValuePair in PhotonNetwork.CurrentRoom.Players)
			{
				if ((bool)keyValuePair.Value.CustomProperties["isRedTeam"] == this.isRedTeam)
				{
					num++;
				}
			}
			this.text.text = num.ToString();
		}
	}

	// Token: 0x04000AB1 RID: 2737
	public bool isRedTeam;

	// Token: 0x04000AB2 RID: 2738
	public Text text;

	// Token: 0x04000AB3 RID: 2739
	public string attribute;
}
