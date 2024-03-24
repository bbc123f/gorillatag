using System;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

public class PlantableFlagManager : MonoBehaviourPun, IPunObservable
{
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

	public void RainbowifyAllFlags(float saturation = 1f, float value = 1f)
	{
		Color red = Color.red;
		for (int i = 0; i < this.flags.Length; i++)
		{
			Color color = Color.HSVToRGB((float)i / (float)this.flags.Length, saturation, value);
			PlantableObject plantableObject = this.flags[i];
			if (plantableObject)
			{
				plantableObject.colorR = color;
				plantableObject.colorG = Color.black;
			}
		}
	}

	public void Awake()
	{
		this.mode = new FlagCauldronColorer.ColorMode[this.flags.Length];
		this.flagColors = new PlantableObject.AppliedColors[this.flags.Length][];
		for (int i = 0; i < this.flags.Length; i++)
		{
			this.flagColors[i] = new PlantableObject.AppliedColors[20];
		}
	}

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

	public PlantableFlagManager()
	{
	}

	public PlantableObject[] flags;

	public FlagCauldronColorer[] cauldrons;

	public FlagCauldronColorer.ColorMode[] mode;

	public PlantableObject.AppliedColors[][] flagColors;

	[CompilerGenerated]
	[Serializable]
	private sealed class <>c
	{
		// Note: this type is marked as 'beforefieldinit'.
		static <>c()
		{
		}

		public <>c()
		{
		}

		internal void <ResetAllFlags>b__2_0()
		{
		}

		public static readonly PlantableFlagManager.<>c <>9 = new PlantableFlagManager.<>c();

		public static Action <>9__2_0;
	}
}
