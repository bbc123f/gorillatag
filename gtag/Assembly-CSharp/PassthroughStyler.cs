using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000B7 RID: 183
public class PassthroughStyler : MonoBehaviour
{
	// Token: 0x06000407 RID: 1031 RVA: 0x0001AAC4 File Offset: 0x00018CC4
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

	// Token: 0x06000408 RID: 1032 RVA: 0x0001AB91 File Offset: 0x00018D91
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

	// Token: 0x06000409 RID: 1033 RVA: 0x0001ABAC File Offset: 0x00018DAC
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

	// Token: 0x0600040A RID: 1034 RVA: 0x0001AC28 File Offset: 0x00018E28
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

	// Token: 0x0600040B RID: 1035 RVA: 0x0001ACA2 File Offset: 0x00018EA2
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
			float t = Mathf.Clamp01(timer / fadeTime);
			this.passthroughLayer.colorMapEditorBrightness = Mathf.Lerp(brightness, this.savedBrightness, t);
			this.passthroughLayer.colorMapEditorContrast = Mathf.Lerp(contrast, this.savedContrast, t);
			this.passthroughLayer.edgeColor = Color.Lerp(edgeCol, this.savedColor, t);
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x0001ACB8 File Offset: 0x00018EB8
	private IEnumerator FadeToDefaultPassthrough(float fadeTime)
	{
		float timer = 0f;
		float brightness = this.passthroughLayer.colorMapEditorBrightness;
		float contrast = this.passthroughLayer.colorMapEditorContrast;
		Color edgeCol = this.passthroughLayer.edgeColor;
		while (timer <= fadeTime)
		{
			timer += Time.deltaTime;
			float t = Mathf.Clamp01(timer / fadeTime);
			this.passthroughLayer.colorMapEditorBrightness = Mathf.Lerp(brightness, 0f, t);
			this.passthroughLayer.colorMapEditorContrast = Mathf.Lerp(contrast, 0f, t);
			this.passthroughLayer.edgeColor = Color.Lerp(edgeCol, new Color(edgeCol.r, edgeCol.g, edgeCol.b, 0f), t);
			if (timer > fadeTime)
			{
				this.passthroughLayer.edgeRenderingEnabled = false;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x0001ACCE File Offset: 0x00018ECE
	public void OnBrightnessChanged(float newValue)
	{
		this.savedBrightness = newValue;
		this.passthroughLayer.colorMapEditorBrightness = this.savedBrightness;
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x0001ACE8 File Offset: 0x00018EE8
	public void OnContrastChanged(float newValue)
	{
		this.savedContrast = newValue;
		this.passthroughLayer.colorMapEditorContrast = this.savedContrast;
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x0001AD02 File Offset: 0x00018F02
	public void OnAlphaChanged(float newValue)
	{
		this.savedColor = new Color(this.savedColor.r, this.savedColor.g, this.savedColor.b, newValue);
		this.passthroughLayer.edgeColor = this.savedColor;
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x0001AD44 File Offset: 0x00018F44
	private void ShowFullMenu(bool doShow)
	{
		GameObject[] array = this.compactObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(doShow);
		}
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x0001AD6F File Offset: 0x00018F6F
	public void Cursor(Vector3 cP)
	{
		this.cursorPosition = cP;
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x0001AD78 File Offset: 0x00018F78
	public void DoColorDrag(bool doDrag)
	{
		this.settingColor = doDrag;
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x0001AD84 File Offset: 0x00018F84
	public void GetColorFromWheel()
	{
		Vector3 vector = this.colorWheel.transform.InverseTransformPoint(this.cursorPosition);
		Vector2 vector2 = new Vector2(vector.x / this.colorWheel.sizeDelta.x + 0.5f, vector.y / this.colorWheel.sizeDelta.y + 0.5f);
		Debug.Log("Sanctuary: " + vector2.x.ToString() + ", " + vector2.y.ToString());
		Color color = Color.black;
		if ((double)vector2.x < 1.0 && vector2.x > 0f && (double)vector2.y < 1.0 && vector2.y > 0f)
		{
			int x = Mathf.RoundToInt(vector2.x * (float)this.colorTexture.width);
			int y = Mathf.RoundToInt(vector2.y * (float)this.colorTexture.height);
			color = this.colorTexture.GetPixel(x, y);
		}
		this.savedColor = new Color(color.r, color.g, color.b, this.savedColor.a);
		this.passthroughLayer.edgeColor = this.savedColor;
	}

	// Token: 0x040004B5 RID: 1205
	public OVRInput.Controller controllerHand;

	// Token: 0x040004B6 RID: 1206
	public OVRPassthroughLayer passthroughLayer;

	// Token: 0x040004B7 RID: 1207
	private IEnumerator fadeIn;

	// Token: 0x040004B8 RID: 1208
	private IEnumerator fadeOut;

	// Token: 0x040004B9 RID: 1209
	public RectTransform[] menuOptions;

	// Token: 0x040004BA RID: 1210
	public RectTransform colorWheel;

	// Token: 0x040004BB RID: 1211
	public Texture2D colorTexture;

	// Token: 0x040004BC RID: 1212
	private Vector3 cursorPosition = Vector3.zero;

	// Token: 0x040004BD RID: 1213
	private bool settingColor;

	// Token: 0x040004BE RID: 1214
	private Color savedColor = Color.white;

	// Token: 0x040004BF RID: 1215
	private float savedBrightness;

	// Token: 0x040004C0 RID: 1216
	private float savedContrast;

	// Token: 0x040004C1 RID: 1217
	public CanvasGroup mainCanvas;

	// Token: 0x040004C2 RID: 1218
	public GameObject[] compactObjects;
}
