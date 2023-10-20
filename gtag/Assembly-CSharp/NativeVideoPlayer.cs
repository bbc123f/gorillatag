﻿using System;
using UnityEngine;

// Token: 0x0200009A RID: 154
public static class NativeVideoPlayer
{
	// Token: 0x17000025 RID: 37
	// (get) Token: 0x06000345 RID: 837 RVA: 0x00013674 File Offset: 0x00011874
	private static IntPtr VideoPlayerClass
	{
		get
		{
			if (NativeVideoPlayer._VideoPlayerClass == null)
			{
				try
				{
					IntPtr intPtr = AndroidJNI.FindClass("com/oculus/videoplayer/NativeVideoPlayer");
					if (intPtr != IntPtr.Zero)
					{
						NativeVideoPlayer._VideoPlayerClass = new IntPtr?(AndroidJNI.NewGlobalRef(intPtr));
						AndroidJNI.DeleteLocalRef(intPtr);
					}
					else
					{
						Debug.LogError("Failed to find NativeVideoPlayer class");
						NativeVideoPlayer._VideoPlayerClass = new IntPtr?(IntPtr.Zero);
					}
				}
				catch (Exception exception)
				{
					Debug.LogError("Failed to find NativeVideoPlayer class");
					Debug.LogException(exception);
					NativeVideoPlayer._VideoPlayerClass = new IntPtr?(IntPtr.Zero);
				}
			}
			return NativeVideoPlayer._VideoPlayerClass.GetValueOrDefault();
		}
	}

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x06000346 RID: 838 RVA: 0x00013714 File Offset: 0x00011914
	private static IntPtr Activity
	{
		get
		{
			if (NativeVideoPlayer._Activity == null)
			{
				try
				{
					IntPtr intPtr = AndroidJNI.FindClass("com/unity3d/player/UnityPlayer");
					IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(intPtr, "currentActivity", "Landroid/app/Activity;");
					IntPtr staticObjectField = AndroidJNI.GetStaticObjectField(intPtr, staticFieldID);
					NativeVideoPlayer._Activity = new IntPtr?(AndroidJNI.NewGlobalRef(staticObjectField));
					AndroidJNI.DeleteLocalRef(staticObjectField);
					AndroidJNI.DeleteLocalRef(intPtr);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					NativeVideoPlayer._Activity = new IntPtr?(IntPtr.Zero);
				}
			}
			return NativeVideoPlayer._Activity.GetValueOrDefault();
		}
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x06000347 RID: 839 RVA: 0x000137A0 File Offset: 0x000119A0
	public static bool IsAvailable
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x06000348 RID: 840 RVA: 0x000137A3 File Offset: 0x000119A3
	public static bool IsPlaying
	{
		get
		{
			if (NativeVideoPlayer.getIsPlayingMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getIsPlayingMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getIsPlaying", "()Z");
			}
			return AndroidJNI.CallStaticBooleanMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getIsPlayingMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x06000349 RID: 841 RVA: 0x000137E3 File Offset: 0x000119E3
	public static NativeVideoPlayer.PlabackState CurrentPlaybackState
	{
		get
		{
			if (NativeVideoPlayer.getCurrentPlaybackStateMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getCurrentPlaybackStateMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getCurrentPlaybackState", "()I");
			}
			return (NativeVideoPlayer.PlabackState)AndroidJNI.CallStaticIntMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getCurrentPlaybackStateMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x0600034A RID: 842 RVA: 0x00013823 File Offset: 0x00011A23
	public static long Duration
	{
		get
		{
			if (NativeVideoPlayer.getDurationMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getDurationMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getDuration", "()J");
			}
			return AndroidJNI.CallStaticLongMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getDurationMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x0600034B RID: 843 RVA: 0x00013863 File Offset: 0x00011A63
	public static NativeVideoPlayer.StereoMode VideoStereoMode
	{
		get
		{
			if (NativeVideoPlayer.getStereoModeMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getStereoModeMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getStereoMode", "()I");
			}
			return (NativeVideoPlayer.StereoMode)AndroidJNI.CallStaticIntMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getStereoModeMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x0600034C RID: 844 RVA: 0x000138A3 File Offset: 0x00011AA3
	public static int VideoWidth
	{
		get
		{
			if (NativeVideoPlayer.getWidthMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getWidthMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getWidth", "()I");
			}
			return AndroidJNI.CallStaticIntMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getWidthMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x0600034D RID: 845 RVA: 0x000138E3 File Offset: 0x00011AE3
	public static int VideoHeight
	{
		get
		{
			if (NativeVideoPlayer.getHeightMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getHeightMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getHeight", "()I");
			}
			return AndroidJNI.CallStaticIntMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getHeightMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x0600034E RID: 846 RVA: 0x00013923 File Offset: 0x00011B23
	// (set) Token: 0x0600034F RID: 847 RVA: 0x00013964 File Offset: 0x00011B64
	public static long PlaybackPosition
	{
		get
		{
			if (NativeVideoPlayer.getPlaybackPositionMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getPlaybackPositionMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getPlaybackPosition", "()J");
			}
			return AndroidJNI.CallStaticLongMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getPlaybackPositionMethodId, NativeVideoPlayer.EmptyParams);
		}
		set
		{
			if (NativeVideoPlayer.setPlaybackPositionMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.setPlaybackPositionMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "setPlaybackPosition", "(J)V");
				NativeVideoPlayer.setPlaybackPositionParams = new jvalue[1];
			}
			NativeVideoPlayer.setPlaybackPositionParams[0].j = value;
			AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.setPlaybackPositionMethodId, NativeVideoPlayer.setPlaybackPositionParams);
		}
	}

	// Token: 0x06000350 RID: 848 RVA: 0x000139CC File Offset: 0x00011BCC
	public static void PlayVideo(string path, string drmLicenseUrl, IntPtr surfaceObj)
	{
		if (NativeVideoPlayer.playVideoMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.playVideoMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "playVideo", "(Landroid/content/Context;Ljava/lang/String;Ljava/lang/String;Landroid/view/Surface;)V");
			NativeVideoPlayer.playVideoParams = new jvalue[4];
		}
		IntPtr intPtr = AndroidJNI.NewStringUTF(path);
		IntPtr intPtr2 = AndroidJNI.NewStringUTF(drmLicenseUrl);
		NativeVideoPlayer.playVideoParams[0].l = NativeVideoPlayer.Activity;
		NativeVideoPlayer.playVideoParams[1].l = intPtr;
		NativeVideoPlayer.playVideoParams[2].l = intPtr2;
		NativeVideoPlayer.playVideoParams[3].l = surfaceObj;
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.playVideoMethodId, NativeVideoPlayer.playVideoParams);
		AndroidJNI.DeleteLocalRef(intPtr);
		AndroidJNI.DeleteLocalRef(intPtr2);
	}

	// Token: 0x06000351 RID: 849 RVA: 0x00013A84 File Offset: 0x00011C84
	public static void Stop()
	{
		if (NativeVideoPlayer.stopMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.stopMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "stop", "()V");
		}
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.stopMethodId, NativeVideoPlayer.EmptyParams);
	}

	// Token: 0x06000352 RID: 850 RVA: 0x00013AC4 File Offset: 0x00011CC4
	public static void Play()
	{
		if (NativeVideoPlayer.resumeMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.resumeMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "resume", "()V");
		}
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.resumeMethodId, NativeVideoPlayer.EmptyParams);
	}

	// Token: 0x06000353 RID: 851 RVA: 0x00013B04 File Offset: 0x00011D04
	public static void Pause()
	{
		if (NativeVideoPlayer.pauseMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.pauseMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "pause", "()V");
		}
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.pauseMethodId, NativeVideoPlayer.EmptyParams);
	}

	// Token: 0x06000354 RID: 852 RVA: 0x00013B44 File Offset: 0x00011D44
	public static void SetPlaybackSpeed(float speed)
	{
		if (NativeVideoPlayer.setPlaybackSpeedMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.setPlaybackSpeedMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "setPlaybackSpeed", "(F)V");
			NativeVideoPlayer.setPlaybackSpeedParams = new jvalue[1];
		}
		NativeVideoPlayer.setPlaybackSpeedParams[0].f = speed;
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.setPlaybackSpeedMethodId, NativeVideoPlayer.setPlaybackSpeedParams);
	}

