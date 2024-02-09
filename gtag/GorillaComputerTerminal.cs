using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

public class GorillaComputerTerminal : MonoBehaviour
{
	private void LateUpdate()
	{
		this.myScreenText.text = GorillaComputer.instance.screenText.Text;
		this.myFunctionText.text = GorillaComputer.instance.functionSelectText.Text;
		this.monitorMesh.materials = GorillaComputer.instance.computerScreenRenderer.materials;
	}

	public Text myScreenText;

	public Text myFunctionText;

	public MeshRenderer monitorMesh;
}
