using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GorillaNetworking
{
	[Serializable]
	internal class CreditsSection
	{
		public string Title
		{
			[CompilerGenerated]
			get
			{
				return this.<Title>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Title>k__BackingField = value;
			}
		}

		public List<string> Entries
		{
			[CompilerGenerated]
			get
			{
				return this.<Entries>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Entries>k__BackingField = value;
			}
		}

		public CreditsSection()
		{
		}

		[CompilerGenerated]
		private string <Title>k__BackingField;

		[CompilerGenerated]
		private List<string> <Entries>k__BackingField;
	}
}
