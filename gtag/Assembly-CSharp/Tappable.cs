﻿using System;
using System.Diagnostics;
using Photon.Pun;
using UnityEngine;

public class Tappable : MonoBehaviour
{
	public void Validate()
	{
		this.CalculateId(true);
	}

	protected virtual void OnEnable()
	{
		if (!this.useStaticId)
		{
			this.CalculateId(false);
		}
		TappableManager.Register(this);
	}

	protected virtual void OnDisable()
	{
		TappableManager.Unregister(this);
	}

	public void OnTap(float tapStrength, float tapTime)
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnTapRPC", this.rpcTarget, new object[]
		{
			this.tappableId,
			tapStrength
		});
	}

	public virtual void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfo sender)
	{
	}

	private void EdRecalculateId()
	{
		this.CalculateId(true);
	}

	private void CalculateId(bool force = false)
	{
		Transform transform = base.transform;
		int staticHash = TransformUtils.GetScenePath(transform).GetStaticHash();
		int staticHash2 = base.GetType().Name.GetStaticHash();
		int num = StaticHash.Combine(staticHash, staticHash2);
		if (this.useStaticId)
		{
			if (string.IsNullOrEmpty(this.staticId) || force)
			{
				Vector3 position = transform.position;
				int i = StaticHash.Combine(position.x, position.y, position.z);
				int instanceID = transform.GetInstanceID();
				int num2 = StaticHash.Combine(num, i, instanceID);
				this.staticId = string.Format("#ID_{0:X8}", num2);
			}
			this.tappableId = this.staticId.GetStaticHash();
			return;
		}
		this.tappableId = (Application.isPlaying ? num : 0);
	}

	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		this.CalculateId(false);
	}

	public Tappable()
	{
	}

	public int tappableId;

	public string staticId;

	public bool useStaticId;

	[Space]
	public TappableManager manager;

	public RpcTarget rpcTarget;
}
