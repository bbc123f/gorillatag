﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[Serializable]
public class PhotonEvent : IOnEventCallback, IEquatable<PhotonEvent>
{
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

	private PhotonEvent()
	{
	}

	public PhotonEvent(int eventId)
	{
		if (eventId == -1)
		{
			throw new Exception(string.Format("<{0}> cannot be {1}.", "eventId", -1));
		}
		this._eventId = eventId;
		this.Enable();
	}

	public PhotonEvent(string eventId) : this(StaticHash.Calculate(eventId))
	{
	}

	public PhotonEvent(int eventId, Action<int, int, object[], PhotonMessageInfoWrapped> callback) : this(eventId)
	{
		this.AddCallback(callback);
	}

	public PhotonEvent(string eventId, Action<int, int, object[], PhotonMessageInfoWrapped> callback) : this(eventId)
	{
		this.AddCallback(callback);
	}

	~PhotonEvent()
	{
		this.Dispose();
	}

	public void AddCallback(Action<int, int, object[], PhotonMessageInfoWrapped> callback)
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
		this._delegate = (Action<int, int, object[], PhotonMessageInfoWrapped>)Delegate.Combine(@delegate, callback);
	}

	public void RemoveCallback(Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (this._disposed)
		{
			return;
		}
		if (callback != null)
		{
			this._delegate = (Action<int, int, object[], PhotonMessageInfoWrapped>)Delegate.Remove(this._delegate, callback);
		}
	}

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

	public static event Action<PhotonEvent, Exception> OnError
	{
		[CompilerGenerated]
		add
		{
			Action<PhotonEvent, Exception> action = PhotonEvent.OnError;
			Action<PhotonEvent, Exception> action2;
			do
			{
				action2 = action;
				Action<PhotonEvent, Exception> value2 = (Action<PhotonEvent, Exception>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action<PhotonEvent, Exception>>(ref PhotonEvent.OnError, value2, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action<PhotonEvent, Exception> action = PhotonEvent.OnError;
			Action<PhotonEvent, Exception> action2;
			do
			{
				action2 = action;
				Action<PhotonEvent, Exception> value2 = (Action<PhotonEvent, Exception>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action<PhotonEvent, Exception>>(ref PhotonEvent.OnError, value2, action2);
			}
			while (action != action2);
		}
	}

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
			PhotonMessageInfoWrapped info = new PhotonMessageInfoWrapped(ev.Sender, PhotonNetwork.ServerTimestamp);
			this.InvokeDelegate(num, eventId, args, info);
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

	private void InvokeDelegate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		Action<int, int, object[], PhotonMessageInfoWrapped> @delegate = this._delegate;
		if (@delegate == null)
		{
			return;
		}
		@delegate(sender, target, args, info);
	}

	public void RaiseLocal(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.Local, args);
	}

	public void RaiseOthers(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteOthers, args);
	}

	public void RaiseAll(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteAll, args);
	}

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
			this.InvokeDelegate(this._eventId, this._eventId, args, new PhotonMessageInfoWrapped(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.ServerTimestamp));
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

	public bool Equals(PhotonEvent other)
	{
		return !(other == null) && (this._eventId == other._eventId && this._enabled == other._enabled && this._reliable == other._reliable && this._failSilent == other._failSilent) && this._disposed == other._disposed;
	}

	public override bool Equals(object obj)
	{
		PhotonEvent photonEvent = obj as PhotonEvent;
		return photonEvent != null && this.Equals(photonEvent);
	}

	public override int GetHashCode()
	{
		int staticHash = this._eventId.GetStaticHash();
		int i = StaticHash.Combine(this._enabled, this._reliable, this._failSilent, this._disposed);
		return StaticHash.Combine(staticHash, i);
	}

	public static PhotonEvent operator +(PhotonEvent photonEvent, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.AddCallback(callback);
		return photonEvent;
	}

	public static PhotonEvent operator -(PhotonEvent photonEvent, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.RemoveCallback(callback);
		return photonEvent;
	}

	static PhotonEvent()
	{
		PhotonEvent.gSendUnreliable.Encrypt = true;
		PhotonEvent.gSendReliable = SendOptions.SendReliable;
		PhotonEvent.gSendReliable.Encrypt = true;
	}

	public static bool operator ==(PhotonEvent x, PhotonEvent y)
	{
		return EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	public static bool operator !=(PhotonEvent x, PhotonEvent y)
	{
		return !EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	private const int INVALID_ID = -1;

	[SerializeField]
	private int _eventId = -1;

	[SerializeField]
	private bool _enabled;

	[SerializeField]
	private bool _reliable;

	[SerializeField]
	private bool _failSilent;

	[NonSerialized]
	private bool _disposed;

	private Action<int, int, object[], PhotonMessageInfoWrapped> _delegate;

	[CompilerGenerated]
	private static Action<PhotonEvent, Exception> OnError;

	public const byte PHOTON_EVENT_CODE = 176;

	private static readonly RaiseEventOptions gReceiversAll = new RaiseEventOptions
	{
		Receivers = ReceiverGroup.All
	};

	private static readonly RaiseEventOptions gReceiversOthers = new RaiseEventOptions
	{
		Receivers = ReceiverGroup.Others
	};

	private static readonly SendOptions gSendReliable;

	private static readonly SendOptions gSendUnreliable = SendOptions.SendUnreliable;

	public enum RaiseMode
	{
		Local,
		RemoteOthers,
		RemoteAll
	}
}
