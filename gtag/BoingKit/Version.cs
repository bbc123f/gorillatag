using System;
using System.Runtime.CompilerServices;

namespace BoingKit
{
	public struct Version : IEquatable<Version>
	{
		public readonly int MajorVersion
		{
			[CompilerGenerated]
			get
			{
				return this.<MajorVersion>k__BackingField;
			}
		}

		public readonly int MinorVersion
		{
			[CompilerGenerated]
			get
			{
				return this.<MinorVersion>k__BackingField;
			}
		}

		public readonly int Revision
		{
			[CompilerGenerated]
			get
			{
				return this.<Revision>k__BackingField;
			}
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.MajorVersion.ToString(),
				".",
				this.MinorVersion.ToString(),
				".",
				this.Revision.ToString()
			});
		}

		public bool IsValid()
		{
			return this.MajorVersion >= 0 && this.MinorVersion >= 0 && this.Revision >= 0;
		}

		public Version(int majorVersion = -1, int minorVersion = -1, int revision = -1)
		{
			this.MajorVersion = majorVersion;
			this.MinorVersion = minorVersion;
			this.Revision = revision;
		}

		public static bool operator ==(Version lhs, Version rhs)
		{
			return lhs.IsValid() && rhs.IsValid() && (lhs.MajorVersion == rhs.MajorVersion && lhs.MinorVersion == rhs.MinorVersion) && lhs.Revision == rhs.Revision;
		}

		public static bool operator !=(Version lhs, Version rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object obj)
		{
			return obj is Version && this.Equals((Version)obj);
		}

		public bool Equals(Version other)
		{
			return this.MajorVersion == other.MajorVersion && this.MinorVersion == other.MinorVersion && this.Revision == other.Revision;
		}

		public override int GetHashCode()
		{
			return ((366299368 * -1521134295 + this.MajorVersion.GetHashCode()) * -1521134295 + this.MinorVersion.GetHashCode()) * -1521134295 + this.Revision.GetHashCode();
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Version()
		{
		}

		public static readonly Version Invalid = new Version(-1, -1, -1);

		public static readonly Version FirstTracked = new Version(1, 2, 33);

		public static readonly Version LastUntracked = new Version(1, 2, 32);

		[CompilerGenerated]
		private readonly int <MajorVersion>k__BackingField;

		[CompilerGenerated]
		private readonly int <MinorVersion>k__BackingField;

		[CompilerGenerated]
		private readonly int <Revision>k__BackingField;
	}
}
