using System;
using System.Runtime.InteropServices;
using Fusion;
using UnityEngine;

[NetworkStructWeaved(34)]
[StructLayout(LayoutKind.Explicit, Size = 136)]
public struct InputStruct : INetworkStruct
{
	[FieldOffset(0)]
	public Quaternion headRotation;

	[FieldOffset(16)]
	public Vector3 rightHandPosition;

	[FieldOffset(28)]
	public Quaternion rightHandRotation;

	[FieldOffset(44)]
	public Vector3 leftHandPosition;

	[FieldOffset(56)]
	public Quaternion leftHandRotation;

	[FieldOffset(72)]
	public Vector3 position;

	[FieldOffset(84)]
	public int roundedRotation;

	[FieldOffset(88)]
	public int handPosition;

	[FieldOffset(92)]
	public TransferrableObject.PositionState state;

	[FieldOffset(96)]
	public int grabbedRopeIndex;

	[FieldOffset(100)]
	public int ropeBoneIndex;

	[FieldOffset(104)]
	public bool ropeGrabIsLeft;

	[FieldOffset(108)]
	public Vector3 ropeGrabOffset;

	[FieldOffset(120)]
	public double serverTimeStamp;

	[FieldOffset(128)]
	public bool remoteUseReplacementVoice;

	[FieldOffset(132)]
	public float speakingLoudness;
}
