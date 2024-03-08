using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen
{
	[NetworkStructWeaved(17)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	internal struct FixedStorage@17 : INetworkStruct
	{
		[FixedBuffer(typeof(int), 17)]
		[FieldOffset(0)]
		public FixedStorage@17.<Data>e__FixedBuffer Data;

		[NonSerialized]
		[FieldOffset(4)]
		private int _1;

		[NonSerialized]
		[FieldOffset(8)]
		private int _2;

		[NonSerialized]
		[FieldOffset(12)]
		private int _3;

		[NonSerialized]
		[FieldOffset(16)]
		private int _4;

		[NonSerialized]
		[FieldOffset(20)]
		private int _5;

		[NonSerialized]
		[FieldOffset(24)]
		private int _6;

		[NonSerialized]
		[FieldOffset(28)]
		private int _7;

		[NonSerialized]
		[FieldOffset(32)]
		private int _8;

		[NonSerialized]
		[FieldOffset(36)]
		private int _9;

		[NonSerialized]
		[FieldOffset(40)]
		private int _10;

		[NonSerialized]
		[FieldOffset(44)]
		private int _11;

		[NonSerialized]
		[FieldOffset(48)]
		private int _12;

		[NonSerialized]
		[FieldOffset(52)]
		private int _13;

		[NonSerialized]
		[FieldOffset(56)]
		private int _14;

		[NonSerialized]
		[FieldOffset(60)]
		private int _15;

		[NonSerialized]
		[FieldOffset(64)]
		private int _16;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 68)]
		public struct <Data>e__FixedBuffer
		{
			private int FixedElementField;
		}
	}
}
