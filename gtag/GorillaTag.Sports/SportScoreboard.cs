using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Sports;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(AudioSource))]
public class SportScoreboard : MonoBehaviourPunCallbacks, IPunObservable
{
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

	[SerializeField]
	private List<TeamParameters> teamParameters = new List<TeamParameters>();

	[SerializeField]
	private int matchEndScore = 3;

	[SerializeField]
	private float matchEndScoreResetDelayTime = 3f;

	private List<int> teamScores = new List<int>();

	private List<int> teamScoresPrev = new List<int>();

	private bool runningMatchEndCoroutine;

	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		for (int i = 0; i < teamParameters.Count; i++)
		{
			teamScores.Add(0);
			teamScoresPrev.Add(0);
		}
	}

	private void UpdateScoreboard()
	{
		for (int i = 0; i < teamParameters.Count; i++)
		{
			int num = teamScores[i];
			if ((bool)teamParameters[i].teamScoreDisplayColumn0)
			{
				teamParameters[i].teamScoreDisplayColumn0.SetUVOffset(num % 10);
			}
			if (teamParameters[i].teamScoreDisplayColumn1 != null)
			{
				teamParameters[i].teamScoreDisplayColumn1.SetUVOffset(num / 10 % 10);
			}
		}
	}

	private void OnScoreUpdated()
	{
		for (int i = 0; i < teamScores.Count; i++)
		{
			if (teamScores[i] > teamScoresPrev[i] && teamParameters[i].goalScoredAudio != null && teamScores[i] < matchEndScore)
			{
				audioSource.PlayOneShot(teamParameters[i].goalScoredAudio);
			}
			teamScoresPrev[i] = teamScores[i];
		}
		if (!runningMatchEndCoroutine)
		{
			for (int j = 0; j < teamScores.Count; j++)
			{
				if (teamScores[j] >= matchEndScore)
				{
					StartCoroutine(MatchEndCoroutine(j));
					break;
				}
			}
		}
		UpdateScoreboard();
	}

	public void TeamScored(int team)
	{
		if (base.photonView.IsMine && !runningMatchEndCoroutine)
		{
			if (team >= 0 && team < teamScores.Count)
			{
				teamScores[team] += 1;
			}
			OnScoreUpdated();
		}
	}

	public void ResetScores()
	{
		if (base.photonView.IsMine && !runningMatchEndCoroutine)
		{
			for (int i = 0; i < teamScores.Count; i++)
			{
				teamScores[i] = 0;
			}
			OnScoreUpdated();
		}
	}

	private IEnumerator MatchEndCoroutine(int winningTeam)
	{
		runningMatchEndCoroutine = true;
		if (winningTeam >= 0 && winningTeam < teamParameters.Count && teamParameters[winningTeam].matchWonAudio != null)
		{
			audioSource.PlayOneShot(teamParameters[winningTeam].matchWonAudio);
		}
		yield return new WaitForSeconds(matchEndScoreResetDelayTime);
		runningMatchEndCoroutine = false;
		ResetScores();
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			for (int i = 0; i < teamScores.Count; i++)
			{
				stream.SendNext(teamScores[i]);
			}
			return;
		}
		for (int j = 0; j < teamScores.Count; j++)
		{
			teamScores[j] = (int)stream.ReceiveNext();
		}
		OnScoreUpdated();
	}
}
