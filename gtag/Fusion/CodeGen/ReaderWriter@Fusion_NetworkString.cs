using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	internal struct ReaderWriter@Fusion_NetworkString : IElementReaderWriter<NetworkString<_16>>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe NetworkString<_16> Read(byte* data, int index)
		{
			return *(NetworkString<_16>*)(data + index * 68);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref NetworkString<_16> ReadRef(byte* data, int index)
		{
			return ref *(NetworkString<_16>*)(data + index * 68);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, NetworkString<_16> val)
		{
			*(NetworkString<_16>*)(data + index * 68) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 17;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<NetworkString<_16>> GetInstance()
		{
			if (ReaderWriter@Fusion_NetworkString.Instance == null)
			{
				ReaderWriter@Fusion_NetworkString.Instance = default(ReaderWriter@Fusion_NetworkString);
			}
			return ReaderWriter@Fusion_NetworkString.Instance;
		}

		public static IElementReaderWriter<NetworkString<_16>> Instance;
	}
}
