using System;
using System.Text;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000211 RID: 529
public class DebugHudStats : MonoBehaviour
{
	// Token: 0x06000D45 RID: 3397 RVA: 0x0004DDDE File Offset: 0x0004BFDE
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x0004DDEC File Offset: 0x0004BFEC
	private void Update()
	{
		if (this.firstAwake == 0f)
		{
			this.firstAwake = Time.time;
		}
		if (this.updateTimer < this.delayUpdateRate)
		{
			this.updateTimer += Time.deltaTime;
			return;
		}
		this.builder.Clear();
		int num = Mathf.RoundToInt(1f / Time.smoothDeltaTime);
		num = Mathf.Min(num, 90);
		if (num < 89)
		{
			if (this.lastCheckInvalid)
			{
				this.text.color = Color.red;
			}
			this.lastCheckInvalid = true;
		}
		else
		{
			this.lastCheckInvalid = false;
			this.text.color = Color.white;
		}
		this.builder.Append(num);
		this.builder.AppendLine(" fps");
		float magnitude = Player.Instance.currentVelocity.magnitude;
		this.builder.Append(Mathf.RoundToInt(magnitude));
		this.builder.AppendLine(" m/s");
		this.text.text = this.builder.ToString();
		this.updateTimer = 0f;
	}

	// Token: 0x0400106B RID: 4203
	[SerializeField]
	private Text text;

	// Token: 0x0400106C RID: 4204
	[SerializeField]
	private float delayUpdateRate = 0.25f;

	// Token: 0x0400106D RID: 4205
	private float updateTimer;

	// Token: 0x0400106E RID: 4206
	private bool lastCheckInvalid;

	// Token: 0x0400106F RID: 4207
	public float sessionAnytrackingLost;

	// Token: 0x04001070 RID: 4208
	public float last30SecondsTrackingLost;

	// Token: 0x04001071 RID: 4209
	private float firstAwake;

	// Token: 0x04001072 RID: 4210
	private bool wasTrackingLost;

	// Token: 0x04001073 RID: 4211
	private bool leftHandTracked;

	// Token: 0x04001074 RID: 4212
	private bool rightHandTracked;

	// Token: 0x04001075 RID: 4213
	private StringBuilder builder;
}
