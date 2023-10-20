using System;
using System.Collections;
using UnityEngine;

namespace UnityChan
{
	// Token: 0x02000360 RID: 864
	public class AutoBlink : MonoBehaviour
	{
		// Token: 0x06001925 RID: 6437 RVA: 0x0008AE2E File Offset: 0x0008902E
		private void Awake()
		{
		}

		// Token: 0x06001926 RID: 6438 RVA: 0x0008AE30 File Offset: 0x00089030
		private void Start()
		{
			this.ResetTimer();
			base.StartCoroutine("RandomChange");
		}

		// Token: 0x06001927 RID: 6439 RVA: 0x0008AE44 File Offset: 0x00089044
		private void ResetTimer()
		{
			this.timeRemining = this.timeBlink;
			this.timerStarted = false;
		}

		// Token: 0x06001928 RID: 6440 RVA: 0x0008AE5C File Offset: 0x0008905C
		private void Update()
		{
			if (!this.timerStarted)
			{
				this.eyeStatus = AutoBlink.Status.Close;
				this.timerStarted = true;
			}
			if (this.timerStarted)
			{
				this.timeRemining -= Time.deltaTime;
				if (this.timeRemining <= 0f)
				{
					this.eyeStatus = AutoBlink.Status.Open;
					this.ResetTimer();
					return;
				}
				if (this.timeRemining <= this.timeBlink * 0.3f)
				{
					this.eyeStatus = AutoBlink.Status.HalfClose;
				}
			}
		}

		// Token: 0x06001929 RID: 6441 RVA: 0x0008AED0 File Offset: 0x000890D0
		private void LateUpdate()
		{
			if (this.isActive && this.isBlink)
			{
				switch (this.eyeStatus)
				{
				case AutoBlink.Status.Close:
					this.SetCloseEyes();
					return;
				case AutoBlink.Status.HalfClose:
					this.SetHalfCloseEyes();
					return;
				case AutoBlink.Status.Open:
					this.SetOpenEyes();
					this.isBlink = false;
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x0600192A RID: 6442 RVA: 0x0008AF22 File Offset: 0x00089122
		private void SetCloseEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_Close);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_Close);
		}

		// Token: 0x0600192B RID: 6443 RVA: 0x0008AF48 File Offset: 0x00089148
		private void SetHalfCloseEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_HalfClose);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_HalfClose);
		}

		// Token: 0x0600192C RID: 6444 RVA: 0x0008AF6E File Offset: 0x0008916E
		private void SetOpenEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_Open);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_Open);
		}

		// Token: 0x0600192D RID: 6445 RVA: 0x0008AF94 File Offset: 0x00089194
		private IEnumerator RandomChange()
		{
			for (;;)
			{
				float num = Random.Range(0f, 1f);
				if (!this.isBlink && num > this.threshold)
				{
					this.isBlink = true;
				}
				yield return new WaitForSeconds(this.interval);
			}
			yield break;
		}

		// Token: 0x040019CB RID: 6603
		public bool isActive = true;

		// Token: 0x040019CC RID: 6604
		public SkinnedMeshRenderer ref_SMR_EYE_DEF;

		// Token: 0x040019CD RID: 6605
		public SkinnedMeshRenderer ref_SMR_EL_DEF;

		// Token: 0x040019CE RID: 6606
		public float ratio_Close = 85f;

		// Token: 0x040019CF RID: 6607
		public float ratio_HalfClose = 20f;

		// Token: 0x040019D0 RID: 6608
		[HideInInspector]
		public float ratio_Open;

		// Token: 0x040019D1 RID: 6609
		private bool timerStarted;

		// Token: 0x040019D2 RID: 6610
		private bool isBlink;

		// Token: 0x040019D3 RID: 6611
		public float timeBlink = 0.4f;

		// Token: 0x040019D4 RID: 6612
		private float timeRemining;

		// Token: 0x040019D5 RID: 6613
		public float threshold = 0.3f;

		// Token: 0x040019D6 RID: 6614
		public float interval = 3f;

		// Token: 0x040019D7 RID: 6615
		private AutoBlink.Status eyeStatus;

		// Token: 0x0200051A RID: 1306
		private enum Status
		{
			// Token: 0x04002153 RID: 8531
			Close,
			// Token: 0x04002154 RID: 8532
			HalfClose,
			// Token: 0x04002155 RID: 8533
			Open
		}
	}
}
