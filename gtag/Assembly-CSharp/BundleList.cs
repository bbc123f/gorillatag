using System;
using GorillaNetworking;

// Token: 0x02000101 RID: 257
internal class BundleList
{
	// Token: 0x0600063C RID: 1596 RVA: 0x00027298 File Offset: 0x00025498
	public void FromJson(string jsonString)
	{
		this.data = JSonHelper.FromJson<BundleData>(jsonString);
		if (this.data.Length == 0)
		{
			return;
		}
		this.activeBundleIdx = 0;
		int majorVersion = this.data[0].majorVersion;
		int minorVersion = this.data[0].minorVersion;
		int minorVersion2 = this.data[0].minorVersion2;
		int gameMajorVersion = PhotonNetworkController.Instance.GameMajorVersion;
		int gameMinorVersion = PhotonNetworkController.Instance.GameMinorVersion;
		int gameMinorVersion2 = PhotonNetworkController.Instance.GameMinorVersion2;
		for (int i = 1; i < this.data.Length; i++)
		{
			this.data[i].isActive = false;
			int num = gameMajorVersion * 1000000 + gameMinorVersion * 1000 + gameMinorVersion2;
			int num2 = this.data[i].majorVersion * 1000000 + this.data[i].minorVersion * 1000 + this.data[i].minorVersion2;
			if (num >= num2 && this.data[i].majorVersion >= majorVersion && this.data[i].minorVersion >= minorVersion && this.data[i].minorVersion2 >= minorVersion2)
			{
				this.activeBundleIdx = i;
				majorVersion = this.data[i].majorVersion;
				minorVersion = this.data[i].minorVersion;
				minorVersion2 = this.data[i].minorVersion2;
				break;
			}
		}
		this.data[this.activeBundleIdx].isActive = true;
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x0002744C File Offset: 0x0002564C
	public bool HasSku(string skuName, out int idx)
	{
		for (int i = 0; i < this.data.Length; i++)
		{
			if (this.data[i].skuName == skuName)
			{
				idx = i;
				return true;
			}
		}
		idx = -1;
		return false;
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x0002748E File Offset: 0x0002568E
	public BundleData ActiveBundle()
	{
		return this.data[this.activeBundleIdx];
	}

	// Token: 0x04000796 RID: 1942
	private int activeBundleIdx;

	// Token: 0x04000797 RID: 1943
	public BundleData[] data;
}
