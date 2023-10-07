using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020001E6 RID: 486
[Serializable]
public class PhotonEvent : IOnEventCallback, IEquatable<PhotonEvent>
{
	// Token: 0x17000090 RID: 144
	// (get) Token: 0x06000C88 RID: 3208 RVA: 0x0004B8F5 File Offset: 0x00049AF5
	// (set) Token: 0x06000C89 RID: 3209 RVA: 0x0004B8FD File Offset: 0x00049AFD
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

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x06000C8A RID: 3210 RVA: 0x0004B906 File Offset: 0x00049B06
	// (set) Token: 0x06000C8B RID: 3211 RVA: 0x0004B90E File Offset: 0x00049B0E
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

	// Token: 0x06000C8C RID: 3212 RVA: 0x0004B917 File Offset: 0x00049B17
	private PhotonEvent()
	{
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x0004B926 File Offset: 0x00049B26
	public PhotonEvent(int eventId)
	{
		if (eventId == -1)
		{
			throw new Exception(string.Format("<{0}> cannot be {1}.", "eventId", -1));
		}
		this._eventId = eventId;
		this.Enable();
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x0004B961 File Offset: 0x00049B61
	public PhotonEvent(string eventId) : this(StaticHash.Calculate(eventId))
	{
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x0004B96F File Offset: 0x00049B6F
	public PhotonEvent(int eventId, Action<int, int, object[]> callback) : this(eventId)
	{
		this.AddCallback(callback);
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x0004B97F File Offset: 0x00049B7F
	public PhotonEvent(string eventId, Action<int, int, object[]> callback) : this(eventId)
	{
		this.AddCallback(callback);
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x0004B990 File Offset: 0x00049B90
	~PhotonEvent()
	{
		this.Dispose();
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x0004B9BC File Offset: 0x00049BBC
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

	// Token: 0x06000C93 RID: 3219 RVA: 0x0004B9ED File Offset: 0x00049BED
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

	// Token: 0x06000C94 RID: 3220 RVA: 0x0004BA12 File Offset: 0x00049C12
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

	// Token: 0x06000C95 RID: 3221 RVA: 0x0004BA3A File Offset: 0x00049C3A
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

	// Token: 0x06000C96 RID: 3222 RVA: 0x0004BA62 File Offset: 0x00049C62
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
	// (add) Token: 0x06000C97 RID: 3223 RVA: 0x0004BA98 File Offset: 0x00049C98
	// (remove) Token: 0x06000C98 RID: 3224 RVA: 0x0004BACC File Offset: 0x00049CCC
	public static event Action<PhotonEvent, Exception> OnError;

	// Token: 0x06000C99 RID: 3225 RVA: 0x0004BB00 File Offset: 0x00049D00
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

	// Token: 0x06000C9A RID: 3226 RVA: 0x0004BBF0 File Offset: 0x00049DF0
	private void InvokeDelegate(int sender, int target, object[] args)
	{
		Action<int, int, object[]> @delegate = this._delegate;
		if (@delegate == null)
		{
			return;
		}
		@delegate(sender, target, args);
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x0004BC05 File Offset: 0x00049E05
	public void RaiseLocal(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.Local, args);
	}

	// Token: 0x06000C9C RID: 3228 RVA: 0x0004BC0F File Offset: 0x00049E0F
	public void RaiseOthers(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteOthers, args);
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x0004BC19 File Offset: 0x00049E19
	public void RaiseAll(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteAll, args);
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x0004BC24 File Offset: 0x00049E24
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

	// Token: 0x06000C9F RID: 3231 RVA: 0x0004BCDC File Offset: 0x00049EDC
	public bool Equals(PhotonEvent other)
	{
		return !(other == null) && (this._eventId == other._eventId && this._enabled == other._enabled && this._reliable == other._reliable && this._failSilent == other._failSilent) && this._disposed == other._disposed;
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x0004BD3C File Offset: 0x00049F3C
	public override bool Equals(object obj)
	{
		PhotonEvent photonEvent = obj as PhotonEvent;
		return photonEvent != null && this.Equals(photonEvent);
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x0004BD5C File Offset: 0x00049F5C
	public override int GetHashCode()
	{
		int staticHash = this._eventId.GetStaticHash();
		int i = StaticHash.Combine(this._enabled, this._reliable, this._failSilent, this._disposed);
		return StaticHash.Combine(staticHash, i);
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x0004BD98 File Offset: 0x00049F98
	public static PhotonEvent operator +(PhotonEvent photonEvent, Action<int, int, object[]> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.AddCallback(callback);
		return photonEvent;
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x0004BDB6 File Offset: 0x00049FB6
	public static PhotonEvent operator -(PhotonEvent photonEvent, Action<int, int, object[]> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.RemoveCallback(callback);
		return photonEvent;
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x0004BDD4 File Offset: 0x00049FD4
	static PhotonEvent()
	{
		PhotonEvent.gSendUnreliable.Encrypt = true;
		PhotonEvent.gSendReliable = SendOptions.SendReliable;
		PhotonEvent.gSendReliable.Encrypt = true;
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x0004BE2D File Offset: 0x0004A02D
	public static bool operator ==(PhotonEvent x, PhotonEvent y)
	{
		return EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x0004BE3B File Offset: 0x0004A03B
	public static bool operator !=(PhotonEvent x, PhotonEvent y)
	{
		return !EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	// Token: 0x04000FF5 RID: 4085
	private const int INVALID_ID = -1;

	// Token: 0x04000FF6 RID: 4086
	[SerializeField]
	private int _eventId = -1;

	// Token: 0x04000FF7 RID: 4087
	[SerializeField]
	private bool _enabled;

	// Token: 0x04000FF8 RID: 4088
	[SerializeField]
	private bool _reliable;

	// Token: 0x04000FF9 RID: 4089
	[SerializeField]
	private bool _failSilent;

	// Token: 0x04000FFA RID: 4090
	[NonSerialized]
	private bool _disposed;

	// Token: 0x04000FFB RID: 4091
	private Action<int, int, object[]> _delegate;

	// Token: 0x04000FFD RID: 4093
	public const byte PHOTON_EVENT_CODE = 176;

	// Token: 0x04000FFE RID: 4094
	private static readonly RaiseEventOptions gReceiversAll = new RaiseEventOptions
	{
		Receivers = ReceiverGroup.All
	};

	// Token: 0x04000FFF RID: 4095
	private static readonly RaiseEventOptions gReceiversOthers = new RaiseEventOptions
	{
		Receivers = ReceiverGroup.Others
	};

	// Token: 0x04001000 RID: 4096
	private static readonly SendOptions gSendReliable;

	// Token: 0x04001001 RID: 4097
	private static readonly SendOptions gSendUnreliable = SendOptions.SendUnreliable;

	// Token: 0x0200046C RID: 1132
	public enum RaiseMode
	{
		// Token: 0x04001E75 RID: 7797
		Local,
		// Token: 0x04001E76 RID: 7798
		RemoteOthers,
		// Token: 0x04001E77 RID: 7799
		RemoteAll
	}
}
