using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	internal struct ReaderWriter@FusionPlayerProperties__PlayerInfo : IElementReaderWriter<FusionPlayerProperties.PlayerInfo>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe FusionPlayerProperties.PlayerInfo Read(byte* data, int index)
		{
			return *(FusionPlayerProperties.PlayerInfo*)(data + index * 896);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref FusionPlayerProperties.PlayerInfo ReadRef(byte* data, int index)
		{
			return ref *(FusionPlayerProperties.PlayerInfo*)(data + index * 896);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Write(byte* data, int index, FusionPlayerProperties.PlayerInfo val)
		{
			*(FusionPlayerProperties.PlayerInfo*)(data + index * 896) = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetElementWordCount()
		{
			return 224;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IElementReaderWriter<FusionPlayerProperties.PlayerInfo> GetInstance()
		{
			if (ReaderWriter@FusionPlayerProperties__PlayerInfo.Instance == null)
			{
				ReaderWriter@FusionPlayerProperties__PlayerInfo.Instance = default(ReaderWriter@FusionPlayerProperties__PlayerInfo);
			}
			return ReaderWriter@FusionPlayerProperties__PlayerInfo.Instance;
		}

		public static IElementReaderWriter<FusionPlayerProperties.PlayerInfo> Instance;
	}
}
