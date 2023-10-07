using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020001FD RID: 509
internal class TickSystem : MonoBehaviour
{
	// Token: 0x06000CFF RID: 3327 RVA: 0x0004CD36 File Offset: 0x0004AF36
	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	// Token: 0x06000D00 RID: 3328 RVA: 0x0004CD3E File Offset: 0x0004AF3E
	private void Update()
	{
		TickSystem.preTickCallbacks.TryRunCallbacks();
		TickSystem.tickCallbacks.TryRunCallbacks();
	}

	// Token: 0x06000D01 RID: 3329 RVA: 0x0004CD54 File Offset: 0x0004AF54
	private void LateUpdate()
	{
		TickSystem.postTickCallbacks.TryRunCallbacks();
	}

	// Token: 0x06000D02 RID: 3330 RVA: 0x0004CD60 File Offset: 0x0004AF60
	static TickSystem()
	{
		TickSystem.preTickCallbacks.SetRunUpdateCallback(new CallbackContainer<ITickSystemPre>.runCallbacksDelegate(TickSystem.CallPreTick));
		TickSystem.tickCallbacks.SetRunUpdateCallback(new CallbackContainer<ITickSystemTick>.runCallbacksDelegate(TickSystem.CallTick));
		TickSystem.postTickCallbacks.SetRunUpdateCallback(new CallbackContainer<ITickSystemPost>.runCallbacksDelegate(TickSystem.CallPostTick));
	}

	// Token: 0x06000D03 RID: 3331 RVA: 0x0004CDCD File Offset: 0x0004AFCD
	private static void CallPreTick(ITickSystemPre callback)
	{
		callback.PreTick();
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x0004CDD5 File Offset: 0x0004AFD5
	private static void CallTick(ITickSystemTick callback)
	{
		callback.Tick();
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x0004CDDD File Offset: 0x0004AFDD
	private static void CallPostTick(ITickSystemPost callback)
	{
		callback.PostTick();
	}

	// Token: 0x06000D06 RID: 3334 RVA: 0x0004CDE5 File Offset: 0x0004AFE5
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPreTickCallback(ITickSystemPre callback)
	{
		TickSystem.preTickCallbacks.Add(callback);
	}

	// Token: 0x06000D07 RID: 3335 RVA: 0x0004CDF2 File Offset: 0x0004AFF2
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddTickCallback(ITickSystemTick callback)
	{
		TickSystem.tickCallbacks.Add(callback);
	}

	// Token: 0x06000D08 RID: 3336 RVA: 0x0004CDFF File Offset: 0x0004AFFF
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPostTickCallback(ITickSystemPost callback)
	{
		TickSystem.postTickCallbacks.Add(callback);
	}

	// Token: 0x06000D09 RID: 3337 RVA: 0x0004CE0C File Offset: 0x0004B00C
	public static void AddTickSystemCallBack(ITickSystem callback)
	{
		TickSystem.preTickCallbacks.Add(callback);
		TickSystem.tickCallbacks.Add(callback);
		TickSystem.postTickCallbacks.Add(callback);
	}

	// Token: 0x06000D0A RID: 3338 RVA: 0x0004CE30 File Offset: 0x0004B030
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

	// Token: 0x06000D0B RID: 3339 RVA: 0x0004CE7E File Offset: 0x0004B07E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePreTickCallback(ITickSystemPre callback)
	{
		TickSystem.preTickCallbacks.Remove(callback);
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x0004CE8B File Offset: 0x0004B08B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveTickCallback(ITickSystemTick callback)
	{
		TickSystem.tickCallbacks.Remove(callback);
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x0004CE98 File Offset: 0x0004B098
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePostTickCallback(ITickSystemPost callback)
	{
		TickSystem.postTickCallbacks.Remove(callback);
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x0004CEA5 File Offset: 0x0004B0A5
	public static void RemoveTickSystemCallback(ITickSystem callback)
	{
		TickSystem.RemovePreTickCallback(callback);
		TickSystem.RemoveTickCallback(callback);
		TickSystem.RemovePostTickCallback(callback);
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x0004CEBC File Offset: 0x0004B0BC
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

	// Token: 0x0400102F RID: 4143
	private static CallbackContainer<ITickSystemPre> preTickCallbacks = new CallbackContainer<ITickSystemPre>();

	// Token: 0x04001030 RID: 4144
	private static CallbackContainer<ITickSystemTick> tickCallbacks = new CallbackContainer<ITickSystemTick>();

	// Token: 0x04001031 RID: 4145
	private static CallbackContainer<ITickSystemPost> postTickCallbacks = new CallbackContainer<ITickSystemPost>();
}
