using System;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class SlingshotProjectileTrail : MonoBehaviour
{
	// Token: 0x0600056F RID: 1391 RVA: 0x00022A61 File Offset: 0x00020C61
	private void Awake()
	{
		this.initialWidthMultiplier = this.trailRenderer.widthMultiplier;
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x00022A74 File Offset: 0x00020C74
	public void AttachTrail(GameObject obj, bool blueTeam, bool redTeam)
	{
		this.followObject = obj;
		this.followXform = this.followObject.transform;
		Transform transform = base.transform;
		transform.position = this.followXform.position;
		this.initialScale = transform.localScale.x;
		transform.localScale = this.followXform.localScale;
		this.trailRenderer.widthMultiplier = this.initialWidthMultiplier * this.followXform.localScale.x;
		this.trailRenderer.Clear();
		if (blueTeam)
		{
			this.SetColor(this.blueColor);
		}
		else if (redTeam)
		{
			this.SetColor(this.orangeColor);
		}
		else
		{
			this.SetColor(this.defaultColor);
		}
		this.timeToDie = -1f;
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x00022B3C File Offset: 0x00020D3C
	protected void LateUpdate()
	{
		base.gameObject.transform.position = this.followXform.position;
		if (!this.followObject.activeSelf && this.timeToDie < 0f)
		{
			this.timeToDie = Time.time + this.trailRenderer.time;
		}
		if (this.timeToDie > 0f && Time.time > this.timeToDie)
		{
			base.transform.localScale = Vector3.one * this.initialScale;
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x00022BDC File Offset: 0x00020DDC
	public void SetColor(Color color)
	{
		TrailRenderer trailRenderer = this.trailRenderer;
		this.trailRenderer.endColor = color;
		trailRenderer.startColor = color;
	}

	// Token: 0x04000667 RID: 1639
	public TrailRenderer trailRenderer;

	// Token: 0x04000668 RID: 1640
	public Color defaultColor = Color.white;

	// Token: 0x04000669 RID: 1641
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

	// Token: 0x0400066A RID: 1642
	public Color blueColor = new Color(0f, 0.72f, 1f, 1f);

	// Token: 0x0400066B RID: 1643
	private GameObject followObject;

	// Token: 0x0400066C RID: 1644
	private Transform followXform;

	// Token: 0x0400066D RID: 1645
	private float timeToDie = -1f;

	// Token: 0x0400066E RID: 1646
	private float initialScale;

	// Token: 0x0400066F RID: 1647
	private float initialWidthMultiplier;
}
