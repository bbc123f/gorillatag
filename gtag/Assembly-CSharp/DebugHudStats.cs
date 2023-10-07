using System;
using System.Text;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000210 RID: 528
public class DebugHudStats : MonoBehaviour
{
	// Token: 0x06000D3F RID: 3391 RVA: 0x0004DB7E File Offset: 0x0004BD7E
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x0004DB8C File Offset: 0x0004BD8C
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

	// Token: 0x04001066 RID: 4198
	[SerializeField]
	private Text text;

	// Token: 0x04001067 RID: 4199
	[SerializeField]
	private float delayUpdateRate = 0.25f;

	// Token: 0x04001068 RID: 4200
	private float updateTimer;

	// Token: 0x04001069 RID: 4201
	private bool lastCheckInvalid;

	// Token: 0x0400106A RID: 4202
	public float sessionAnytrackingLost;

	// Token: 0x0400106B RID: 4203
	public float last30SecondsTrackingLost;

	// Token: 0x0400106C RID: 4204
	private float firstAwake;

	// Token: 0x0400106D RID: 4205
	private bool wasTrackingLost;

	// Token: 0x0400106E RID: 4206
	private bool leftHandTracked;

	// Token: 0x0400106F RID: 4207
	private bool rightHandTracked;

	// Token: 0x04001070 RID: 4208
	private StringBuilder builder;
}
