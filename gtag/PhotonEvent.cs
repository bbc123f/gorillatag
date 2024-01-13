using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utilities;

[Serializable]
public class PhotonEvent : IOnEventCallback, IEquatable<PhotonEvent>
{
	public enum RaiseMode
	{
		Local,
		RemoteOthers,
		RemoteAll
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

	private Action<int, int, object[]> _delegate;

	private const byte PHOTON_EVENT_CODE = 176;

	private static readonly RaiseEventOptions gReceiversAll;

	private static readonly RaiseEventOptions gReceiversOthers;

	private static readonly SendOptions gSendReliable;

	private static readonly SendOptions gSendUnreliable;

	public bool reliable
	{
		get
		{
			return _reliable;
		}
		set
		{
			_reliable = value;
		}
	}

	public bool failSilent
	{
		get
		{
			return _failSilent;
		}
		set
		{
			_failSilent = value;
		}
	}

	public static event Action<PhotonEvent, Exception> OnError;

	private PhotonEvent()
	{
	}

	public PhotonEvent(int eventId)
	{
		if (eventId == -1)
		{
			throw new Exception(string.Format("<{0}> cannot be {1}.", "eventId", -1));
		}
		_eventId = eventId;
		Enable();
	}

	public PhotonEvent(string eventId)
		: this(StaticHash.Calculate(eventId))
	{
	}

	public PhotonEvent(int eventId, Action<int, int, object[]> callback)
		: this(eventId)
	{
		AddCallback(callback);
	}

	~PhotonEvent()
	{
		Dispose();
	}

	public void AddCallback(Action<int, int, object[]> callback)
	{
		if (!_disposed)
		{
			_delegate = (Action<int, int, object[]>)Delegate.Combine(_delegate, callback ?? throw new ArgumentNullException("callback"));
		}
	}

	public void RemoveCallback(Action<int, int, object[]> callback)
	{
		if (!_disposed && callback != null)
		{
			_delegate = (Action<int, int, object[]>)Delegate.Remove(_delegate, callback);
		}
	}

	public void Enable()
	{
		if (!_disposed && !_enabled)
		{
			if (Application.isPlaying)
			{
				PhotonNetwork.AddCallbackTarget(this);
			}
			_enabled = true;
		}
	}

	public void Disable()
	{
		if (!_disposed && _enabled)
		{
			if (Application.isPlaying)
			{
				PhotonNetwork.RemoveCallbackTarget(this);
			}
			_enabled = false;
		}
	}

	public void Dispose()
	{
		_delegate = null;
		if (_enabled)
		{
			_enabled = false;
			if (Application.isPlaying)
			{
				PhotonNetwork.RemoveCallbackTarget(this);
			}
		}
		_eventId = -1;
		_disposed = true;
	}

	void IOnEventCallback.OnEvent(EventData ev)
	{
		if (ev.Code != 176 || _disposed || !_enabled)
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
			int eventId = _eventId;
			if (num == -1)
			{
				throw new Exception(string.Format("Invalid {0} ID! ({1})", "sender", -1));
			}
			if (eventId == -1)
			{
				throw new Exception(string.Format("Invalid {0} ID! ({1})", "receiver", -1));
			}
			object[] args = ((array.Length == 1) ? Array.Empty<object>() : array.Skip(1).ToArray());
			InvokeDelegate(num, eventId, args);
		}
		catch (Exception ex)
		{
			PhotonEvent.OnError?.Invoke(this, ex);
			if (!_failSilent)
			{
				throw ex;
			}
		}
	}

	private void InvokeDelegate(int sender, int target, object[] args)
	{
		_delegate?.Invoke(sender, target, args);
	}

	public void RaiseLocal(params object[] args)
	{
		Raise(RaiseMode.Local, args);
	}

	public void RaiseOthers(params object[] args)
	{
		Raise(RaiseMode.RemoteOthers, args);
	}

	public void RaiseAll(params object[] args)
	{
		Raise(RaiseMode.RemoteAll, args);
	}

	private void Raise(RaiseMode mode, params object[] args)
	{
		if (!_disposed && Application.isPlaying && _enabled)
		{
			SendOptions sendOptions = (_reliable ? gSendReliable : gSendUnreliable);
			switch (mode)
			{
			case RaiseMode.Local:
				InvokeDelegate(_eventId, _eventId, args);
				break;
			case RaiseMode.RemoteOthers:
			{
				object[] eventContent2 = args.Prepend(_eventId).ToArray();
				PhotonNetwork.RaiseEvent(176, eventContent2, gReceiversOthers, sendOptions);
				break;
			}
			case RaiseMode.RemoteAll:
			{
				object[] eventContent = args.Prepend(_eventId).ToArray();
				PhotonNetwork.RaiseEvent(176, eventContent, gReceiversAll, sendOptions);
				break;
			}
			}
		}
	}

	public bool Equals(PhotonEvent other)
	{
		if (other == null)
		{
			return false;
		}
		if (_eventId == other._eventId && _enabled == other._enabled && _reliable == other._reliable && _failSilent == other._failSilent)
		{
			return _disposed == other._disposed;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is PhotonEvent other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int staticHash = _enabled.GetStaticHash();
		int staticHash2 = _reliable.GetStaticHash();
		int staticHash3 = _failSilent.GetStaticHash();
		int staticHash4 = _disposed.GetStaticHash();
		int staticHash5 = _eventId.GetStaticHash();
		int staticHash6 = (i1: staticHash, i2: staticHash2, i3: staticHash3, i4: staticHash4).GetStaticHash();
		return (i1: staticHash5, i2: staticHash6).GetStaticHash();
	}

	public static PhotonEvent operator +(PhotonEvent photonEvent, Action<int, int, object[]> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.AddCallback(callback);
		return photonEvent;
	}

	public static PhotonEvent operator -(PhotonEvent photonEvent, Action<int, int, object[]> callback)
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
		gReceiversAll = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.All
		};
		gReceiversOthers = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.Others
		};
		gSendUnreliable = SendOptions.SendUnreliable;
		gSendUnreliable.Encrypt = true;
		gSendReliable = SendOptions.SendReliable;
		gSendReliable.Encrypt = true;
	}

	public static bool operator ==(PhotonEvent x, PhotonEvent y)
	{
		return EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	public static bool operator !=(PhotonEvent x, PhotonEvent y)
	{
		return !EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}
}
