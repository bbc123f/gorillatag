using System;
using System.Runtime.CompilerServices;
using UnityEngine;

internal class TickSystem : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Update()
	{
		TickSystem.preTickCallbacks.TryRunCallbacks();
		TickSystem.tickCallbacks.TryRunCallbacks();
	}

	private void LateUpdate()
	{
		TickSystem.postTickCallbacks.TryRunCallbacks();
	}

	static TickSystem()
	{
		TickSystem.preTickCallbacks.SetRunUpdateCallback(new CallbackContainer<ITickSystemPre>.runCallbacksDelegate(TickSystem.CallPreTick));
		TickSystem.tickCallbacks.SetRunUpdateCallback(new CallbackContainer<ITickSystemTick>.runCallbacksDelegate(TickSystem.CallTick));
		TickSystem.postTickCallbacks.SetRunUpdateCallback(new CallbackContainer<ITickSystemPost>.runCallbacksDelegate(TickSystem.CallPostTick));
	}

	private static void CallPreTick(ITickSystemPre callback)
	{
		callback.PreTick();
	}

	private static void CallTick(ITickSystemTick callback)
	{
		callback.Tick();
	}

	private static void CallPostTick(ITickSystemPost callback)
	{
		callback.PostTick();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPreTickCallback(ITickSystemPre callback)
	{
		TickSystem.preTickCallbacks.Add(callback);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddTickCallback(ITickSystemTick callback)
	{
		TickSystem.tickCallbacks.Add(callback);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPostTickCallback(ITickSystemPost callback)
	{
		TickSystem.postTickCallbacks.Add(callback);
	}

	public static void AddTickSystemCallBack(ITickSystem callback)
	{
		TickSystem.preTickCallbacks.Add(callback);
		TickSystem.tickCallbacks.Add(callback);
		TickSystem.postTickCallbacks.Add(callback);
	}

	public static void AddCallbackTarget(object target)
	{
		ITickSystem tickSystem = target as ITickSystem;
		if (tickSystem != null)
		{
			TickSystem.AddTickSystemCallBack(tickSystem);
			return;
		}
		ITickSystemPre tickSystemPre = target as ITickSystemPre;
		if (tickSystemPre != null)
		{
			TickSystem.AddPreTickCallback(tickSystemPre);
		}
		ITickSystemTick tickSystemTick = target as ITickSystemTick;
		if (tickSystemTick != null)
		{
			TickSystem.AddTickCallback(tickSystemTick);
		}
		ITickSystemPost tickSystemPost = target as ITickSystemPost;
		if (tickSystemPost != null)
		{
			TickSystem.AddPostTickCallback(tickSystemPost);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePreTickCallback(ITickSystemPre callback)
	{
		TickSystem.preTickCallbacks.Remove(callback);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveTickCallback(ITickSystemTick callback)
	{
		TickSystem.tickCallbacks.Remove(callback);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePostTickCallback(ITickSystemPost callback)
	{
		TickSystem.postTickCallbacks.Remove(callback);
	}

	public static void RemoveTickSystemCallback(ITickSystem callback)
	{
		TickSystem.RemovePreTickCallback(callback);
		TickSystem.RemoveTickCallback(callback);
		TickSystem.RemovePostTickCallback(callback);
	}

	public static void RemoveCallbackTarget(object target)
	{
		ITickSystem tickSystem = target as ITickSystem;
		if (tickSystem != null)
		{
			TickSystem.RemoveTickSystemCallback(tickSystem);
			return;
		}
		ITickSystemPre tickSystemPre = target as ITickSystemPre;
		if (tickSystemPre != null)
		{
			TickSystem.RemovePreTickCallback(tickSystemPre);
		}
		ITickSystemTick tickSystemTick = target as ITickSystemTick;
		if (tickSystemTick != null)
		{
			TickSystem.RemoveTickCallback(tickSystemTick);
		}
		ITickSystemPost tickSystemPost = target as ITickSystemPost;
		if (tickSystemPost != null)
		{
			TickSystem.RemovePostTickCallback(tickSystemPost);
		}
	}

	private static CallbackContainer<ITickSystemPre> preTickCallbacks = new CallbackContainer<ITickSystemPre>();

	private static CallbackContainer<ITickSystemTick> tickCallbacks = new CallbackContainer<ITickSystemTick>();

	private static CallbackContainer<ITickSystemPost> postTickCallbacks = new CallbackContainer<ITickSystemPost>();
}
