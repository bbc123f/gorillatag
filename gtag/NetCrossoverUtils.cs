using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Pun;
using UnityEngine;

public static class NetCrossoverUtils
{
	public static void Prewarm()
	{
		NetCrossoverUtils.FixedBuffer = new byte[2048];
	}

	public static void WriteNetDataToBuffer<T>(this T data, PhotonStream stream) where T : struct, INetworkStruct
	{
		if (stream.IsReading)
		{
			Debug.LogError("Attempted to write data to a reading stream!");
			return;
		}
		IntPtr intPtr = 0;
		try
		{
			int num = Marshal.SizeOf(typeof(T));
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr<T>(data, intPtr, true);
			Marshal.Copy(intPtr, NetCrossoverUtils.FixedBuffer, 0, num);
			stream.SendNext(num);
			for (int i = 0; i < num; i++)
			{
				stream.SendNext(NetCrossoverUtils.FixedBuffer[i]);
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static T ReadNetDataFromBuffer<T>(PhotonStream stream) where T : struct, INetworkStruct
	{
		T t;
		if (stream.IsWriting)
		{
			Debug.LogError("Attmpted to read data from a writing stream!");
			t = default(T);
			return t;
		}
		IntPtr intPtr = 0;
		try
		{
			Type typeFromHandle = typeof(T);
			int num = (int)stream.ReceiveNext();
			int num2 = Marshal.SizeOf(typeFromHandle);
			if (num != num2)
			{
				Debug.LogError(string.Format("Expected datasize {0} when reading data for type '{1}',", num2, typeFromHandle.Name) + string.Format("but {0} data is available!", num));
				t = default(T);
				t = t;
			}
			else
			{
				intPtr = Marshal.AllocHGlobal(num2);
				for (int i = 0; i < num2; i++)
				{
					NetCrossoverUtils.FixedBuffer[i] = (byte)stream.ReceiveNext();
				}
				Marshal.Copy(NetCrossoverUtils.FixedBuffer, 0, intPtr, num2);
				t = (T)((object)Marshal.PtrToStructure(intPtr, typeFromHandle));
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
		return t;
	}

	public static void SerializeToRPCData<T>(this RPCArgBuffer<T> argBuffer) where T : struct
	{
		IntPtr intPtr = 0;
		try
		{
			int num = Marshal.SizeOf(typeof(T));
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr<T>(argBuffer.Args, intPtr, true);
			Marshal.Copy(intPtr, argBuffer.Data, 0, num);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static void PopulateWithRPCData<T>(this RPCArgBuffer<T> argBuffer, byte[] data) where T : struct
	{
		IntPtr intPtr = 0;
		try
		{
			int num = Marshal.SizeOf(typeof(T));
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.Copy(data, 0, intPtr, num);
			argBuffer.Args = Marshal.PtrToStructure<T>(intPtr);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static Dictionary<string, SessionProperty> ToPropDict(this Hashtable hash)
	{
		Dictionary<string, SessionProperty> dictionary = new Dictionary<string, SessionProperty>();
		foreach (DictionaryEntry dictionaryEntry in hash)
		{
			dictionary.Add((string)dictionaryEntry.Key, (string)dictionaryEntry.Value);
		}
		return dictionary;
	}

	private const int MaxParameterByteLength = 2048;

	private static byte[] FixedBuffer;
}
