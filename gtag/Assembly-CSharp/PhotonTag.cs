using System;
using System.Diagnostics;
using BuildSafe;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020001E7 RID: 487
public class PhotonTag : MonoBehaviour, IOnEventCallback, IEquatable<PhotonTag>
{
	// Token: 0x17000092 RID: 146
	// (get) Token: 0x06000CA7 RID: 3239 RVA: 0x0004BE4C File Offset: 0x0004A04C
	public Id128 TagId
	{
		get
		{
			return this._tagId;
		}
	}

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x06000CA8 RID: 3240 RVA: 0x0004BE54 File Offset: 0x0004A054
	public Id128 SubId
	{
		get
		{
			return this._subId;
		}
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x0004BE5C File Offset: 0x0004A05C
	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.AddCallbackTarget(this);
		}
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x0004BE6B File Offset: 0x0004A06B
	private void OnDisable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x0004BE7A File Offset: 0x0004A07A
	void IOnEventCallback.OnEvent(EventData ev)
	{
		byte code = ev.Code;
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x0004BE89 File Offset: 0x0004A089
	[Conditional("UNITY_EDITOR")]
	private void Reset()
	{
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x0004BE8B File Offset: 0x0004A08B
	[Conditional("UNITY_EDITOR")]
	private void ComputeID()
	{
		if (Application.isPlaying)
		{
			return;
		}
		this._tagId = BuildSafe.ComponentUtils.GetComponentID(this, 0);
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x0004BEA8 File Offset: 0x0004A0A8
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

	// Token: 0x06000CAF RID: 3247 RVA: 0x0004BEF4 File Offset: 0x0004A0F4
	public override bool Equals(object obj)
	{
		if (this != obj)
		{
			PhotonTag photonTag = obj as PhotonTag;
			return photonTag != null && this.Equals(photonTag);
		}
		return true;
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x0004BF1A File Offset: 0x0004A11A
	public override int GetHashCode()
	{
		return StaticHash.Combine(this._tagId.GetHashCode(), this._subId.GetHashCode());
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x0004BF43 File Offset: 0x0004A143
	public static bool operator ==(PhotonTag x, PhotonTag y)
	{
		return object.Equals(x, y);
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x0004BF4C File Offset: 0x0004A14C
	public static bool operator !=(PhotonTag x, PhotonTag y)
	{
		return !object.Equals(x, y);
	}

	// Token: 0x04001002 RID: 4098
	public const byte PHOTON_TAG_CODE = 177;

	// Token: 0x04001003 RID: 4099
	[SerializeField]
	private Id128 _tagId;

	// Token: 0x04001004 RID: 4100
	[SerializeField]
	private Id128 _subId;
}
