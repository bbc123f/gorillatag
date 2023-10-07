using System;
using UnityEngine;

// Token: 0x020000F2 RID: 242
public class PuppetFollow : MonoBehaviour
{
	// Token: 0x060005A8 RID: 1448 RVA: 0x000234F0 File Offset: 0x000216F0
	private void FixedUpdate()
	{
		base.transform.position = this.sourceTarget.position - this.sourceBase.position + this.puppetBase.position;
		base.transform.localRotation = this.sourceTarget.localRotation;
	}

	// Token: 0x0400069A RID: 1690
	public Transform sourceTarget;

	// Token: 0x0400069B RID: 1691
	public Transform sourceBase;

	// Token: 0x0400069C RID: 1692
	public Transform puppetBase;
}
