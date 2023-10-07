using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000159 RID: 345
public class GorillaScoreCounter : MonoBehaviour
{
	// Token: 0x06000886 RID: 2182 RVA: 0x00034C94 File Offset: 0x00032E94
	private void Awake()
	{
		this.text = base.gameObject.GetComponent<Text>();
		if (this.isRedTeam)
		{
			this.attribute = "redScore";
			return;
		}
		this.attribute = "blueScore";
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x00034CC8 File Offset: 0x00032EC8
	private void Update()
	{
		if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties[this.attribute] != null)
		{
			this.text.text = ((int)PhotonNetwork.CurrentRoom.CustomProperties[this.attribute]).ToString();
		}
	}

	// Token: 0x04000AB4 RID: 2740
	public bool isRedTeam;

	// Token: 0x04000AB5 RID: 2741
	public Text text;

	// Token: 0x04000AB6 RID: 2742
	public string attribute;
}
