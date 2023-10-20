using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020001E7 RID: 487
[Serializable]
public class PhotonEvent : IOnEventCallback, IEquatable<PhotonEvent>
{
	// Token: 0x17000092 RID: 146
	// (get) Token: 0x06000C8E RID: 3214 RVA: 0x0004BB5D File Offset: 0x00049D5D
	// (set) Token: 0x06000C8F RID: 3215 RVA: 0x0004BB65 File Offset: 0x00049D65
	public bool reliable
	{
		get
		{
			return this._reliable;
		}
		set
		{
			this._reliable = value;
		}
	}

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x06000C90 RID: 3216 RVA: 0x0004BB6E File Offset: 0x00049D6E
	// (set) Token: 0x06000C91 RID: 3217 RVA: 0x0004BB76 File Offset: 0x00049D76
	public bool failSilent
	{
		get
		{
			return this._failSilent;
		}
		set
		{
			this._failSilent = value;
		}
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x0004BB7F File Offset: 0x00049D7F
	private PhotonEvent()
	{
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x0004BB8E File Offset: 0x00049D8E
	public PhotonEvent(int eventId)
	{
		if (eventId == -1)
		{
			throw new Exception(string.Format("<{0}> cannot be {1}.", "eventId", -1));
		}
		this._eventId = eventId;
		this.Enable();
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x0004BBC9 File Offset: 0x00049DC9
	public PhotonEvent(string eventId) : this(StaticHash.Calculate(eventId))
	{
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x0004BBD7 File Offset: 0x00049DD7
	public PhotonEvent(int eventId, Action<int, int, object[]> callback) : this(eventId)
	{
		this.AddCallback(callback);
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x0004BBE7 File Offset: 0x00049DE7
	public PhotonEvent(string eventId, Action<int, int, object[]> callback) : this(eventId)
	{
		this.AddCallback(callback);
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x0004BBF8 File Offset: 0x00049DF8
	~PhotonEvent()
	{
		this.Dispose();
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x0004BC24 File Offset: 0x00049E24
	public void AddCallback(Action<int, int, object[]> callback)
	{
		if (this._disposed)
		{
			return;
		}
		Delegate @delegate = this._delegate;
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		this._delegate = (Action<int, int, object[]>)Delegate.Combine(@delegate, callback);
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x0004BC55 File Offset: 0x00049E55
	public void RemoveCallback(Action<int, int, object[]> callback)
	{
		if (this._disposed)
		{
			return;
		}
		if (callback != null)
		{
			this._delegate = (Action<int, int, object[]>)Delegate.Remove(this._delegate, callback);
		}
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x0004BC7A File Offset: 0x00049E7A
	public void Enable()
	{
		if (this._disposed)
		{
			return;
		}
		if (this._enabled)
		{
			return;
		}
		if (Application.isPlaying)
		{
			PhotonNetwork.AddCallbackTarget(this);
		}
		this._enabled = true;
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x0004BCA2 File Offset: 0x00049EA2
	public void Disable()
	{
		if (this._disposed)
		{
			return;
		}
		if (!this._enabled)
		{
			return;
		}
		if (Application.isPlaying)
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}
		this._enabled = false;
	}

	// Token: 0x06000C9C RID: 3228 RVA: 0x0004BCCA File Offset: 0x00049ECA
	public void Dispose()
	{
		this._delegate = null;
		if (this._enabled)
		{
			this._enabled = false;
			if (Application.isPlaying)
			{
				PhotonNetwork.RemoveCallbackTarget(this);
			}
		}
		this._eventId = -1;
		this._disposed = true;
	}

	// Token: 0x14000016 RID: 22
	// (add) Token: 0x06000C9D RID: 3229 RVA: 0x0004BD00 File Offset: 0x00049F00
	// (remove) Token: 0x06000C9E RID: 3230 RVA: 0x0004BD34 File Offset: 0x00049F34
	public static event Action<PhotonEvent, Exception> OnError;

	// Token: 0x06000C9F RID: 3231 RVA: 0x0004BD68 File Offset: 0x00049F68
	void IOnEventCallback.OnEvent(EventData ev)
	{
		if (ev.Code != 176)
		{
			return;
		}
		if (this._disposed)
		{
			return;
		}
		if (!this._enabled)
		{
			return;
		}
		try
		{
			object[] array = (object[])ev.CustomData;
			if (array.Length == 0)
			{
				throw new Exception("Invalid/missing event data!");
			}
			int num = (int)array[0];
			int eventId = this._eventId;
			if (num == -1)
			{
				throw new Exception(string.Format("Invalid {0} ID! ({1})", "sender", -1));
			}
			if (eventId == -1)
			{
				throw new Exception(string.Format("Invalid {0} ID! ({1})", "receiver", -1));
			}
			object[] args = (array.Length == 1) ? Array.Empty<object>() : array.Skip(1).ToArray<object>();
			this.InvokeDelegate(num, eventId, args);
		}
		catch (Exception ex)
		{
			Action<PhotonEvent, Exception> onError = PhotonEvent.OnError;
			if (onError != null)
			{
				onError(this, ex);
			}
			if (!this._failSilent)
			{
				throw ex;
			}
		}
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x0004BE58 File Offset: 0x0004A058
	private void InvokeDelegate(int sender, int target, object[] args)
	{
		Action<int, int, object[]> @delegate = this._delegate;
		if (@delegate == null)
		{
			return;
		}
		@delegate(sender, target, args);
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x0004BE6D File Offset: 0x0004A06D
	public void RaiseLocal(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.Local, args);
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x0004BE77 File Offset: 0x0004A077
	public void RaiseOthers(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteOthers, args);
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x0004BE81 File Offset: 0x0004A081
	public void RaiseAll(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteAll, args);
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x0004BE8C File Offset: 0x0004A08C
	private void Raise(PhotonEvent.RaiseMode mode, params object[] args)
	{
		if (this._disposed)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			return;
		}
		if (!this._enabled)
		{
			return;
		}
		SendOptions sendOptions = this._reliable ? PhotonEvent.gSendReliable : PhotonEvent.gSendUnreliable;
		switch (mode)
		{
		case PhotonEvent.RaiseMode.Local:
			this.InvokeDelegate(this._eventId, this._eventId, args);
			return;
		case PhotonEvent.RaiseMode.RemoteOthers:
		{
			object[] eventContent = args.Prepend(this._eventId).ToArray<object>();
			PhotonNetwork.RaiseEvent(176, eventContent, PhotonEvent.gReceiversOthers, sendOptions);
			return;
		}
		case PhotonEvent.RaiseMode.RemoteAll:
		{
			object[] eventContent2 = args.Prepend(this._eventId).ToArray<object>();
			PhotonNetwork.RaiseEvent(176, eventContent2, PhotonEvent.gReceiversAll, sendOptions);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x0004BF44 File Offset: 0x0004A144
	public bool Equals(PhotonEvent other)
	{
		return !(other == null) && (this._eventId == other._eventId && this._enabled == other._enabled && this._reliable == other._reliable && this._failSilent == other._failSilent) && this._disposed == other._disposed;
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x0004BFA4 File Offset: 0x0004A1A4
	public override bool Equals(object obj)
	{
		PhotonEvent photonEvent = obj as PhotonEvent;
		return photonEvent != null && this.Equals(photonEvent);
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x0004BFC4 File Offset: 0x0004A1C4
	public override int GetHashCode()
	{
		int staticHash = this._eventId.GetStaticHash();
		int i = StaticHash.Combine(this._enabled, this._reliable, this._failSilent, this._disposed);
		return StaticHash.Combine(staticHash, i);
	}

	// Token: 0x06000CA8 RID: 3240 RVA: 0x0004C000 File Offset: 0x0004A200
	public static PhotonEvent operator +(PhotonEvent photonEvent, Action<int, int, object[]> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.AddCallback(callback);
		return photonEvent;
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x0004C01E File Offset: 0x0004A21E
	public static PhotonEvent operator -(PhotonEvent photonEvent, Action<int, int, object[]> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.RemoveCallback(callback);
		return photonEvent;
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x0004C03C File Offset: 0x0004A23C
	static PhotonEvent()
	{
		PhotonEvent.gSendUnreliable.Encrypt = true;
		PhotonEvent.gSendReliable = SendOptions.SendReliable;
		PhotonEvent.gSendReliable.Encrypt = true;
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x0004C095 File Offset: 0x0004A295
	public static bool operator ==(PhotonEvent x, PhotonEvent y)
	{
		return EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x0004C0A3 File Offset: 0x0004A2A3
	public static bool operator !=(PhotonEvent x, PhotonEvent y)
	{
		return !EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	// Token: 0x04000FF9 RID: 4089
	private const int INVALID_ID = -1;

	// Token: 0x04000FFA RID: 4090
	[SerializeField]
	private int _eventId = -1;

	// Token: 0x04000FFB RID: 4091
	[SerializeField]
	private bool _enabled;

	// Token: 0x04000FFC RID: 4092
	[SerializeField]
	private bool _reliable;

	// Token: 0x04000FFD RID: 4093
	[SerializeField]
	private bool _failSilent;

	// Token: 0x04000FFE RID: 4094
	[NonSerialized]
	private bool _disposed;

	// Token: 0x04000FFF RID: 4095
	private Action<int, int, object[]> _delegate;

	// Token: 0x04001001 RID: 4097
	public const byte PHOTON_EVENT_CODE = 176;

	// Token: 0x04001002 RID: 4098
	private static readonly RaiseEventOptions gReceiversAll = new RaiseEventOptions
	{
		Receivers = ReceiverGroup.All
	};

	// Token: 0x04001003 RID: 4099
	private static readonly RaiseEventOptions gReceiversOthers = new RaiseEventOptions
	{
		Receivers = ReceiverGroup.Others
	};

	// Token: 0x04001004 RID: 4100
	private static readonly SendOptions gSendReliable;

	// Token: 0x04001005 RID: 4101
	private static readonly SendOptions gSendUnreliable = SendOptions.SendUnreliable;

	// Token: 0x0200046E RID: 1134
	public enum RaiseMode
	{
		// Token: 0x04001E82 RID: 7810
		Local,
		// Token: 0x04001E83 RID: 7811
		RemoteOthers,
		// Token: 0x04001E84 RID: 7812
		RemoteAll
	}
}
