using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000363 RID: 867
	public class BoingBase : MonoBehaviour
	{
		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06001938 RID: 6456 RVA: 0x0008B9C2 File Offset: 0x00089BC2
		public Version CurrentVersion
		{
			get
			{
				return this.m_currentVersion;
			}
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06001939 RID: 6457 RVA: 0x0008B9CA File Offset: 0x00089BCA
		public Version PreviousVersion
		{
			get
			{
				return this.m_previousVersion;
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x0600193A RID: 6458 RVA: 0x0008B9D2 File Offset: 0x00089BD2
		public Version InitialVersion
		{
			get
			{
				return this.m_initialVersion;
			}
		}

		// Token: 0x0600193B RID: 6459 RVA: 0x0008B9DA File Offset: 0x00089BDA
		protected virtual void OnUpgrade(Version oldVersion, Version newVersion)
		{
			this.m_previousVersion = this.m_currentVersion;
			if (this.m_currentVersion.Revision < 33)
			{
				this.m_initialVersion = Version.Invalid;
				this.m_previousVersion = Version.Invalid;
			}
			this.m_currentVersion = newVersion;
		}

		// Token: 0x040019FD RID: 6653
		[SerializeField]
		private Version m_currentVersion;

		// Token: 0x040019FE RID: 6654
		[SerializeField]
		private Version m_previousVersion;

		// Token: 0x040019FF RID: 6655
		[SerializeField]
		private Version m_initialVersion = BoingKit.Version;
	}
}
