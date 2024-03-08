using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GorillaComputerTerminal : MonoBehaviour
{
	private void OnEnable()
	{
		if (GorillaComputer.instance == null)
		{
			base.StartCoroutine(this.<OnEnable>g__OnEnable_Local|3_0());
			return;
		}
		this.Init();
	}

	private void Init()
	{
		GameEvents.ScreenTextChangedEvent.AddListener(new UnityAction<string>(this.OnScreenTextChanged));
		GameEvents.FunctionSelectTextChangedEvent.AddListener(new UnityAction<string>(this.OnFunctionTextChanged));
		GameEvents.ScreenTextMaterialsEvent.AddListener(new UnityAction<Material[]>(this.OnMaterialsChanged));
		this.myScreenText.text = GorillaComputer.instance.screenText.Text;
		this.myFunctionText.text = GorillaComputer.instance.functionSelectText.Text;
		if (GorillaComputer.instance.screenText.currentMaterials != null)
		{
			this.monitorMesh.materials = GorillaComputer.instance.screenText.currentMaterials;
		}
	}

	private void OnDisable()
	{
		GameEvents.ScreenTextChangedEvent.RemoveListener(new UnityAction<string>(this.OnScreenTextChanged));
		GameEvents.FunctionSelectTextChangedEvent.RemoveListener(new UnityAction<string>(this.OnFunctionTextChanged));
		GameEvents.ScreenTextMaterialsEvent.RemoveListener(new UnityAction<Material[]>(this.OnMaterialsChanged));
	}

	public void OnScreenTextChanged(string text)
	{
		this.myScreenText.text = text;
	}

	public void OnFunctionTextChanged(string text)
	{
		this.myFunctionText.text = text;
	}

	private void OnMaterialsChanged(Material[] materials)
	{
		this.monitorMesh.materials = materials;
	}

	[CompilerGenerated]
	private IEnumerator <OnEnable>g__OnEnable_Local|3_0()
	{
		yield return new WaitUntil(() => GorillaComputer.instance != null && GorillaComputer.instance.initialized);
		yield return null;
		this.Init();
		yield break;
	}

	public Text myScreenText;

	public Text myFunctionText;

	public MeshRenderer monitorMesh;
}
