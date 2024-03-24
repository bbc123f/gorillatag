using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
			SportScoreboard.Instance = this;
			this.audioSource = base.GetComponent<AudioSource>();
			this.scoreVisuals = new SportScoreboardVisuals[this.teamParameters.Count];
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				this.teamScores.Add(0);
				this.teamScoresPrev.Add(0);
			}
		}

		public void RegisterTeamVisual(int TeamIndex, SportScoreboardVisuals visuals)
		{
			this.scoreVisuals[TeamIndex] = visuals;
			this.UpdateScoreboard();
		}

		private void UpdateScoreboard()
		{
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				if (!(this.scoreVisuals[i] == null))
				{
					int num = this.teamScores[i];
					if (this.scoreVisuals[i].score1s != null)
					{
						this.scoreVisuals[i].score1s.SetUVOffset(num % 10);
					}
					if (this.scoreVisuals[i].score10s != null)
					{
						this.scoreVisuals[i].score10s.SetUVOffset(num / 10 % 10);
					}
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

		public SportScoreboard()
		{
		}

		[OnEnterPlay_SetNull]
		public static SportScoreboard Instance;

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

		private SportScoreboardVisuals[] scoreVisuals;

		[Serializable]
		private class TeamParameters
		{
			public TeamParameters()
			{
			}

			[SerializeField]
			public AudioClip matchWonAudio;

			[SerializeField]
			public AudioClip goalScoredAudio;
		}

		[CompilerGenerated]
		private sealed class <MatchEndCoroutine>d__16 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <MatchEndCoroutine>d__16(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				SportScoreboard sportScoreboard = this;
				if (num == 0)
				{
					this.<>1__state = -1;
					sportScoreboard.runningMatchEndCoroutine = true;
					if (winningTeam >= 0 && winningTeam < sportScoreboard.teamParameters.Count && sportScoreboard.teamParameters[winningTeam].matchWonAudio != null)
					{
						sportScoreboard.audioSource.PlayOneShot(sportScoreboard.teamParameters[winningTeam].matchWonAudio);
					}
					this.<>2__current = new WaitForSeconds(sportScoreboard.matchEndScoreResetDelayTime);
					this.<>1__state = 1;
					return true;
				}
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				sportScoreboard.runningMatchEndCoroutine = false;
				sportScoreboard.ResetScores();
				return false;
			}

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			private int <>1__state;

			private object <>2__current;

			public SportScoreboard <>4__this;

			public int winningTeam;
		}
	}
}
