using System;
using Photon.Pun;
using UnityEngine;

public class PlantableFlagManager : MonoBehaviourPun, IPunObservable
{
	public PlantableObject[] flags;

	public FlagCauldronColorer[] cauldrons;

	public FlagCauldronColorer.ColorMode[] mode;

	public PlantableObject.AppliedColors[][] flagColors;

	public void ResetMyFlags()
	{
		PlantableObject[] array = flags;
		foreach (PlantableObject plantableObject in array)
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
		PlantableObject[] array = flags;
		foreach (PlantableObject plantableObject in array)
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
		_ = Color.red;
		for (int i = 0; i < flags.Length; i++)
		{
			Color colorR = Color.HSVToRGB((float)i / (float)flags.Length, saturation, value);
			PlantableObject plantableObject = flags[i];
			if ((bool)plantableObject)
			{
				plantableObject.colorR = colorR;
				plantableObject.colorG = Color.black;
			}
		}
	}

	public void Awake()
	{
		mode = new FlagCauldronColorer.ColorMode[flags.Length];
		flagColors = new PlantableObject.AppliedColors[flags.Length][];
		for (int i = 0; i < flags.Length; i++)
		{
			flagColors[i] = new PlantableObject.AppliedColors[20];
		}
	}

	public void Update()
	{
		if (mode == null)
		{
			mode = new FlagCauldronColorer.ColorMode[flags.Length];
		}
		if (flagColors == null)
		{
			flagColors = new PlantableObject.AppliedColors[flags.Length][];
			for (int i = 0; i < flags.Length; i++)
			{
				flagColors[i] = new PlantableObject.AppliedColors[20];
			}
		}
		for (int j = 0; j < flags.Length; j++)
		{
			PlantableObject plantableObject = flags[j];
			if (!plantableObject.IsMyItem() || Vector3.SqrMagnitude(plantableObject.flagTip.position - base.transform.position) > 25f)
			{
				continue;
			}
			bool flag = false;
			FlagCauldronColorer[] array = cauldrons;
			foreach (FlagCauldronColorer flagCauldronColorer in array)
			{
				if (!((double)Vector3.SqrMagnitude(plantableObject.flagTip.position - flagCauldronColorer.colorPoint.position) < 0.05))
				{
					continue;
				}
				flag = true;
				if (mode[j] != flagCauldronColorer.mode)
				{
					mode[j] = flagCauldronColorer.mode;
					PlantableObject.AppliedColors appliedColors = flagCauldronColorer.mode switch
					{
						FlagCauldronColorer.ColorMode.Red => PlantableObject.AppliedColors.Red, 
						FlagCauldronColorer.ColorMode.Green => PlantableObject.AppliedColors.Green, 
						FlagCauldronColorer.ColorMode.Blue => PlantableObject.AppliedColors.Blue, 
						FlagCauldronColorer.ColorMode.Black => PlantableObject.AppliedColors.Black, 
						FlagCauldronColorer.ColorMode.Clear => PlantableObject.AppliedColors.None, 
						_ => throw new ArgumentOutOfRangeException(), 
					};
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
						base.photonView.RPC("UpdateFlagColorRPC", RpcTarget.MasterClient, j, (int)appliedColors);
					}
					plantableObject.UpdateDisplayedDippedColor();
					flagColors[j] = plantableObject.dippedColors;
				}
				break;
			}
			if (!flag)
			{
				mode[j] = FlagCauldronColorer.ColorMode.None;
			}
		}
	}

	[PunRPC]
	public void UpdateFlagColorRPC(int flagIndex, int colorIndex, PhotonMessageInfo info)
	{
		PlantableObject plantableObject = flags[flagIndex];
		if (colorIndex == 0)
		{
			plantableObject.ClearColors();
		}
		else
		{
			plantableObject.AddColor((PlantableObject.AppliedColors)colorIndex);
		}
	}

	public void UpdateFlagColors()
	{
		for (int i = 0; i < flagColors.Length; i++)
		{
			PlantableObject.AppliedColors[] array = flagColors[i];
			PlantableObject plantableObject = flags[i];
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
			for (int i = 0; i < flagColors.Length; i++)
			{
				for (int j = 0; j < 20; j++)
				{
					stream.SendNext((int)flagColors[i][j]);
				}
			}
			return;
		}
		for (int k = 0; k < flagColors.Length; k++)
		{
			for (int l = 0; l < 20; l++)
			{
				flagColors[k][l] = (PlantableObject.AppliedColors)stream.ReceiveNext();
			}
		}
		UpdateFlagColors();
	}
}