	// Token: 0x06000355 RID: 853 RVA: 0x00013BAC File Offset: 0x00011DAC
	public static void SetLooping(bool looping)
	{
		if (NativeVideoPlayer.setLoopingMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.setLoopingMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "setLooping", "(Z)V");
			NativeVideoPlayer.setLoopingParams = new jvalue[1];
		}
		NativeVideoPlayer.setLoopingParams[0].z = looping;
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.setLoopingMethodId, NativeVideoPlayer.setLoopingParams);
	}

	// Token: 0x06000356 RID: 854 RVA: 0x00013C14 File Offset: 0x00011E14
	public static void SetListenerRotation(Quaternion rotation)
	{
		if (NativeVideoPlayer.setListenerRotationQuaternionMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.setListenerRotationQuaternionMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "setListenerRotationQuaternion", "(FFFF)V");
			NativeVideoPlayer.setListenerRotationQuaternionParams = new jvalue[4];
		}
		NativeVideoPlayer.setListenerRotationQuaternionParams[0].f = rotation.x;
		NativeVideoPlayer.setListenerRotationQuaternionParams[1].f = rotation.y;
		NativeVideoPlayer.setListenerRotationQuaternionParams[2].f = rotation.z;
		NativeVideoPlayer.setListenerRotationQuaternionParams[3].f = rotation.w;
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.setListenerRotationQuaternionMethodId, NativeVideoPlayer.setListenerRotationQuaternionParams);
	}

	// Token: 0x040003E9 RID: 1001
	private static IntPtr? _Activity;

	// Token: 0x040003EA RID: 1002
	private static IntPtr? _VideoPlayerClass;

	// Token: 0x040003EB RID: 1003
	private static readonly jvalue[] EmptyParams = new jvalue[0];

	// Token: 0x040003EC RID: 1004
	private static IntPtr getIsPlayingMethodId;

	// Token: 0x040003ED RID: 1005
	private static IntPtr getCurrentPlaybackStateMethodId;

	// Token: 0x040003EE RID: 1006
	private static IntPtr getDurationMethodId;

	// Token: 0x040003EF RID: 1007
	private static IntPtr getStereoModeMethodId;

	// Token: 0x040003F0 RID: 1008
	private static IntPtr getWidthMethodId;

	// Token: 0x040003F1 RID: 1009
	private static IntPtr getHeightMethodId;

	// Token: 0x040003F2 RID: 1010
	private static IntPtr getPlaybackPositionMethodId;

	// Token: 0x040003F3 RID: 1011
	private static IntPtr setPlaybackPositionMethodId;

	// Token: 0x040003F4 RID: 1012
	private static jvalue[] setPlaybackPositionParams;

	// Token: 0x040003F5 RID: 1013
	private static IntPtr playVideoMethodId;

	// Token: 0x040003F6 RID: 1014
	private static jvalue[] playVideoParams;

	// Token: 0x040003F7 RID: 1015
	private static IntPtr stopMethodId;

	// Token: 0x040003F8 RID: 1016
	private static IntPtr resumeMethodId;

	// Token: 0x040003F9 RID: 1017
	private static IntPtr pauseMethodId;

	// Token: 0x040003FA RID: 1018
	private static IntPtr setPlaybackSpeedMethodId;

	// Token: 0x040003FB RID: 1019
	private static jvalue[] setPlaybackSpeedParams;

	// Token: 0x040003FC RID: 1020
	private static IntPtr setLoopingMethodId;

	// Token: 0x040003FD RID: 1021
	private static jvalue[] setLoopingParams;

	// Token: 0x040003FE RID: 1022
	private static IntPtr setListenerRotationQuaternionMethodId;

	// Token: 0x040003FF RID: 1023
	private static jvalue[] setListenerRotationQuaternionParams;

	// Token: 0x020003C4 RID: 964
	public enum PlabackState
	{
		// Token: 0x04001BD3 RID: 7123
		Idle = 1,
		// Token: 0x04001BD4 RID: 7124
		Preparing,
		// Token: 0x04001BD5 RID: 7125
		Buffering,
		// Token: 0x04001BD6 RID: 7126
		Ready,
		// Token: 0x04001BD7 RID: 7127
		Ended
	}

	// Token: 0x020003C5 RID: 965
	public enum StereoMode
	{
		// Token: 0x04001BD9 RID: 7129
		Unknown = -1,
		// Token: 0x04001BDA RID: 7130
		Mono,
		// Token: 0x04001BDB RID: 7131
		TopBottom,
		// Token: 0x04001BDC RID: 7132
		LeftRight,
		// Token: 0x04001BDD RID: 7133
		Mesh
	}
}