using System;
using System.Diagnostics;
using BuildSafe;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020001E8 RID: 488
public class PhotonTag : MonoBehaviour, IOnEventCallback, IEquatable<PhotonTag>
{
	// Token: 0x17000094 RID: 148
	// (get) Token: 0x06000CAD RID: 3245 RVA: 0x0004C0B4 File Offset: 0x0004A2B4
	public Id128 TagId
	{
		get
		{
			return this._tagId;
		}
	}

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x06000CAE RID: 3246 RVA: 0x0004C0BC File Offset: 0x0004A2BC
	public Id128 SubId
	{
		get
		{
			return this._subId;
		}
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x0004C0C4 File Offset: 0x0004A2C4
	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.AddCallbackTarget(this);
		}
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x0004C0D3 File Offset: 0x0004A2D3
	private void OnDisable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x0004C0E2 File Offset: 0x0004A2E2
	void IOnEventCallback.OnEvent(EventData ev)
	{
		byte code = ev.Code;
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x0004C0F1 File Offset: 0x0004A2F1
	[Conditional("UNITY_EDITOR")]
	private void Reset()
	{
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x0004C0F3 File Offset: 0x0004A2F3
	[Conditional("UNITY_EDITOR")]
	private void ComputeID()
	{
		if (Application.isPlaying)
		{
			return;
		}
		this._tagId = BuildSafe.ComponentUtils.GetComponentID(this, 0);
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x0004C110 File Offset: 0x0004A310
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

	// Token: 0x06000CB5 RID: 3253 RVA: 0x0004C15C File Offset: 0x0004A35C
	public override bool Equals(object obj)
	{
		if (this != obj)
		{
			PhotonTag photonTag = obj as PhotonTag;
			return photonTag != null && this.Equals(photonTag);
		}
		return true;
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x0004C182 File Offset: 0x0004A382
	public override int GetHashCode()
	{
		return StaticHash.Combine(this._tagId.GetHashCode(), this._subId.GetHashCode());
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x0004C1AB File Offset: 0x0004A3AB
	public static bool operator ==(PhotonTag x, PhotonTag y)
	{
		return object.Equals(x, y);
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x0004C1B4 File Offset: 0x0004A3B4
	public static bool operator !=(PhotonTag x, PhotonTag y)
	{
		return !object.Equals(x, y);
	}

	// Token: 0x04001006 RID: 4102
	public const byte PHOTON_TAG_CODE = 177;

	// Token: 0x04001007 RID: 4103
	[SerializeField]
	private Id128 _tagId;

	// Token: 0x04001008 RID: 4104
	[SerializeField]
	private Id128 _subId;
}
