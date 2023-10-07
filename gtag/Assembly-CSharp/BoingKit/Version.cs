using System;

namespace BoingKit
{
	// Token: 0x02000367 RID: 871
	public struct Version : IEquatable<Version>
	{
		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06001967 RID: 6503 RVA: 0x0008D0AC File Offset: 0x0008B2AC
		public readonly int MajorVersion { get; }

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06001968 RID: 6504 RVA: 0x0008D0B4 File Offset: 0x0008B2B4
		public readonly int MinorVersion { get; }

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06001969 RID: 6505 RVA: 0x0008D0BC File Offset: 0x0008B2BC
		public readonly int Revision { get; }

		// Token: 0x0600196A RID: 6506 RVA: 0x0008D0C4 File Offset: 0x0008B2C4
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

		// Token: 0x0600196B RID: 6507 RVA: 0x0008D11F File Offset: 0x0008B31F
		public bool IsValid()
		{
			return this.MajorVersion >= 0 && this.MinorVersion >= 0 && this.Revision >= 0;
		}

		// Token: 0x0600196C RID: 6508 RVA: 0x0008D141 File Offset: 0x0008B341
		public Version(int majorVersion = -1, int minorVersion = -1, int revision = -1)
		{
			this.MajorVersion = majorVersion;
			this.MinorVersion = minorVersion;
			this.Revision = revision;
		}

		// Token: 0x0600196D RID: 6509 RVA: 0x0008D158 File Offset: 0x0008B358
		public static bool operator ==(Version lhs, Version rhs)
		{
			return lhs.IsValid() && rhs.IsValid() && (lhs.MajorVersion == rhs.MajorVersion && lhs.MinorVersion == rhs.MinorVersion) && lhs.Revision == rhs.Revision;
		}

		// Token: 0x0600196E RID: 6510 RVA: 0x0008D1AD File Offset: 0x0008B3AD
		public static bool operator !=(Version lhs, Version rhs)
		{
			return !(lhs == rhs);
		}

		// Token: 0x0600196F RID: 6511 RVA: 0x0008D1B9 File Offset: 0x0008B3B9
		public override bool Equals(object obj)
		{
			return obj is Version && this.Equals((Version)obj);
		}

		// Token: 0x06001970 RID: 6512 RVA: 0x0008D1D1 File Offset: 0x0008B3D1
		public bool Equals(Version other)
		{
			return this.MajorVersion == other.MajorVersion && this.MinorVersion == other.MinorVersion && this.Revision == other.Revision;
		}

		// Token: 0x06001971 RID: 6513 RVA: 0x0008D204 File Offset: 0x0008B404
		public override int GetHashCode()
		{
			return ((366299368 * -1521134295 + this.MajorVersion.GetHashCode()) * -1521134295 + this.MinorVersion.GetHashCode()) * -1521134295 + this.Revision.GetHashCode();
		}

		// Token: 0x04001A2E RID: 6702
		public static readonly Version Invalid = new Version(-1, -1, -1);

		// Token: 0x04001A2F RID: 6703
		public static readonly Version FirstTracked = new Version(1, 2, 33);

		// Token: 0x04001A30 RID: 6704
		public static readonly Version LastUntracked = new Version(1, 2, 32);
	}
}
