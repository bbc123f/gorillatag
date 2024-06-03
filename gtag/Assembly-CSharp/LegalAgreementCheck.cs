using System;
using UnityEngine;

public class LegalAgreementCheck : MonoBehaviour
{
	public LegalAgreementCheck()
	{
	}

	[SerializeField]
	private LegalAgreementTextAsset[] agreements;

	[SerializeField]
	private bool testAgreement;

	[SerializeField]
	private LegalAgreements legalAgreements;
}
