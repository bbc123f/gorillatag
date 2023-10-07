using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02000321 RID: 801
	[RequireComponent(typeof(PhotonView))]
	[RequireComponent(typeof(AudioSource))]
	public class SportScoreboard : MonoBehaviourPunCallbacks, IPunObservable
	{
		// Token: 0x0600165D RID: 5725 RVA: 0x0007C5EC File Offset: 0x0007A7EC
		private void Awake()
		{
			this.audioSource = base.GetComponent<AudioSource>();
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				this.teamScores.Add(0);
				this.teamScoresPrev.Add(0);
			}
		}

		// Token: 0x0600165E RID: 5726 RVA: 0x0007C634 File Offset: 0x0007A834
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

		// Token: 0x0600165F RID: 5727 RVA: 0x0007C6D0 File Offset: 0x0007A8D0
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

		// Token: 0x06001660 RID: 5728 RVA: 0x0007C7BC File Offset: 0x0007A9BC
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

		// Token: 0x06001661 RID: 5729 RVA: 0x0007C810 File Offset: 0x0007AA10
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

		// Token: 0x06001662 RID: 5730 RVA: 0x0007C85B File Offset: 0x0007AA5B
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

		// Token: 0x06001663 RID: 5731 RVA: 0x0007C874 File Offset: 0x0007AA74
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

		// Token: 0x0400186F RID: 6255
		[SerializeField]
		private List<SportScoreboard.TeamParameters> teamParameters = new List<SportScoreboard.TeamParameters>();

		// Token: 0x04001870 RID: 6256
		[SerializeField]
		private int matchEndScore = 3;

		// Token: 0x04001871 RID: 6257
		[SerializeField]
		private float matchEndScoreResetDelayTime = 3f;

		// Token: 0x04001872 RID: 6258
		private List<int> teamScores = new List<int>();

		// Token: 0x04001873 RID: 6259
		private List<int> teamScoresPrev = new List<int>();

		// Token: 0x04001874 RID: 6260
		private bool runningMatchEndCoroutine;

		// Token: 0x04001875 RID: 6261
		private AudioSource audioSource;

		// Token: 0x0200050B RID: 1291
		[Serializable]
		private class TeamParameters
		{
			// Token: 0x0400211A RID: 8474
			[SerializeField]
			public MaterialUVOffsetListSetter teamScoreDisplayColumn0;

			// Token: 0x0400211B RID: 8475
			[SerializeField]
			public MaterialUVOffsetListSetter teamScoreDisplayColumn1;

			// Token: 0x0400211C RID: 8476
			[SerializeField]
			public AudioClip matchWonAudio;

			// Token: 0x0400211D RID: 8477
			[SerializeField]
			public AudioClip goalScoredAudio;
		}
	}
}
