using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200000D RID: 13
public class CollectibleCoin : MonoBehaviour
{
	// Token: 0x06000029 RID: 41 RVA: 0x00002DEC File Offset: 0x00000FEC
	public void Update()
	{
		BoingBehavior component = base.GetComponent<BoingBehavior>();
		if (this.m_taken)
		{
			if (Time.time - this.m_respawnTimerStartTime < this.RespawnTime)
			{
				return;
			}
			base.transform.position = this.m_respawnPosition + 0.4f * Vector3.down;
			if (component != null)
			{
				component.Reboot();
			}
			base.transform.position = this.m_respawnPosition;
			this.m_taken = false;
		}
		GameObject gameObject = GameObject.Find("Character");
		GameObject gameObject2 = GameObject.Find("Coin Icon");
		GameObject gameObject3 = GameObject.Find("Coin Counter");
		if ((gameObject.transform.position - base.transform.position).sqrMagnitude > 0.4f)
		{
			return;
		}
		this.m_respawnPosition = base.transform.position;
		if (component != null)
		{
			Vector3Spring positionSpring = component.PositionSpring;
			positionSpring.Reset(base.transform.position, new Vector3(100f, 0f, 0f));
			component.PositionSpring = positionSpring;
		}
		base.transform.position = gameObject2.transform.position + new Vector3(-2f, 0.5f, 0f);
		TextMesh component2 = gameObject3.GetComponent<TextMesh>();
		component2.text = (Convert.ToInt32(component2.text) + 1).ToString();
		this.m_respawnTimerStartTime = Time.time;
		this.m_taken = true;
	}

	// Token: 0x0400002A RID: 42
	public float RespawnTime;

	// Token: 0x0400002B RID: 43
	private bool m_taken;

	// Token: 0x0400002C RID: 44
	private Vector3 m_respawnPosition;

	// Token: 0x0400002D RID: 45
	private float m_respawnTimerStartTime;
}
