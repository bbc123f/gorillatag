using System;
using System.Runtime.CompilerServices;

namespace Viveport
{
	public class Leaderboard
	{
		public int Rank
		{
			[CompilerGenerated]
			get
			{
				return this.<Rank>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Rank>k__BackingField = value;
			}
		}

		public int Score
		{
			[CompilerGenerated]
			get
			{
				return this.<Score>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Score>k__BackingField = value;
			}
		}

		public string UserName
		{
			[CompilerGenerated]
			get
			{
				return this.<UserName>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<UserName>k__BackingField = value;
			}
		}

		public Leaderboard()
		{
		}

		[CompilerGenerated]
		private int <Rank>k__BackingField;

		[CompilerGenerated]
		private int <Score>k__BackingField;

		[CompilerGenerated]
		private string <UserName>k__BackingField;
	}
}
