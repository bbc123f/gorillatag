using System;
using System.Collections;
using UnityEngine;

public class PassthroughStyler : MonoBehaviour
{
	private void Start()
	{
		if (base.GetComponent<GrabObject>())
		{
			GrabObject component = base.GetComponent<GrabObject>();
			component.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(component.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject component2 = base.GetComponent<GrabObject>();
			component2.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(component2.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
			GrabObject component3 = base.GetComponent<GrabObject>();
			component3.CursorPositionDelegate = (GrabObject.SetCursorPosition)Delegate.Combine(component3.CursorPositionDelegate, new GrabObject.SetCursorPosition(this.Cursor));
		}
		this.savedColor = new Color(1f, 1f, 1f, 0f);
		this.ShowFullMenu(false);
		this.mainCanvas.interactable = false;
		this.passthroughLayer.colorMapEditorType = OVRPassthroughLayer.ColorMapEditorType.ColorAdjustment;
	}

	private void Update()
	{
		if (this.controllerHand == OVRInput.Controller.None)
		{
			return;
		}
		if (this.settingColor)
		{
			this.GetColorFromWheel();
		}
	}

	public void Grab(OVRInput.Controller grabHand)
	{
		this.controllerHand = grabHand;
		this.ShowFullMenu(true);
		if (this.mainCanvas)
		{
			this.mainCanvas.interactable = true;
		}
		if (this.fadeIn != null)
		{
			base.StopCoroutine(this.fadeIn);
		}
		if (this.fadeOut != null)
		{
			base.StopCoroutine(this.fadeOut);
		}
		this.fadeIn = this.FadeToCurrentStyle(0.2f);
		base.StartCoroutine(this.fadeIn);
	}

	public void Release()
	{
		this.controllerHand = OVRInput.Controller.None;
		this.ShowFullMenu(false);
		if (this.mainCanvas)
		{
			this.mainCanvas.interactable = false;
		}
		if (this.fadeIn != null)
		{
			base.StopCoroutine(this.fadeIn);
		}
		if (this.fadeOut != null)
		{
			base.StopCoroutine(this.fadeOut);
		}
		this.fadeOut = this.FadeToDefaultPassthrough(0.2f);
		base.StartCoroutine(this.fadeOut);
	}

	private IEnumerator FadeToCurrentStyle(float fadeTime)
	{
		float timer = 0f;
		float brightness = this.passthroughLayer.colorMapEditorBrightness;
		float contrast = this.passthroughLayer.colorMapEditorContrast;
		Color edgeCol = new Color(this.savedColor.r, this.savedColor.g, this.savedColor.b, 0f);
		this.passthroughLayer.edgeRenderingEnabled = true;
		while (timer <= fadeTime)
		{
			timer += Time.deltaTime;
			float num = Mathf.Clamp01(timer / fadeTime);
			this.passthroughLayer.colorMapEditorBrightness = Mathf.Lerp(brightness, this.savedBrightness, num);
			this.passthroughLayer.colorMapEditorContrast = Mathf.Lerp(contrast, this.savedContrast, num);
			this.passthroughLayer.edgeColor = Color.Lerp(edgeCol, this.savedColor, num);
			yield return null;
		}
		yield break;
	}

	private IEnumerator FadeToDefaultPassthrough(float fadeTime)
	{
		float timer = 0f;
		float brightness = this.passthroughLayer.colorMapEditorBrightness;
		float contrast = this.passthroughLayer.colorMapEditorContrast;
		Color edgeCol = this.passthroughLayer.edgeColor;
		while (timer <= fadeTime)
		{
			timer += Time.deltaTime;
			float num = Mathf.Clamp01(timer / fadeTime);
			this.passthroughLayer.colorMapEditorBrightness = Mathf.Lerp(brightness, 0f, num);
			this.passthroughLayer.colorMapEditorContrast = Mathf.Lerp(contrast, 0f, num);
			this.passthroughLayer.edgeColor = Color.Lerp(edgeCol, new Color(edgeCol.r, edgeCol.g, edgeCol.b, 0f), num);
			if (timer > fadeTime)
			{
				this.passthroughLayer.edgeRenderingEnabled = false;
			}
			yield return null;
		}
		yield break;
	}

	public void OnBrightnessChanged(float newValue)
	{
		this.savedBrightness = newValue;
		this.passthroughLayer.colorMapEditorBrightness = this.savedBrightness;
	}

	public void OnContrastChanged(float newValue)
	{
		this.savedContrast = newValue;
		this.passthroughLayer.colorMapEditorContrast = this.savedContrast;
	}

	public void OnAlphaChanged(float newValue)
	{
		this.savedColor = new Color(this.savedColor.r, this.savedColor.g, this.savedColor.b, newValue);
		this.passthroughLayer.edgeColor = this.savedColor;
	}

	private void ShowFullMenu(bool doShow)
	{
		GameObject[] array = this.compactObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(doShow);
		}
	}

	public void Cursor(Vector3 cP)
	{
		this.cursorPosition = cP;
	}

	public void DoColorDrag(bool doDrag)
	{
		this.settingColor = doDrag;
	}

	public void GetColorFromWheel()
	{
		Vector3 vector = this.colorWheel.transform.InverseTransformPoint(this.cursorPosition);
		Vector2 vector2 = new Vector2(vector.x / this.colorWheel.sizeDelta.x + 0.5f, vector.y / this.colorWheel.sizeDelta.y + 0.5f);
		Debug.Log("Sanctuary: " + vector2.x.ToString() + ", " + vector2.y.ToString());
		Color color = Color.black;
		if ((double)vector2.x < 1.0 && vector2.x > 0f && (double)vector2.y < 1.0 && vector2.y > 0f)
		{
			int num = Mathf.RoundToInt(vector2.x * (float)this.colorTexture.width);
			int num2 = Mathf.RoundToInt(vector2.y * (float)this.colorTexture.height);
			color = this.colorTexture.GetPixel(num, num2);
		}
		this.savedColor = new Color(color.r, color.g, color.b, this.savedColor.a);
		this.passthroughLayer.edgeColor = this.savedColor;
	}

	public OVRInput.Controller controllerHand;

	public OVRPassthroughLayer passthroughLayer;

	private IEnumerator fadeIn;

	private IEnumerator fadeOut;

	public RectTransform[] menuOptions;

	public RectTransform colorWheel;

	public Texture2D colorTexture;

	private Vector3 cursorPosition = Vector3.zero;

	private bool settingColor;

	private Color savedColor = Color.white;

	private float savedBrightness;

	private float savedContrast;

	public CanvasGroup mainCanvas;

	public GameObject[] compactObjects;
}
