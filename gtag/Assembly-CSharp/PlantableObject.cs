using System;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000113 RID: 275
public class PlantableObject : TransferrableObject
{
	// Token: 0x060006F2 RID: 1778 RVA: 0x0002B934 File Offset: 0x00029B34
	protected override void Awake()
	{
		base.Awake();
		this.colorRShaderPropID = Shader.PropertyToID("_ColorR");
		this.colorGShaderPropID = Shader.PropertyToID("_ColorG");
		this.colorBShaderPropID = Shader.PropertyToID("_ColorB");
		this.materialPropertyBlock = new MaterialPropertyBlock();
		this.materialPropertyBlock.SetColor(this.colorRShaderPropID, this._colorR);
		this.flagRenderer.material = this.flagRenderer.sharedMaterial;
		this.flagRenderer.SetPropertyBlock(this.materialPropertyBlock);
		this.dippedColors = new PlantableObject.AppliedColors[20];
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x0002B9D0 File Offset: 0x00029BD0
	private void AssureShaderStuff()
	{
		if (!this.flagRenderer)
		{
			return;
		}
		if (this.colorRShaderPropID == 0)
		{
			this.colorRShaderPropID = Shader.PropertyToID("_ColorR");
			this.colorGShaderPropID = Shader.PropertyToID("_ColorG");
			this.colorBShaderPropID = Shader.PropertyToID("_ColorB");
		}
		if (this.materialPropertyBlock == null)
		{
			this.materialPropertyBlock = new MaterialPropertyBlock();
		}
		try
		{
			this.materialPropertyBlock.SetColor(this.colorRShaderPropID, this._colorR);
			this.materialPropertyBlock.SetColor(this.colorGShaderPropID, this._colorG);
		}
		catch
		{
			this.materialPropertyBlock = new MaterialPropertyBlock();
			this.materialPropertyBlock.SetColor(this.colorRShaderPropID, this._colorR);
			this.materialPropertyBlock.SetColor(this.colorGShaderPropID, this._colorG);
		}
		this.flagRenderer.material = this.flagRenderer.sharedMaterial;
		this.flagRenderer.SetPropertyBlock(this.materialPropertyBlock);
	}

	// Token: 0x1700004D RID: 77
	// (get) Token: 0x060006F4 RID: 1780 RVA: 0x0002BADC File Offset: 0x00029CDC
	// (set) Token: 0x060006F5 RID: 1781 RVA: 0x0002BAE4 File Offset: 0x00029CE4
	public Color colorR
	{
		get
		{
			return this._colorR;
		}
		set
		{
			this._colorR = value;
			this.AssureShaderStuff();
		}
	}

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x060006F6 RID: 1782 RVA: 0x0002BAF3 File Offset: 0x00029CF3
	// (set) Token: 0x060006F7 RID: 1783 RVA: 0x0002BAFB File Offset: 0x00029CFB
	public Color colorG
	{
		get
		{
			return this._colorG;
		}
		set
		{
			this._colorG = value;
			this.AssureShaderStuff();
		}
	}

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x060006F9 RID: 1785 RVA: 0x0002BB38 File Offset: 0x00029D38
	// (set) Token: 0x060006F8 RID: 1784 RVA: 0x0002BB0A File Offset: 0x00029D0A
	[DevInspectorShow]
	[DevInspectorCyan]
	public bool planted
	{
		get
		{
			return this._planted;
		}
		set
		{
			if (value != this._planted)
			{
				if (value && !this.rigidbodyInstance.isKinematic)
				{
					this.rigidbodyInstance.isKinematic = true;
				}
				this._planted = value;
			}
		}
	}

	// Token: 0x060006FA RID: 1786 RVA: 0x0002BB40 File Offset: 0x00029D40
	private void AddRed()
	{
		this.AddColor(PlantableObject.AppliedColors.Red);
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x0002BB49 File Offset: 0x00029D49
	private void AddGreen()
	{
		this.AddColor(PlantableObject.AppliedColors.Blue);
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x0002BB52 File Offset: 0x00029D52
	private void AddBlue()
	{
		this.AddColor(PlantableObject.AppliedColors.Green);
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x0002BB5B File Offset: 0x00029D5B
	private void AddBlack()
	{
		this.AddColor(PlantableObject.AppliedColors.Black);
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x0002BB64 File Offset: 0x00029D64
	public void AddColor(PlantableObject.AppliedColors color)
	{
		this.dippedColors[this.currentDipIndex] = color;
		this.currentDipIndex++;
		if (this.currentDipIndex >= this.dippedColors.Length)
		{
			this.currentDipIndex = 0;
		}
		this.UpdateDisplayedDippedColor();
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x0002BBA0 File Offset: 0x00029DA0
	public void ClearColors()
	{
		for (int i = 0; i < this.dippedColors.Length; i++)
		{
			this.dippedColors[i] = PlantableObject.AppliedColors.None;
		}
		this.currentDipIndex = 0;
		this.UpdateDisplayedDippedColor();
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x0002BBD8 File Offset: 0x00029DD8
	public Color CalculateOutputColor()
	{
		Color color = Color.black;
		int num = 0;
		int num2 = 0;
		foreach (PlantableObject.AppliedColors appliedColors in this.dippedColors)
		{
			if (appliedColors == PlantableObject.AppliedColors.None)
			{
				break;
			}
			switch (appliedColors)
			{
			case PlantableObject.AppliedColors.Red:
				color += Color.red;
				num2++;
				break;
			case PlantableObject.AppliedColors.Green:
				color += Color.green;
				num2++;
				break;
			case PlantableObject.AppliedColors.Blue:
				color += Color.blue;
				num2++;
				break;
			case PlantableObject.AppliedColors.Black:
				num++;
				num2++;
				break;
			}
		}
		if (color == Color.black && num == 0)
		{
			return Color.white;
		}
		float num3 = Mathf.Max(new float[]
		{
			color.r,
			color.g,
			color.b
		});
		if (num3 == 0f)
		{
			return Color.black;
		}
		color /= num3;
		float num4 = (float)num / (float)num2;
		if (num4 > 0f)
		{
			color *= 1f - num4;
		}
		return color;
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x0002BCE1 File Offset: 0x00029EE1
	public void UpdateDisplayedDippedColor()
	{
		this.colorR = this.CalculateOutputColor();
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x0002BCEF File Offset: 0x00029EEF
	public override void DropItem()
	{
		base.DropItem();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x0002BD19 File Offset: 0x00029F19
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		this.itemState = (this.planted ? TransferrableObject.ItemStates.State1 : TransferrableObject.ItemStates.State0);
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x0002BD33 File Offset: 0x00029F33
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x0002BD5D File Offset: 0x00029F5D
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x0002BD67 File Offset: 0x00029F67
	public override bool ShouldBeKinematic()
	{
		return base.ShouldBeKinematic() || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x0002BD80 File Offset: 0x00029F80
	public override void OnOwnershipTransferred(Player toPlayer, Player fromPlayer)
	{
		base.OnOwnershipTransferred(toPlayer, fromPlayer);
		if (toPlayer == null)
		{
			return;
		}
		Action<Color> <>9__1;
		GorillaGameManager.OnInstanceReady(delegate
		{
			VRRig vrrig = GorillaGameManager.instance.FindPlayerVRRig(toPlayer);
			if (vrrig == null)
			{
				return;
			}
			VRRig vrrig2 = vrrig;
			Action<Color> action;
			if ((action = <>9__1) == null)
			{
				action = (<>9__1 = delegate(Color color1)
				{
					this.colorG = color1;
				});
			}
			vrrig2.OnColorInitialized(action);
		});
	}

	// Token: 0x04000877 RID: 2167
	public PlantablePoint point;

	// Token: 0x04000878 RID: 2168
	public SkinnedMeshRenderer flagRenderer;

	// Token: 0x04000879 RID: 2169
	[FormerlySerializedAs("colorShaderPropID")]
	[SerializeReference]
	private int colorRShaderPropID;

	// Token: 0x0400087A RID: 2170
	[SerializeReference]
	private int colorGShaderPropID;

	// Token: 0x0400087B RID: 2171
	[SerializeReference]
	private int colorBShaderPropID;

	// Token: 0x0400087C RID: 2172
	private MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x0400087D RID: 2173
	[HideInInspector]
	[SerializeReference]
	private Color _colorR;

	// Token: 0x0400087E RID: 2174
	[HideInInspector]
	[SerializeReference]
	private Color _colorG;

	// Token: 0x0400087F RID: 2175
	private bool _planted;

	// Token: 0x04000880 RID: 2176
	public Transform flagTip;

	// Token: 0x04000881 RID: 2177
	public PlantableObject.AppliedColors[] dippedColors = new PlantableObject.AppliedColors[20];

	// Token: 0x04000882 RID: 2178
	public int currentDipIndex;

	// Token: 0x020003FF RID: 1023
	public enum AppliedColors
	{
		// Token: 0x04001CA0 RID: 7328
		None,
		// Token: 0x04001CA1 RID: 7329
		Red,
		// Token: 0x04001CA2 RID: 7330
		Green,
		// Token: 0x04001CA3 RID: 7331
		Blue,
		// Token: 0x04001CA4 RID: 7332
		Black
	}
}
