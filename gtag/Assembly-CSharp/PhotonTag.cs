using System;
using System.Diagnostics;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonTag : MonoBehaviour, IOnEventCallback, IEquatable<PhotonTag>
{
	public Id128 TagId
	{
		get
		{
			return this._tagId;
		}
	}

	public Id128 SubId
	{
		get
		{
			return this._subId;
		}
	}

	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.AddCallbackTarget(this);
		}
	}

	private void OnDisable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}
	}

	void IOnEventCallback.OnEvent(EventData ev)
	{
		byte code = ev.Code;
	}

	[Conditional("UNITY_EDITOR")]
	private void Reset()
	{
	}

	[Conditional("UNITY_EDITOR")]
	private void ComputeID()
	{
		if (Application.isPlaying)
		{
			return;
		}
		this._tagId = ComponentUtils.ComputeStaticHash128(this, 0);
	}

	public bool Equals(PhotonTag other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		bool flag = this._tagId.Equals(other._tagId) && this._subId.Equals(other._subId);
		return base.Equals(other) && flag;
	}

	public override bool Equals(object obj)
	{
		if (this != obj)
		{
			PhotonTag photonTag = obj as PhotonTag;
			return photonTag != null && this.Equals(photonTag);
		}
		return true;
	}

	public override int GetHashCode()
	{
		return StaticHash.Combine(this._tagId.GetHashCode(), this._subId.GetHashCode());
	}

	public static bool operator ==(PhotonTag x, PhotonTag y)
	{
		return object.Equals(x, y);
	}

	public static bool operator !=(PhotonTag x, PhotonTag y)
	{
		return !object.Equals(x, y);
	}

	public PhotonTag()
	{
	}

	public const byte PHOTON_TAG_CODE = 177;

	[SerializeField]
	private Id128 _tagId;

	[SerializeField]
	private Id128 _subId;
}
