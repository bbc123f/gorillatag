using System;
using UnityEngine;

public class BuildTargetManager : MonoBehaviour
{
	public string GetPath()
	{
		return this.path;
	}

	public BuildTargetManager()
	{
	}

	public BuildTargetManager.BuildTowards newBuildTarget;

	public bool isBeta;

	public bool isQA;

	public bool spoofIDs;

	public bool spoofChild;

	public bool enableAllCosmetics;

	public OVRManager ovrManager;

	private string path = "Assets/csc.rsp";

	public BuildTargetManager.BuildTowards currentBuildTargetDONOTCHANGE;

	public GorillaTagger gorillaTagger;

	public GameObject[] betaDisableObjects;

	public GameObject[] betaEnableObjects;

	public enum BuildTowards
	{
		Steam,
		OculusPC,
		Quest,
		Viveport
	}
}
