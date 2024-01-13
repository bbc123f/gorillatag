using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

public class GorillaComputerTerminal : MonoBehaviour
{
	public Text myScreenText;

	public Text myFunctionText;

	public MeshRenderer monitorMesh;

	private void LateUpdate()
	{
		myScreenText.text = GorillaComputer.instance.screenText.Text;
		myFunctionText.text = GorillaComputer.instance.functionSelectText.Text;
		monitorMesh.materials = GorillaComputer.instance.computerScreenRenderer.materials;
	}
}
