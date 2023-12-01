using System;
using UnityEngine;
using UnityEngine.UI;

public class GorillaLevelScreen : MonoBehaviour
{
	private void Awake()
	{
		this.startingText = this.myText.text;
	}

	public void UpdateText(string newText, bool setToGoodMaterial)
	{
		this.myText.text = newText;
		Material[] materials = base.GetComponent<MeshRenderer>().materials;
		materials[0] = (setToGoodMaterial ? this.goodMaterial : this.badMaterial);
		base.GetComponent<MeshRenderer>().materials = materials;
	}

	public string startingText;

	public Material goodMaterial;

	public Material badMaterial;

	public Text myText;
}
