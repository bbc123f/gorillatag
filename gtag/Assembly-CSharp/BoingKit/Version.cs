using System;

namespace BoingKit
{
	// Token: 0x02000369 RID: 873
	public struct Version : IEquatable<Version>
	{
		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06001970 RID: 6512 RVA: 0x0008D594 File Offset: 0x0008B794
		public readonly int MajorVersion { get; }

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06001971 RID: 6513 RVA: 0x0008D59C File Offset: 0x0008B79C
		public readonly int MinorVersion { get; }

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06001972 RID: 6514 RVA: 0x0008D5A4 File Offset: 0x0008B7A4
		public readonly int Revision { get; }

		// Token: 0x06001973 RID: 6515 RVA: 0x0008D5AC File Offset: 0x0008B7AC
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

		// Token: 0x06001974 RID: 6516 RVA: 0x0008D607 File Offset: 0x0008B807
		public bool IsValid()
		{
			return this.MajorVersion >= 0 && this.MinorVersion >= 0 && this.Revision >= 0;
		}

		// Token: 0x06001975 RID: 6517 RVA: 0x0008D629 File Offset: 0x0008B829
		public Version(int majorVersion = -1, int minorVersion = -1, int revision = -1)
		{
			this.MajorVersion = majorVersion;
			this.MinorVersion = minorVersion;
			this.Revision = revision;
		}

		// Token: 0x06001976 RID: 6518 RVA: 0x0008D640 File Offset: 0x0008B840
		public static bool operator ==(Version lhs, Version rhs)
		{
			return lhs.IsValid() && rhs.IsValid() && (lhs.MajorVersion == rhs.MajorVersion && lhs.MinorVersion == rhs.MinorVersion) && lhs.Revision == rhs.Revision;
		}

		// Token: 0x06001977 RID: 6519 RVA: 0x0008D695 File Offset: 0x0008B895
		public static bool operator !=(Version lhs, Version rhs)
		{
			return !(lhs == rhs);
		}

		// Token: 0x06001978 RID: 6520 RVA: 0x0008D6A1 File Offset: 0x0008B8A1
		public override bool Equals(object obj)
		{
			return obj is Version && this.Equals((Version)obj);
		}

		// Token: 0x06001979 RID: 6521 RVA: 0x0008D6B9 File Offset: 0x0008B8B9
		public bool Equals(Version other)
		{
			return this.MajorVersion == other.MajorVersion && this.MinorVersion == other.MinorVersion && this.Revision == other.Revision;
		}

		// Token: 0x0600197A RID: 6522 RVA: 0x0008D6EC File Offset: 0x0008B8EC
		public override int GetHashCode()
		{
			return ((366299368 * -1521134295 + this.MajorVersion.GetHashCode()) * -1521134295 + this.MinorVersion.GetHashCode()) * -1521134295 + this.Revision.GetHashCode();
		}

		// Token: 0x04001A3B RID: 6715
		public static readonly Version Invalid = new Version(-1, -1, -1);

		// Token: 0x04001A3C RID: 6716
		public static readonly Version FirstTracked = new Version(1, 2, 33);

		// Token: 0x04001A3D RID: 6717
		public static readonly Version LastUntracked = new Version(1, 2, 32);
	}
}
