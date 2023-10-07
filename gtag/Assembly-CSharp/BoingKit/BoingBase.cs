using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000361 RID: 865
	public class BoingBase : MonoBehaviour
	{
		// Token: 0x17000193 RID: 403
		// (get) Token: 0x0600192F RID: 6447 RVA: 0x0008B4DA File Offset: 0x000896DA
		public Version CurrentVersion
		{
			get
			{
				return this.m_currentVersion;
			}
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06001930 RID: 6448 RVA: 0x0008B4E2 File Offset: 0x000896E2
		public Version PreviousVersion
		{
			get
			{
				return this.m_previousVersion;
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06001931 RID: 6449 RVA: 0x0008B4EA File Offset: 0x000896EA
		public Version InitialVersion
		{
			get
			{
				return this.m_initialVersion;
			}
		}

		// Token: 0x06001932 RID: 6450 RVA: 0x0008B4F2 File Offset: 0x000896F2
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

		// Token: 0x040019F0 RID: 6640
		[SerializeField]
		private Version m_currentVersion;

		// Token: 0x040019F1 RID: 6641
		[SerializeField]
		private Version m_previousVersion;

		// Token: 0x040019F2 RID: 6642
		[SerializeField]
		private Version m_initialVersion = BoingKit.Version;
	}
}
