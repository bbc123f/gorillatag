using System;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020000DA RID: 218
public class BattleBalloons : MonoBehaviour
{
	// Token: 0x060004F1 RID: 1265 RVA: 0x0001F6E8 File Offset: 0x0001D8E8
	protected void Awake()
	{
		this.matPropBlock = new MaterialPropertyBlock();
		this.renderers = new Renderer[this.balloons.Length];
		this.balloonsCachedActiveState = new bool[this.balloons.Length];
		for (int i = 0; i < this.balloons.Length; i++)
		{
			this.renderers[i] = this.balloons[i].GetComponentInChildren<Renderer>();
			this.balloonsCachedActiveState[i] = this.balloons[i].activeSelf;
		}
		this.colorShaderPropID = Shader.PropertyToID("_Color");
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x0001F773 File Offset: 0x0001D973
	protected void OnEnable()
	{
		this.UpdateBalloonColors();
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x0001F77C File Offset: 0x0001D97C
	protected void LateUpdate()
	{
		if (GorillaGameManager.instance != null && (this.bMgr != null || GorillaGameManager.instance.gameObject.GetComponent<GorillaBattleManager>() != null))
		{
			if (this.bMgr == null)
			{
				this.bMgr = GorillaGameManager.instance.gameObject.GetComponent<GorillaBattleManager>();
			}
			int playerLives = this.bMgr.GetPlayerLives(this.myRig.creator);
			for (int i = 0; i < this.balloons.Length; i++)
			{
				bool flag = playerLives >= i + 1;
				if (flag != this.balloonsCachedActiveState[i])
				{
					this.balloonsCachedActiveState[i] = flag;
					this.balloons[i].SetActive(flag);
					if (!flag)
					{
						this.PopBalloon(i);
					}
				}
			}
		}
		else if (GorillaGameManager.instance != null)
		{
			base.gameObject.SetActive(false);
		}
		this.UpdateBalloonColors();
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x0001F870 File Offset: 0x0001DA70
	private void PopBalloon(int i)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(this.balloonPopFXPrefab);
		gameObject.transform.position = this.balloons[i].transform.position;
		GorillaColorizableBase componentInChildren = gameObject.GetComponentInChildren<GorillaColorizableBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(this.teamColor);
		}
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x0001F8C8 File Offset: 0x0001DAC8
	public void UpdateBalloonColors()
	{
		if (this.bMgr != null && this.myRig.creator != null)
		{
			if (this.bMgr.OnRedTeam(this.myRig.creator))
			{
				this.teamColor = this.orangeColor;
			}
			else
			{
				this.teamColor = this.blueColor;
			}
		}
		if (this.teamColor != this.lastColor)
		{
			this.lastColor = this.teamColor;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer)
				{
					foreach (Material material in renderer.materials)
					{
						if (!(material == null))
						{
							if (material.HasProperty("_BaseColor"))
							{
								material.SetColor("_BaseColor", this.teamColor);
							}
							if (material.HasProperty("_Color"))
							{
								material.SetColor("_Color", this.teamColor);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x040005B6 RID: 1462
	public VRRig myRig;

	// Token: 0x040005B7 RID: 1463
	public GameObject[] balloons;

	// Token: 0x040005B8 RID: 1464
	public Color orangeColor;

	// Token: 0x040005B9 RID: 1465
	public Color blueColor;

	// Token: 0x040005BA RID: 1466
	public Color defaultColor;

	// Token: 0x040005BB RID: 1467
	public Color lastColor;

	// Token: 0x040005BC RID: 1468
	public GameObject balloonPopFXPrefab;

	// Token: 0x040005BD RID: 1469
	public GorillaBattleManager bMgr;

	// Token: 0x040005BE RID: 1470
	public Player myPlayer;

	// Token: 0x040005BF RID: 1471
	private int colorShaderPropID;

	// Token: 0x040005C0 RID: 1472
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x040005C1 RID: 1473
	private bool[] balloonsCachedActiveState;

	// Token: 0x040005C2 RID: 1474
	private Renderer[] renderers;

	// Token: 0x040005C3 RID: 1475
	private Color teamColor;
}
