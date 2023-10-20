using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02000323 RID: 803
	[RequireComponent(typeof(PhotonView))]
	[RequireComponent(typeof(AudioSource))]
	public class SportScoreboard : MonoBehaviourPunCallbacks, IPunObservable
	{
		// Token: 0x06001666 RID: 5734 RVA: 0x0007CAD4 File Offset: 0x0007ACD4
		private void Awake()
		{
			this.audioSource = base.GetComponent<AudioSource>();
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				this.teamScores.Add(0);
				this.teamScoresPrev.Add(0);
			}
		}

		// Token: 0x06001667 RID: 5735 RVA: 0x0007CB1C File Offset: 0x0007AD1C
		private void UpdateScoreboard()
		{
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				int num = this.teamScores[i];
				if (this.teamParameters[i].teamScoreDisplayColumn0)
				{
					this.teamParameters[i].teamScoreDisplayColumn0.SetUVOffset(num % 10);
				}
				if (this.teamParameters[i].teamScoreDisplayColumn1 != null)
				{
					this.teamParameters[i].teamScoreDisplayColumn1.SetUVOffset(num / 10 % 10);
				}
			}
		}

		// Token: 0x06001668 RID: 5736 RVA: 0x0007CBB8 File Offset: 0x0007ADB8
		private void OnScoreUpdated()
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				if (this.teamScores[i] > this.teamScoresPrev[i] && this.teamParameters[i].goalScoredAudio != null && this.teamScores[i] < this.matchEndScore)
				{
					this.audioSource.PlayOneShot(this.teamParameters[i].goalScoredAudio);
				}
				this.teamScoresPrev[i] = this.teamScores[i];
			}
			if (!this.runningMatchEndCoroutine)
			{
				for (int j = 0; j < this.teamScores.Count; j++)
				{
					if (this.teamScores[j] >= this.matchEndScore)
					{
						base.StartCoroutine(this.MatchEndCoroutine(j));
						break;
					}
				}
			}
			this.UpdateScoreboard();
		}

		// Token: 0x06001669 RID: 5737 RVA: 0x0007CCA4 File Offset: 0x0007AEA4
		public void TeamScored(int team)
		{
			if (base.photonView.IsMine && !this.runningMatchEndCoroutine)
			{
				if (team >= 0 && team < this.teamScores.Count)
				{
					this.teamScores[team] = this.teamScores[team] + 1;
				}
				this.OnScoreUpdated();
			}
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x0007CCF8 File Offset: 0x0007AEF8
		public void ResetScores()
		{
			if (base.photonView.IsMine && !this.runningMatchEndCoroutine)
			{
				for (int i = 0; i < this.teamScores.Count; i++)
				{
					this.teamScores[i] = 0;
				}
				this.OnScoreUpdated();
			}
		}

		// Token: 0x0600166B RID: 5739 RVA: 0x0007CD43 File Offset: 0x0007AF43
		private IEnumerator MatchEndCoroutine(int winningTeam)
		{
			this.runningMatchEndCoroutine = true;
			if (winningTeam >= 0 && winningTeam < this.teamParameters.Count && this.teamParameters[winningTeam].matchWonAudio != null)
			{
				this.audioSource.PlayOneShot(this.teamParameters[winningTeam].matchWonAudio);
			}
			yield return new WaitForSeconds(this.matchEndScoreResetDelayTime);
			this.runningMatchEndCoroutine = false;
			this.ResetScores();
			yield break;
		}

		// Token: 0x0600166C RID: 5740 RVA: 0x0007CD5C File Offset: 0x0007AF5C
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				for (int i = 0; i < this.teamScores.Count; i++)
				{
					stream.SendNext(this.teamScores[i]);
				}
				return;
			}
			for (int j = 0; j < this.teamScores.Count; j++)
			{
				this.teamScores[j] = (int)stream.ReceiveNext();
			}
			this.OnScoreUpdated();
		}

		// Token: 0x0400187C RID: 6268
		[SerializeField]
		private List<SportScoreboard.TeamParameters> teamParameters = new List<SportScoreboard.TeamParameters>();

		// Token: 0x0400187D RID: 6269
		[SerializeField]
		private int matchEndScore = 3;

		// Token: 0x0400187E RID: 6270
		[SerializeField]
		private float matchEndScoreResetDelayTime = 3f;

		// Token: 0x0400187F RID: 6271
		private List<int> teamScores = new List<int>();

		// Token: 0x04001880 RID: 6272
		private List<int> teamScoresPrev = new List<int>();

		// Token: 0x04001881 RID: 6273
		private bool runningMatchEndCoroutine;

		// Token: 0x04001882 RID: 6274
		private AudioSource audioSource;

		// Token: 0x0200050D RID: 1293
		[Serializable]
		private class TeamParameters
		{
			// Token: 0x04002127 RID: 8487
			[SerializeField]
			public MaterialUVOffsetListSetter teamScoreDisplayColumn0;

			// Token: 0x04002128 RID: 8488
			[SerializeField]
			public MaterialUVOffsetListSetter teamScoreDisplayColumn1;

			// Token: 0x04002129 RID: 8489
			[SerializeField]
			public AudioClip matchWonAudio;

			// Token: 0x0400212A RID: 8490
			[SerializeField]
			public AudioClip goalScoredAudio;
		}
	}
}
