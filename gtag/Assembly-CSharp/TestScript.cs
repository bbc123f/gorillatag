using System;
using Photon.Pun;
using UnityEngine;

public class TestScript : MonoBehaviourPunCallbacks
{
	public int callbackOrder
	{
		get
		{
			return 0;
		}
	}

	public GameObject testDelete;
}
