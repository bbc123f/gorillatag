using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

	public GorillaComputerTerminal()
	{
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

	[CompilerGenerated]
	private sealed class <<OnEnable>g__OnEnable_Local|3_0>d : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <<OnEnable>g__OnEnable_Local|3_0>d(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			GorillaComputerTerminal gorillaComputerTerminal = this;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				this.<>2__current = new WaitUntil(new Func<bool>(GorillaComputerTerminal.<>c.<>9.<OnEnable>b__3_1));
				this.<>1__state = 1;
				return true;
			case 1:
				this.<>1__state = -1;
				this.<>2__current = null;
				this.<>1__state = 2;
				return true;
			case 2:
				this.<>1__state = -1;
				gorillaComputerTerminal.Init();
				return false;
			default:
				return false;
			}
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public GorillaComputerTerminal <>4__this;
	}

	[CompilerGenerated]
	[Serializable]
	private sealed class <>c
	{
		// Note: this type is marked as 'beforefieldinit'.
		static <>c()
		{
		}

		public <>c()
		{
		}

		internal bool <OnEnable>b__3_1()
		{
			return GorillaComputer.instance != null && GorillaComputer.instance.initialized;
		}

		public static readonly GorillaComputerTerminal.<>c <>9 = new GorillaComputerTerminal.<>c();

		public static Func<bool> <>9__3_1;
	}
}
