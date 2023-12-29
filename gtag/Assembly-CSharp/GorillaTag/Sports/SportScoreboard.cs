using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Sports
{
	[RequireComponent(typeof(PhotonView))]
	[RequireComponent(typeof(AudioSource))]
	public class SportScoreboard : MonoBehaviourPunCallbacks, IPunObservable
	{
		private void Awake()
		{
			this.audioSource = base.GetComponent<AudioSource>();
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				this.teamScores.Add(0);
				this.teamScoresPrev.Add(0);
			}
		}

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

		[SerializeField]
		private List<SportScoreboard.TeamParameters> teamParameters = new List<SportScoreboard.TeamParameters>();

		[SerializeField]
		private int matchEndScore = 3;

		[SerializeField]
		private float matchEndScoreResetDelayTime = 3f;

		private List<int> teamScores = new List<int>();

		private List<int> teamScoresPrev = new List<int>();

		private bool runningMatchEndCoroutine;

		private AudioSource audioSource;

		[Serializable]
		private class TeamParameters
		{
			[SerializeField]
			public MaterialUVOffsetListSetter teamScoreDisplayColumn0;

			[SerializeField]
			public MaterialUVOffsetListSetter teamScoreDisplayColumn1;

			[SerializeField]
			public AudioClip matchWonAudio;

			[SerializeField]
			public AudioClip goalScoredAudio;
		}
	}
}
