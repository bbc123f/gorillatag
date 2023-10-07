using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002E5 RID: 741
	public class CowController : MonoBehaviour
	{
		// Token: 0x0600140E RID: 5134 RVA: 0x00071AEA File Offset: 0x0006FCEA
		private void Start()
		{
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x00071AEC File Offset: 0x0006FCEC
		public void PlayMooSound()
		{
			this._mooCowAudioSource.timeSamples = 0;
			this._mooCowAudioSource.Play();
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x00071B05 File Offset: 0x0006FD05
		public void GoMooCowGo()
		{
			this._cowAnimation.Rewind();
			this._cowAnimation.Play();
		}

		// Token: 0x040016B3 RID: 5811
		[SerializeField]
		private Animation _cowAnimation;

		// Token: 0x040016B4 RID: 5812
		[SerializeField]
		private AudioSource _mooCowAudioSource;
	}
}
