using System;
using Photon.Voice;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000328 RID: 808
	internal class ProcessVoiceDataToLoudness : IProcessor<float>, IDisposable
	{
		// Token: 0x0600167C RID: 5756 RVA: 0x0007D4B5 File Offset: 0x0007B6B5
		public ProcessVoiceDataToLoudness(VoiceToLoudness voiceToLoudness)
		{
			this._voiceToLoudness = voiceToLoudness;
		}

		// Token: 0x0600167D RID: 5757 RVA: 0x0007D4C4 File Offset: 0x0007B6C4
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

		// Token: 0x0600167E RID: 5758 RVA: 0x0007D502 File Offset: 0x0007B702
		public void Dispose()
		{
		}

		// Token: 0x04001892 RID: 6290
		private VoiceToLoudness _voiceToLoudness;
	}
}
