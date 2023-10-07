using System;
using Photon.Voice;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000326 RID: 806
	internal class ProcessVoiceDataToLoudness : IProcessor<float>, IDisposable
	{
		// Token: 0x06001673 RID: 5747 RVA: 0x0007CFCD File Offset: 0x0007B1CD
		public ProcessVoiceDataToLoudness(VoiceToLoudness voiceToLoudness)
		{
			this._voiceToLoudness = voiceToLoudness;
		}

		// Token: 0x06001674 RID: 5748 RVA: 0x0007CFDC File Offset: 0x0007B1DC
		public float[] Process(float[] buf)
		{
			float num = 0f;
			for (int i = 0; i < buf.Length; i++)
			{
				num += Mathf.Abs(buf[i]);
			}
			this._voiceToLoudness.loudness = num / (float)buf.Length;
			return buf;
		}

		// Token: 0x06001675 RID: 5749 RVA: 0x0007D01A File Offset: 0x0007B21A
		public void Dispose()
		{
		}

		// Token: 0x04001885 RID: 6277
		private VoiceToLoudness _voiceToLoudness;
	}
}
