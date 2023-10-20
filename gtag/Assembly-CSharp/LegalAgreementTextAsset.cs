using System;
using UnityEngine;

// Token: 0x020001DA RID: 474
[CreateAssetMenu(fileName = "NewLegalAgreementAsset", menuName = "Gorilla Tag/Legal Agreement Asset")]
public class LegalAgreementTextAsset : ScriptableObject
{
	// Token: 0x04000FA3 RID: 4003
	public string title;

	// Token: 0x04000FA4 RID: 4004
	public string playFabKey;

	// Token: 0x04000FA5 RID: 4005
	public string latestVersionKey;

	// Token: 0x04000FA6 RID: 4006
	[TextArea(3, 5)]
	public string errorMessage;
}
