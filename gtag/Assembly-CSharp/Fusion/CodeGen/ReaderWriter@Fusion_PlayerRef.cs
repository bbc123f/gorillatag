using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	internal struct ReaderWriter@Fusion_PlayerRef : IElementReaderWriter<PlayerRef>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe PlayerRef Read(byte* data, int index)
		{
			return *(PlayerRef*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref PlayerRef ReadRef(byte* data, int index)
		{
			return ref *(PlayerRef*)(data + index * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, PlayerRef val)
		{
			*(PlayerRef*)(data + index * 4) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<PlayerRef> GetInstance()
		{
			if (ReaderWriter@Fusion_PlayerRef.Instance == null)
			{
				ReaderWriter@Fusion_PlayerRef.Instance = default(ReaderWriter@Fusion_PlayerRef);
			}
			return ReaderWriter@Fusion_PlayerRef.Instance;
		}

		public static IElementReaderWriter<PlayerRef> Instance;
	}
}
