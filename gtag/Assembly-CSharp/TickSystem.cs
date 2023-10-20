using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020001FE RID: 510
internal class TickSystem : MonoBehaviour
{
	// Token: 0x06000D05 RID: 3333 RVA: 0x0004CF96 File Offset: 0x0004B196
	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	// Token: 0x06000D06 RID: 3334 RVA: 0x0004CF9E File Offset: 0x0004B19E
	private void Update()
	{
		TickSystem.preTickCallbacks.TryRunCallbacks();
		TickSystem.tickCallbacks.TryRunCallbacks();
	}

	// Token: 0x06000D07 RID: 3335 RVA: 0x0004CFB4 File Offset: 0x0004B1B4
	private void LateUpdate()
	{
		TickSystem.postTickCallbacks.TryRunCallbacks();
	}

	// Token: 0x06000D08 RID: 3336 RVA: 0x0004CFC0 File Offset: 0x0004B1C0
	static TickSystem()
	{
		TickSystem.preTickCallbacks.SetRunUpdateCallback(new CallbackContainer<ITickSystemPre>.runCallbacksDelegate(TickSystem.CallPreTick));
		TickSystem.tickCallbacks.SetRunUpdateCallback(new CallbackContainer<ITickSystemTick>.runCallbacksDelegate(TickSystem.CallTick));
		TickSystem.postTickCallbacks.SetRunUpdateCallback(new CallbackContainer<ITickSystemPost>.runCallbacksDelegate(TickSystem.CallPostTick));
	}

	// Token: 0x06000D09 RID: 3337 RVA: 0x0004D02D File Offset: 0x0004B22D
	private static void CallPreTick(ITickSystemPre callback)
	{
		callback.PreTick();
	}

	// Token: 0x06000D0A RID: 3338 RVA: 0x0004D035 File Offset: 0x0004B235
	private static void CallTick(ITickSystemTick callback)
	{
		callback.Tick();
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x0004D03D File Offset: 0x0004B23D
	private static void CallPostTick(ITickSystemPost callback)
	{
		callback.PostTick();
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x0004D045 File Offset: 0x0004B245
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPreTickCallback(ITickSystemPre callback)
	{
		TickSystem.preTickCallbacks.Add(callback);
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x0004D052 File Offset: 0x0004B252
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddTickCallback(ITickSystemTick callback)
	{
		TickSystem.tickCallbacks.Add(callback);
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x0004D05F File Offset: 0x0004B25F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPostTickCallback(ITickSystemPost callback)
	{
		TickSystem.postTickCallbacks.Add(callback);
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x0004D06C File Offset: 0x0004B26C
	public static void AddTickSystemCallBack(ITickSystem callback)
	{
		TickSystem.preTickCallbacks.Add(callback);
		TickSystem.tickCallbacks.Add(callback);
		TickSystem.postTickCallbacks.Add(callback);
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x0004D090 File Offset: 0x0004B290
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

	// Token: 0x06000D11 RID: 3345 RVA: 0x0004D0DE File Offset: 0x0004B2DE
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePreTickCallback(ITickSystemPre callback)
	{
		TickSystem.preTickCallbacks.Remove(callback);
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x0004D0EB File Offset: 0x0004B2EB
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveTickCallback(ITickSystemTick callback)
	{
		TickSystem.tickCallbacks.Remove(callback);
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x0004D0F8 File Offset: 0x0004B2F8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePostTickCallback(ITickSystemPost callback)
	{
		TickSystem.postTickCallbacks.Remove(callback);
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x0004D105 File Offset: 0x0004B305
	public static void RemoveTickSystemCallback(ITickSystem callback)
	{
		TickSystem.RemovePreTickCallback(callback);
		TickSystem.RemoveTickCallback(callback);
		TickSystem.RemovePostTickCallback(callback);
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x0004D11C File Offset: 0x0004B31C
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

	// Token: 0x04001034 RID: 4148
	private static CallbackContainer<ITickSystemPre> preTickCallbacks = new CallbackContainer<ITickSystemPre>();

	// Token: 0x04001035 RID: 4149
	private static CallbackContainer<ITickSystemTick> tickCallbacks = new CallbackContainer<ITickSystemTick>();

	// Token: 0x04001036 RID: 4150
	private static CallbackContainer<ITickSystemPost> postTickCallbacks = new CallbackContainer<ITickSystemPost>();
}
