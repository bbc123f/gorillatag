using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000155 RID: 341
public class ChestHeartbeat : MonoBehaviour
{
	// Token: 0x0600087E RID: 2174 RVA: 0x00034878 File Offset: 0x00032A78
	public void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			if ((PhotonNetwork.ServerTimestamp > this.lastShot + this.millisMin || Mathf.Abs(PhotonNetwork.ServerTimestamp - this.lastShot) > 10000) && PhotonNetwork.ServerTimestamp % 1500 <= 10)
			{
				this.lastShot = PhotonNetwork.ServerTimestamp;
				this.audioSource.PlayOneShot(this.audioSource.clip);
				base.StartCoroutine(this.HeartBeat());
				return;
			}
		}
		else if ((Time.time * 1000f > (float)(this.lastShot + this.millisMin) || Mathf.Abs(Time.time * 1000f - (float)this.lastShot) > 10000f) && Time.time * 1000f % 1500f <= 10f)
		{
			this.lastShot = PhotonNetwork.ServerTimestamp;
			this.audioSource.PlayOneShot(this.audioSource.clip);
			base.StartCoroutine(this.HeartBeat());
		}
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x0003497C File Offset: 0x00032B7C
	private IEnumerator HeartBeat()
	{
		float startTime = Time.time;
		while (Time.time < startTime + this.endtime)
		{
			if (Time.time < startTime + this.minTime)
			{
				this.deltaTime = Time.time - startTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * this.heartMinSize, this.deltaTime / this.minTime);
			}
			else if (Time.time < startTime + this.maxTime)
			{
				this.deltaTime = Time.time - startTime - this.minTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one * this.heartMinSize, Vector3.one * this.heartMaxSize, this.deltaTime / (this.maxTime - this.minTime));
			}
			else if (Time.time < startTime + this.endtime)
			{
				this.deltaTime = Time.time - startTime - this.maxTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one * this.heartMaxSize, Vector3.one, this.deltaTime / (this.endtime - this.maxTime));
			}
			yield return new WaitForFixedUpdate();
		}
		yield break;
	}

	// Token: 0x04000AA3 RID: 2723
	public int millisToWait;

	// Token: 0x04000AA4 RID: 2724
	public int millisMin = 300;

	// Token: 0x04000AA5 RID: 2725
	public int lastShot;

	// Token: 0x04000AA6 RID: 2726
	public AudioSource audioSource;

	// Token: 0x04000AA7 RID: 2727
	public Transform scaleTransform;

	// Token: 0x04000AA8 RID: 2728
	private float deltaTime;

	// Token: 0x04000AA9 RID: 2729
	private float heartMinSize = 0.9f;

	// Token: 0x04000AAA RID: 2730
	private float heartMaxSize = 1.2f;

	// Token: 0x04000AAB RID: 2731
	private float minTime = 0.05f;

	// Token: 0x04000AAC RID: 2732
	private float maxTime = 0.1f;

	// Token: 0x04000AAD RID: 2733
	private float endtime = 0.25f;

	// Token: 0x04000AAE RID: 2734
	private float currentTime;
}
