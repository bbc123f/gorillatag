using System;
using UnityEngine;

// Token: 0x020001D9 RID: 473
[CreateAssetMenu(fileName = "NewLegalAgreementAsset", menuName = "Gorilla Tag/Legal Agreement Asset")]
public class LegalAgreementTextAsset : ScriptableObject
{
	// Token: 0x04000F9F RID: 3999
	public string title;

	// Token: 0x04000FA0 RID: 4000
	public string playFabKey;

	// Token: 0x04000FA1 RID: 4001
	public string latestVersionKey;

	// Token: 0x04000FA2 RID: 4002
	[TextArea(3, 5)]
	public string errorMessage;
}
