using System;
using System.Collections;
using UnityEngine;

namespace UnityChan
{
	// Token: 0x0200035E RID: 862
	public class AutoBlink : MonoBehaviour
	{
		// Token: 0x0600191C RID: 6428 RVA: 0x0008A946 File Offset: 0x00088B46
		private void Awake()
		{
		}

		// Token: 0x0600191D RID: 6429 RVA: 0x0008A948 File Offset: 0x00088B48
		private void Start()
		{
			this.ResetTimer();
			base.StartCoroutine("RandomChange");
		}

		// Token: 0x0600191E RID: 6430 RVA: 0x0008A95C File Offset: 0x00088B5C
		private void ResetTimer()
		{
			this.timeRemining = this.timeBlink;
			this.timerStarted = false;
		}

		// Token: 0x0600191F RID: 6431 RVA: 0x0008A974 File Offset: 0x00088B74
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

		// Token: 0x06001920 RID: 6432 RVA: 0x0008A9E8 File Offset: 0x00088BE8
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

		// Token: 0x06001921 RID: 6433 RVA: 0x0008AA3A File Offset: 0x00088C3A
		private void SetCloseEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_Close);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_Close);
		}

		// Token: 0x06001922 RID: 6434 RVA: 0x0008AA60 File Offset: 0x00088C60
		private void SetHalfCloseEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_HalfClose);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_HalfClose);
		}

		// Token: 0x06001923 RID: 6435 RVA: 0x0008AA86 File Offset: 0x00088C86
		private void SetOpenEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_Open);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_Open);
		}

		// Token: 0x06001924 RID: 6436 RVA: 0x0008AAAC File Offset: 0x00088CAC
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

		// Token: 0x040019BE RID: 6590
		public bool isActive = true;

		// Token: 0x040019BF RID: 6591
		public SkinnedMeshRenderer ref_SMR_EYE_DEF;

		// Token: 0x040019C0 RID: 6592
		public SkinnedMeshRenderer ref_SMR_EL_DEF;

		// Token: 0x040019C1 RID: 6593
		public float ratio_Close = 85f;

		// Token: 0x040019C2 RID: 6594
		public float ratio_HalfClose = 20f;

		// Token: 0x040019C3 RID: 6595
		[HideInInspector]
		public float ratio_Open;

		// Token: 0x040019C4 RID: 6596
		private bool timerStarted;

		// Token: 0x040019C5 RID: 6597
		private bool isBlink;

		// Token: 0x040019C6 RID: 6598
		public float timeBlink = 0.4f;

		// Token: 0x040019C7 RID: 6599
		private float timeRemining;

		// Token: 0x040019C8 RID: 6600
		public float threshold = 0.3f;

		// Token: 0x040019C9 RID: 6601
		public float interval = 3f;

		// Token: 0x040019CA RID: 6602
		private AutoBlink.Status eyeStatus;

		// Token: 0x02000518 RID: 1304
		private enum Status
		{
			// Token: 0x04002146 RID: 8518
			Close,
			// Token: 0x04002147 RID: 8519
			HalfClose,
			// Token: 0x04002148 RID: 8520
			Open
		}
	}
}
