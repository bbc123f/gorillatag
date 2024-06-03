using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PassthroughStyler : MonoBehaviour
{
	private void Start()
	{
		GrabObject grabObject;
		if (base.TryGetComponent<GrabObject>(out grabObject))
		{
			GrabObject grabObject2 = grabObject;
			grabObject2.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(grabObject2.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject grabObject3 = grabObject;
			grabObject3.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(grabObject3.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
			GrabObject grabObject4 = grabObject;
			grabObject4.CursorPositionDelegate = (GrabObject.SetCursorPosition)Delegate.Combine(grabObject4.CursorPositionDelegate, new GrabObject.SetCursorPosition(this.Cursor));
		}
		this._savedColor = new Color(1f, 1f, 1f, 0f);
		this.ShowFullMenu(false);
		this._mainCanvas.interactable = false;
		this._passthroughColorLut = new OVRPassthroughColorLut(this._colorLutTexture, true);
		if (!OVRManager.GetPassthroughCapabilities().SupportsColorPassthrough && this._objectsToHideForColorPassthrough != null)
		{
			for (int i = 0; i < this._objectsToHideForColorPassthrough.Length; i++)
			{
				this._objectsToHideForColorPassthrough[i].SetActive(false);
			}
		}
	}

	private void Update()
	{
		if (this._controllerHand == OVRInput.Controller.None)
		{
			return;
		}
		if (this._settingColor)
		{
			this.GetColorFromWheel();
		}
	}

	public void OnBrightnessChanged(float newValue)
	{
		this._savedBrightness = newValue;
		this.UpdateBrighnessContrastSaturation();
	}

	public void OnContrastChanged(float newValue)
	{
		this._savedContrast = newValue;
		this.UpdateBrighnessContrastSaturation();
	}

	public void OnSaturationChanged(float newValue)
	{
		this._savedSaturation = newValue;
		this.UpdateBrighnessContrastSaturation();
	}

	public void OnAlphaChanged(float newValue)
	{
		this._savedColor = new Color(this._savedColor.r, this._savedColor.g, this._savedColor.b, newValue);
		this._passthroughLayer.edgeColor = this._savedColor;
	}

	public void OnBlendChange(float newValue)
	{
		this._savedBlend = newValue;
		this._passthroughLayer.SetColorLut(this._passthroughColorLut, this._savedBlend);
	}

	public void DoColorDrag(bool doDrag)
	{
		this._settingColor = doDrag;
	}

	public void SetPassthroughStyleToColorAdjustment(bool isOn)
	{
		if (isOn)
		{
			this.SetPassthroughStyle(OVRPassthroughLayer.ColorMapEditorType.ColorAdjustment);
		}
	}

	public void SetPassthroughStyleToColorLut(bool isOn)
	{
		if (isOn)
		{
			this.SetPassthroughStyle(OVRPassthroughLayer.ColorMapEditorType.ColorLut);
		}
	}

	private void Grab(OVRInput.Controller grabHand)
	{
		this._controllerHand = grabHand;
		this.ShowFullMenu(true);
		if (this._mainCanvas)
		{
			this._mainCanvas.interactable = true;
		}
		if (this._fade != null)
		{
			base.StopCoroutine(this._fade);
		}
		this._fade = this.FadeToCurrentStyle(0.2f);
		base.StartCoroutine(this._fade);
	}

	private void Release()
	{
		this._controllerHand = OVRInput.Controller.None;
		this.ShowFullMenu(false);
		if (this._mainCanvas)
		{
			this._mainCanvas.interactable = false;
		}
		if (this._fade != null)
		{
			base.StopCoroutine(this._fade);
		}
		this._fade = this.FadeToDefaultPassthrough(0.2f);
		base.StartCoroutine(this._fade);
	}

	private IEnumerator FadeToCurrentStyle(float fadeTime)
	{
		this._passthroughLayer.edgeRenderingEnabled = true;
		yield return this.FadeTo(1f, fadeTime);
		yield break;
	}

	private IEnumerator FadeToDefaultPassthrough(float fadeTime)
	{
		yield return this.FadeTo(0f, fadeTime);
		this._passthroughLayer.edgeRenderingEnabled = false;
		yield break;
	}

	private IEnumerator FadeTo(float styleValueMultiplier, float duration)
	{
		float timer = 0f;
		float brightness = this._passthroughLayer.colorMapEditorBrightness;
		float contrast = this._passthroughLayer.colorMapEditorContrast;
		float saturation = this._passthroughLayer.colorMapEditorSaturation;
		Color edgeCol = this._passthroughLayer.edgeColor;
		float blend = this._savedBlend;
		while (timer <= duration)
		{
			timer += Time.deltaTime;
			float t = Mathf.Clamp01(timer / duration);
			if (this._currentStyle == OVRPassthroughLayer.ColorMapEditorType.ColorLut)
			{
				this._passthroughLayer.SetColorLut(this._passthroughColorLut, Mathf.Lerp(blend, this._savedBlend * styleValueMultiplier, t));
			}
			else
			{
				this._passthroughLayer.SetBrightnessContrastSaturation(Mathf.Lerp(brightness, this._savedBrightness * styleValueMultiplier, t), Mathf.Lerp(contrast, this._savedContrast * styleValueMultiplier, t), Mathf.Lerp(saturation, this._savedSaturation * styleValueMultiplier, t));
			}
			this._passthroughLayer.edgeColor = Color.Lerp(edgeCol, new Color(this._savedColor.r, this._savedColor.g, this._savedColor.b, this._savedColor.a * styleValueMultiplier), t);
			yield return null;
		}
		yield break;
	}

	private void UpdateBrighnessContrastSaturation()
	{
		this._passthroughLayer.SetBrightnessContrastSaturation(this._savedBrightness, this._savedContrast, this._savedSaturation);
	}

	private void ShowFullMenu(bool doShow)
	{
		GameObject[] compactObjects = this._compactObjects;
		for (int i = 0; i < compactObjects.Length; i++)
		{
			compactObjects[i].SetActive(doShow);
		}
	}

	private void Cursor(Vector3 cP)
	{
		this._cursorPosition = cP;
	}

	private void GetColorFromWheel()
	{
		Vector3 vector = this._colorWheel.transform.InverseTransformPoint(this._cursorPosition);
		Vector2 vector2 = new Vector2(vector.x / this._colorWheel.sizeDelta.x + 0.5f, vector.y / this._colorWheel.sizeDelta.y + 0.5f);
		Debug.Log("Sanctuary: " + vector2.x.ToString() + ", " + vector2.y.ToString());
		Color color = Color.black;
		if ((double)vector2.x < 1.0 && vector2.x > 0f && (double)vector2.y < 1.0 && vector2.y > 0f)
		{
			int x = Mathf.RoundToInt(vector2.x * (float)this._colorTexture.width);
			int y = Mathf.RoundToInt(vector2.y * (float)this._colorTexture.height);
			color = this._colorTexture.GetPixel(x, y);
		}
		this._savedColor = new Color(color.r, color.g, color.b, this._savedColor.a);
		this._passthroughLayer.edgeColor = this._savedColor;
	}

	private void SetPassthroughStyle(OVRPassthroughLayer.ColorMapEditorType passthroughStyle)
	{
		this._currentStyle = passthroughStyle;
		if (this._currentStyle == OVRPassthroughLayer.ColorMapEditorType.ColorLut)
		{
			this._passthroughLayer.SetColorLut(this._passthroughColorLut, this._savedBlend);
			return;
		}
		this.UpdateBrighnessContrastSaturation();
	}

	public PassthroughStyler()
	{
	}

	private const float FadeDuration = 0.2f;

	[SerializeField]
	private OVRInput.Controller _controllerHand;

	[SerializeField]
	private OVRPassthroughLayer _passthroughLayer;

	[SerializeField]
	private RectTransform _colorWheel;

	[SerializeField]
	private Texture2D _colorTexture;

	[SerializeField]
	private Texture2D _colorLutTexture;

	[SerializeField]
	private CanvasGroup _mainCanvas;

	[SerializeField]
	private GameObject[] _compactObjects;

	[SerializeField]
	private GameObject[] _objectsToHideForColorPassthrough;

	private Vector3 _cursorPosition = Vector3.zero;

	private bool _settingColor;

	private Color _savedColor = Color.white;

	private float _savedBrightness;

	private float _savedContrast;

	private float _savedSaturation;

	private OVRPassthroughLayer.ColorMapEditorType _currentStyle = OVRPassthroughLayer.ColorMapEditorType.ColorAdjustment;

	private float _savedBlend = 1f;

	private OVRPassthroughColorLut _passthroughColorLut;

	private IEnumerator _fade;

	[CompilerGenerated]
	private sealed class <FadeTo>d__33 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <FadeTo>d__33(int <>1__state)
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
			PassthroughStyler passthroughStyler = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
			}
			else
			{
				this.<>1__state = -1;
				timer = 0f;
				brightness = passthroughStyler._passthroughLayer.colorMapEditorBrightness;
				contrast = passthroughStyler._passthroughLayer.colorMapEditorContrast;
				saturation = passthroughStyler._passthroughLayer.colorMapEditorSaturation;
				edgeCol = passthroughStyler._passthroughLayer.edgeColor;
				blend = passthroughStyler._savedBlend;
			}
			if (timer > duration)
			{
				return false;
			}
			timer += Time.deltaTime;
			float t = Mathf.Clamp01(timer / duration);
			if (passthroughStyler._currentStyle == OVRPassthroughLayer.ColorMapEditorType.ColorLut)
			{
				passthroughStyler._passthroughLayer.SetColorLut(passthroughStyler._passthroughColorLut, Mathf.Lerp(blend, passthroughStyler._savedBlend * styleValueMultiplier, t));
			}
			else
			{
				passthroughStyler._passthroughLayer.SetBrightnessContrastSaturation(Mathf.Lerp(brightness, passthroughStyler._savedBrightness * styleValueMultiplier, t), Mathf.Lerp(contrast, passthroughStyler._savedContrast * styleValueMultiplier, t), Mathf.Lerp(saturation, passthroughStyler._savedSaturation * styleValueMultiplier, t));
			}
			passthroughStyler._passthroughLayer.edgeColor = Color.Lerp(edgeCol, new Color(passthroughStyler._savedColor.r, passthroughStyler._savedColor.g, passthroughStyler._savedColor.b, passthroughStyler._savedColor.a * styleValueMultiplier), t);
			this.<>2__current = null;
			this.<>1__state = 1;
			return true;
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

		public PassthroughStyler <>4__this;

		public float duration;

		public float styleValueMultiplier;

		private float <timer>5__2;

		private float <brightness>5__3;

		private float <contrast>5__4;

		private float <saturation>5__5;

		private Color <edgeCol>5__6;

		private float <blend>5__7;
	}

	[CompilerGenerated]
	private sealed class <FadeToCurrentStyle>d__31 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <FadeToCurrentStyle>d__31(int <>1__state)
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
			PassthroughStyler passthroughStyler = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				passthroughStyler._passthroughLayer.edgeRenderingEnabled = true;
				this.<>2__current = passthroughStyler.FadeTo(1f, fadeTime);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			return false;
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

		public PassthroughStyler <>4__this;

		public float fadeTime;
	}

	[CompilerGenerated]
	private sealed class <FadeToDefaultPassthrough>d__32 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <FadeToDefaultPassthrough>d__32(int <>1__state)
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
			PassthroughStyler passthroughStyler = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				this.<>2__current = passthroughStyler.FadeTo(0f, fadeTime);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			passthroughStyler._passthroughLayer.edgeRenderingEnabled = false;
			return false;
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

		public PassthroughStyler <>4__this;

		public float fadeTime;
	}
}
