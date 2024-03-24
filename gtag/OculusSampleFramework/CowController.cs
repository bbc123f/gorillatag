using System;
using UnityEngine;

namespace OculusSampleFramework
{
	public class CowController : MonoBehaviour
	{
		private void Start()
		{
		}

		public void PlayMooSound()
		{
			this._mooCowAudioSource.timeSamples = 0;
			this._mooCowAudioSource.Play();
		}

		public void GoMooCowGo()
		{
			this._cowAnimation.Rewind();
			this._cowAnimation.Play();
		}

		public CowController()
		{
		}

		[SerializeField]
		private Animation _cowAnimation;

		[SerializeField]
		private AudioSource _mooCowAudioSource;
	}
}
