using Photon.Realtime;
using UnityEngine;

public class BattleBalloons : MonoBehaviour
{
	public VRRig myRig;

	public GameObject[] balloons;

	public Color orangeColor;

	public Color blueColor;

	public Color defaultColor;

	public Color lastColor;

	public GameObject balloonPopFXPrefab;

	public GorillaBattleManager bMgr;

	public Player myPlayer;

	private int colorShaderPropID;

	private MaterialPropertyBlock matPropBlock;

	private bool[] balloonsCachedActiveState;

	private Renderer[] renderers;

	private Color teamColor;

	protected void Awake()
	{
		matPropBlock = new MaterialPropertyBlock();
		renderers = new Renderer[balloons.Length];
		balloonsCachedActiveState = new bool[balloons.Length];
		for (int i = 0; i < balloons.Length; i++)
		{
			renderers[i] = balloons[i].GetComponentInChildren<Renderer>();
			balloonsCachedActiveState[i] = balloons[i].activeSelf;
		}
		colorShaderPropID = Shader.PropertyToID("_Color");
	}

	protected void OnEnable()
	{
		UpdateBalloonColors();
	}

	protected void LateUpdate()
	{
		if (GorillaGameManager.instance != null && (bMgr != null || GorillaGameManager.instance.gameObject.GetComponent<GorillaBattleManager>() != null))
		{
			if (bMgr == null)
			{
				bMgr = GorillaGameManager.instance.gameObject.GetComponent<GorillaBattleManager>();
			}
			int playerLives = bMgr.GetPlayerLives(myRig.creator);
			for (int i = 0; i < balloons.Length; i++)
			{
				bool flag = playerLives >= i + 1;
				if (flag != balloonsCachedActiveState[i])
				{
					balloonsCachedActiveState[i] = flag;
					balloons[i].SetActive(flag);
					if (!flag)
					{
						PopBalloon(i);
					}
				}
			}
		}
		else if (GorillaGameManager.instance != null)
		{
			base.gameObject.SetActive(value: false);
		}
		UpdateBalloonColors();
	}

	private void PopBalloon(int i)
	{
		GameObject obj = ObjectPools.instance.Instantiate(balloonPopFXPrefab);
		obj.transform.position = balloons[i].transform.position;
		GorillaColorizableBase componentInChildren = obj.GetComponentInChildren<GorillaColorizableBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(teamColor);
		}
	}

	public void UpdateBalloonColors()
	{
		if (bMgr != null && myRig.creator != null)
		{
			if (bMgr.OnRedTeam(myRig.creator))
			{
				teamColor = orangeColor;
			}
			else
			{
				teamColor = blueColor;
			}
		}
		if (teamColor != lastColor)
		{
			lastColor = teamColor;
			matPropBlock.SetColor(colorShaderPropID, teamColor);
			Renderer[] array = renderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetPropertyBlock(matPropBlock);
			}
		}
	}
}
