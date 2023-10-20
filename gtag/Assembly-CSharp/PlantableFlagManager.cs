using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000067 RID: 103
public class PlantableFlagManager : MonoBehaviourPun, IPunObservable
{
	// Token: 0x060001EB RID: 491 RVA: 0x0000DA38 File Offset: 0x0000BC38
	public void ResetMyFlags()
	{
		foreach (PlantableObject plantableObject in this.flags)
		{
			if (plantableObject.IsMyItem())
			{
				if (plantableObject.currentState != TransferrableObject.PositionState.Dropped)
				{
					plantableObject.DropItem();
				}
				plantableObject.ResetToHome();
			}
		}
	}

	// Token: 0x060001EC RID: 492 RVA: 0x0000DA80 File Offset: 0x0000BC80
	public void ResetAllFlags()
	{
		foreach (PlantableObject plantableObject in this.flags)
		{
			if (!plantableObject.IsMyItem())
			{
				plantableObject.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RequestOwnershipImmediately(delegate
				{
				});
			}
			if (plantableObject.currentState != TransferrableObject.PositionState.Dropped)
			{
				plantableObject.DropItem();
			}
			plantableObject.ResetToHome();
		}
	}

	// Token: 0x060001ED RID: 493 RVA: 0x0000DAF8 File Offset: 0x0000BCF8
	public void RainbowifyAllFlags(float saturation = 1f, float value = 1f)
	{
		Color red = Color.red;
		for (int i = 0; i < this.flags.Length; i++)
		{
			Color colorR = Color.HSVToRGB((float)i / (float)this.flags.Length, saturation, value);
			PlantableObject plantableObject = this.flags[i];
			if (plantableObject)
			{
				plantableObject.colorR = colorR;
				plantableObject.colorG = Color.black;
			}
		}
	}

	// Token: 0x060001EE RID: 494 RVA: 0x0000DB58 File Offset: 0x0000BD58
	public void Awake()
	{
		this.mode = new FlagCauldronColorer.ColorMode[this.flags.Length];
		this.flagColors = new PlantableObject.AppliedColors[this.flags.Length][];
		for (int i = 0; i < this.flags.Length; i++)
		{
			this.flagColors[i] = new PlantableObject.AppliedColors[20];
		}
	}

	// Token: 0x060001EF RID: 495 RVA: 0x0000DBB0 File Offset: 0x0000BDB0
	public void Update()
	{
		if (this.mode == null)
		{
			this.mode = new FlagCauldronColorer.ColorMode[this.flags.Length];
		}
		if (this.flagColors == null)
		{
			this.flagColors = new PlantableObject.AppliedColors[this.flags.Length][];
			for (int i = 0; i < this.flags.Length; i++)
			{
				this.flagColors[i] = new PlantableObject.AppliedColors[20];
			}
		}
		for (int j = 0; j < this.flags.Length; j++)
		{
			PlantableObject plantableObject = this.flags[j];
			if (plantableObject.IsMyItem() && Vector3.SqrMagnitude(plantableObject.flagTip.position - base.transform.position) <= 25f)
			{
				bool flag = false;
				FlagCauldronColorer[] array = this.cauldrons;
				int k = 0;
				while (k < array.Length)
				{
					FlagCauldronColorer flagCauldronColorer = array[k];
					if ((double)Vector3.SqrMagnitude(plantableObject.flagTip.position - flagCauldronColorer.colorPoint.position) < 0.05)
					{
						flag = true;
						if (this.mode[j] != flagCauldronColorer.mode)
						{
							this.mode[j] = flagCauldronColorer.mode;
							PlantableObject.AppliedColors appliedColors;
							switch (flagCauldronColorer.mode)
							{
							case FlagCauldronColorer.ColorMode.Red:
								appliedColors = PlantableObject.AppliedColors.Red;
								break;
							case FlagCauldronColorer.ColorMode.Green:
								appliedColors = PlantableObject.AppliedColors.Green;
								break;
							case FlagCauldronColorer.ColorMode.Blue:
								appliedColors = PlantableObject.AppliedColors.Blue;
								break;
							case FlagCauldronColorer.ColorMode.Black:
								appliedColors = PlantableObject.AppliedColors.Black;
								break;
							case FlagCauldronColorer.ColorMode.Clear:
								appliedColors = PlantableObject.AppliedColors.None;
								break;
							default:
								throw new ArgumentOutOfRangeException();
							}
							if (appliedColors == PlantableObject.AppliedColors.None)
							{
								plantableObject.ClearColors();
							}
							else
							{
								plantableObject.AddColor(appliedColors);
							}
							if (!PhotonNetwork.IsMasterClient)
							{
								base.photonView.RPC("UpdateFlagColorRPC", RpcTarget.MasterClient, new object[]
								{
									j,
									(int)appliedColors
								});
							}
							plantableObject.UpdateDisplayedDippedColor();
							this.flagColors[j] = plantableObject.dippedColors;
							break;
						}
						break;
					}
					else
					{
						k++;
					}
				}
				if (!flag)
				{
					this.mode[j] = FlagCauldronColorer.ColorMode.None;
				}
			}
		}
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x0000DD9C File Offset: 0x0000BF9C
	[PunRPC]
	public void UpdateFlagColorRPC(int flagIndex, int colorIndex, PhotonMessageInfo info)
	{
		PlantableObject plantableObject = this.flags[flagIndex];
		if (colorIndex == 0)
		{
			plantableObject.ClearColors();
			return;
		}
		plantableObject.AddColor((PlantableObject.AppliedColors)colorIndex);
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x0000DDC8 File Offset: 0x0000BFC8
	public void UpdateFlagColors()
	{
		for (int i = 0; i < this.flagColors.Length; i++)
		{
			PlantableObject.AppliedColors[] array = this.flagColors[i];
			PlantableObject plantableObject = this.flags[i];
			if (!plantableObject.IsMyItem() && array.Length <= 20)
			{
				plantableObject.dippedColors = array;
				plantableObject.UpdateDisplayedDippedColor();
			}
		}
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x0000DE18 File Offset: 0x0000C018
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			for (int i = 0; i < this.flagColors.Length; i++)
			{
				for (int j = 0; j < 20; j++)
				{
					stream.SendNext((int)this.flagColors[i][j]);
				}
			}
			return;
		}
		for (int k = 0; k < this.flagColors.Length; k++)
		{
			for (int l = 0; l < 20; l++)
			{
				this.flagColors[k][l] = (PlantableObject.AppliedColors)stream.ReceiveNext();
			}
		}
		this.UpdateFlagColors();
	}

	// Token: 0x040002BA RID: 698
	public PlantableObject[] flags;

	// Token: 0x040002BB RID: 699
	public FlagCauldronColorer[] cauldrons;

	// Token: 0x040002BC RID: 700
	public FlagCauldronColorer.ColorMode[] mode;

	// Token: 0x040002BD RID: 701
	public PlantableObject.AppliedColors[][] flagColors;
}
