using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class SpatialUtils
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Grid3DToFlatIndex(int x, int y, int z, int xMax, int yMax)
	{
		return z * xMax * yMax + y * xMax + x;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Grid3DToFlatIndex(Vector3Int g3d, int xMax, int yMax)
	{
		return g3d.z * xMax * yMax + g3d.y * xMax + g3d.x;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Int FlatIndexToGrid3D(int idx, int xMax, int yMax)
	{
		int num = idx / (xMax * yMax);
		idx -= num * xMax * yMax;
		int num2 = idx / xMax;
		return new Vector3Int(idx % xMax, num2, num);
	}
}
