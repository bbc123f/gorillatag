using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

public class PlantableObject : TransferrableObject
{
	public enum AppliedColors
	{
		None,
		Red,
		Green,
		Blue,
		Black
	}

	public PlantablePoint point;

	public SkinnedMeshRenderer flagRenderer;

	[FormerlySerializedAs("colorShaderPropID")]
	[SerializeReference]
	private int colorRShaderPropID;

	[SerializeReference]
	private int colorGShaderPropID;

	[SerializeReference]
	private int colorBShaderPropID;

	private MaterialPropertyBlock materialPropertyBlock;

	[HideInInspector]
	[SerializeReference]
	private Color _colorR;

	[HideInInspector]
	[SerializeReference]
	private Color _colorG;

	private bool _planted;

	public Transform flagTip;

	public AppliedColors[] dippedColors = new AppliedColors[20];

	public int currentDipIndex;

	public Color colorR
	{
		get
		{
			return _colorR;
		}
		set
		{
			_colorR = value;
			AssureShaderStuff();
		}
	}

	public Color colorG
	{
		get
		{
			return _colorG;
		}
		set
		{
			_colorG = value;
			AssureShaderStuff();
		}
	}

	[DevInspectorShow]
	[DevInspectorCyan]
	public bool planted
	{
		get
		{
			return _planted;
		}
		set
		{
			if (value != _planted)
			{
				if (value && !rigidbodyInstance.isKinematic)
				{
					rigidbodyInstance.isKinematic = true;
				}
				_planted = value;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		colorRShaderPropID = Shader.PropertyToID("_ColorR");
		colorGShaderPropID = Shader.PropertyToID("_ColorG");
		colorBShaderPropID = Shader.PropertyToID("_ColorB");
		materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetColor(colorRShaderPropID, _colorR);
		flagRenderer.material = flagRenderer.sharedMaterial;
		flagRenderer.SetPropertyBlock(materialPropertyBlock);
		dippedColors = new AppliedColors[20];
	}

	private void AssureShaderStuff()
	{
		if ((bool)flagRenderer)
		{
			if (colorRShaderPropID == 0)
			{
				colorRShaderPropID = Shader.PropertyToID("_ColorR");
				colorGShaderPropID = Shader.PropertyToID("_ColorG");
				colorBShaderPropID = Shader.PropertyToID("_ColorB");
			}
			if (materialPropertyBlock == null)
			{
				materialPropertyBlock = new MaterialPropertyBlock();
			}
			try
			{
				materialPropertyBlock.SetColor(colorRShaderPropID, _colorR);
				materialPropertyBlock.SetColor(colorGShaderPropID, _colorG);
			}
			catch
			{
				materialPropertyBlock = new MaterialPropertyBlock();
				materialPropertyBlock.SetColor(colorRShaderPropID, _colorR);
				materialPropertyBlock.SetColor(colorGShaderPropID, _colorG);
			}
			flagRenderer.material = flagRenderer.sharedMaterial;
			flagRenderer.SetPropertyBlock(materialPropertyBlock);
		}
	}

	private void AddRed()
	{
		AddColor(AppliedColors.Red);
	}

	private void AddGreen()
	{
		AddColor(AppliedColors.Blue);
	}

	private void AddBlue()
	{
		AddColor(AppliedColors.Green);
	}

	private void AddBlack()
	{
		AddColor(AppliedColors.Black);
	}

	public void AddColor(AppliedColors color)
	{
		dippedColors[currentDipIndex] = color;
		currentDipIndex++;
		if (currentDipIndex >= dippedColors.Length)
		{
			currentDipIndex = 0;
		}
		UpdateDisplayedDippedColor();
	}

	public void ClearColors()
	{
		for (int i = 0; i < dippedColors.Length; i++)
		{
			dippedColors[i] = AppliedColors.None;
		}
		currentDipIndex = 0;
		UpdateDisplayedDippedColor();
	}

	public Color CalculateOutputColor()
	{
		Color black = Color.black;
		int num = 0;
		int num2 = 0;
		AppliedColors[] array = dippedColors;
		for (int i = 0; i < array.Length; i++)
		{
			switch (array[i])
			{
			case AppliedColors.Red:
				black += Color.red;
				num2++;
				continue;
			case AppliedColors.Green:
				black += Color.green;
				num2++;
				continue;
			case AppliedColors.Blue:
				black += Color.blue;
				num2++;
				continue;
			case AppliedColors.Black:
				num++;
				num2++;
				continue;
			default:
				continue;
			case AppliedColors.None:
				break;
			}
			break;
		}
		if (black == Color.black && num == 0)
		{
			return Color.white;
		}
		float num3 = Mathf.Max(black.r, black.g, black.b);
		if (num3 == 0f)
		{
			return Color.black;
		}
		black /= num3;
		float num4 = (float)num / (float)num2;
		if (num4 > 0f)
		{
			black *= 1f - num4;
		}
		return black;
	}

	public void UpdateDisplayedDippedColor()
	{
		colorR = CalculateOutputColor();
	}

	public override void DropItem()
	{
		base.DropItem();
		if (itemState == ItemStates.State1 && !rigidbodyInstance.isKinematic)
		{
			rigidbodyInstance.isKinematic = true;
		}
	}

	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		itemState = ((!planted) ? ItemStates.State0 : ItemStates.State1);
	}

	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (itemState == ItemStates.State1 && !rigidbodyInstance.isKinematic)
		{
			rigidbodyInstance.isKinematic = true;
		}
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
	}

	public override bool ShouldBeKinematic()
	{
		if (base.ShouldBeKinematic())
		{
			return true;
		}
		if (itemState == ItemStates.State1)
		{
			return true;
		}
		return false;
	}

	public override void OnOwnershipTransferred(Player toPlayer, Player fromPlayer)
	{
		base.OnOwnershipTransferred(toPlayer, fromPlayer);
		if (toPlayer == null)
		{
			return;
		}
		GorillaGameManager.OnInstanceReady(delegate
		{
			VRRig vRRig = GorillaGameManager.instance.FindPlayerVRRig(toPlayer);
			if (!(vRRig == null))
			{
				vRRig.OnColorInitialized(delegate(Color color1)
				{
					colorG = color1;
				});
			}
		});
	}
}
