using System;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class OVROverlayCanvas : MonoBehaviour
{
	private void Start()
	{
		this._canvas = base.GetComponent<Canvas>();
		this._rectTransform = this._canvas.GetComponent<RectTransform>();
		float width = this._rectTransform.rect.width;
		float height = this._rectTransform.rect.height;
		float num = ((width >= height) ? 1f : (width / height));
		float num2 = ((height >= width) ? 1f : (height / width));
		int num3 = (this.ScaleViewport ? 0 : 8);
		int num4 = Mathf.CeilToInt(num * (float)(this.MaxTextureSize - num3 * 2));
		int num5 = Mathf.CeilToInt(num2 * (float)(this.MaxTextureSize - num3 * 2));
		int num6 = num4 + num3 * 2;
		int num7 = num5 + num3 * 2;
		float num8 = width * ((float)num6 / (float)num4);
		float num9 = height * ((float)num7 / (float)num5);
		float num10 = (float)num4 / (float)num6;
		float num11 = (float)num5 / (float)num7;
		Vector2 vector = ((this.Opacity == OVROverlayCanvas.DrawMode.Opaque) ? new Vector2(0.005f / this._rectTransform.lossyScale.x, 0.005f / this._rectTransform.lossyScale.y) : Vector2.zero);
		this._renderTexture = new RenderTexture(num6, num7, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		this._renderTexture.useMipMap = !this.ScaleViewport;
		GameObject gameObject = new GameObject(base.name + " Overlay Camera")
		{
			hideFlags = (HideFlags.HideInHierarchy | HideFlags.NotEditable)
		};
		gameObject.transform.SetParent(base.transform, false);
		this._camera = gameObject.AddComponent<Camera>();
		this._camera.stereoTargetEye = StereoTargetEyeMask.None;
		this._camera.transform.position = base.transform.position - base.transform.forward;
		this._camera.orthographic = true;
		this._camera.enabled = false;
		this._camera.targetTexture = this._renderTexture;
		this._camera.cullingMask = 1 << base.gameObject.layer;
		this._camera.clearFlags = CameraClearFlags.Color;
		this._camera.backgroundColor = Color.clear;
		this._camera.orthographicSize = 0.5f * num9 * this._rectTransform.localScale.y;
		this._camera.nearClipPlane = 0.99f;
		this._camera.farClipPlane = 1.01f;
		this._quad = new Mesh
		{
			name = base.name + " Overlay Quad",
			hideFlags = HideFlags.HideAndDontSave
		};
		this._quad.vertices = new Vector3[]
		{
			new Vector3(-0.5f, -0.5f),
			new Vector3(-0.5f, 0.5f),
			new Vector3(0.5f, 0.5f),
			new Vector3(0.5f, -0.5f)
		};
		this._quad.uv = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f)
		};
		this._quad.triangles = new int[] { 0, 1, 2, 2, 3, 0 };
		this._quad.bounds = new Bounds(Vector3.zero, Vector3.one);
		this._quad.UploadMeshData(true);
		switch (this.Opacity)
		{
		case OVROverlayCanvas.DrawMode.Opaque:
			this._defaultMat = new Material(this._opaqueShader);
			break;
		case OVROverlayCanvas.DrawMode.OpaqueWithClip:
			this._defaultMat = new Material(this._opaqueShader);
			this._defaultMat.EnableKeyword("WITH_CLIP");
			break;
		case OVROverlayCanvas.DrawMode.TransparentDefaultAlpha:
			this._defaultMat = new Material(this._transparentShader);
			this._defaultMat.EnableKeyword("ALPHA_SQUARED");
			break;
		case OVROverlayCanvas.DrawMode.TransparentCorrectAlpha:
			this._defaultMat = new Material(this._transparentShader);
			break;
		}
		this._defaultMat.mainTexture = this._renderTexture;
		this._defaultMat.color = Color.black;
		this._defaultMat.mainTextureOffset = new Vector2(0.5f - 0.5f * num10, 0.5f - 0.5f * num11);
		this._defaultMat.mainTextureScale = new Vector2(num10, num11);
		GameObject gameObject2 = new GameObject(base.name + " MeshRenderer")
		{
			hideFlags = (HideFlags.HideInHierarchy | HideFlags.NotEditable)
		};
		gameObject2.transform.SetParent(base.transform, false);
		gameObject2.AddComponent<MeshFilter>().sharedMesh = this._quad;
		this._meshRenderer = gameObject2.AddComponent<MeshRenderer>();
		this._meshRenderer.sharedMaterial = this._defaultMat;
		gameObject2.layer = this.Layer;
		gameObject2.transform.localScale = new Vector3(width - vector.x, height - vector.y, 1f);
		GameObject gameObject3 = new GameObject(base.name + " Overlay")
		{
			hideFlags = (HideFlags.HideInHierarchy | HideFlags.NotEditable)
		};
		gameObject3.transform.SetParent(base.transform, false);
		this._overlay = gameObject3.AddComponent<OVROverlay>();
		this._overlay.isDynamic = true;
		this._overlay.noDepthBufferTesting = true;
		this._overlay.isAlphaPremultiplied = !Application.isMobilePlatform;
		this._overlay.textures[0] = this._renderTexture;
		this._overlay.currentOverlayType = OVROverlay.OverlayType.Underlay;
		this._overlay.transform.localScale = new Vector3(num8, num9, 1f);
		this._overlay.useExpensiveSuperSample = this.Expensive;
	}

	private void OnDestroy()
	{
		Object.Destroy(this._defaultMat);
		Object.Destroy(this._quad);
		Object.Destroy(this._renderTexture);
	}

	private void OnEnable()
	{
		if (this._overlay)
		{
			this._meshRenderer.enabled = true;
			this._overlay.enabled = true;
		}
		if (this._camera)
		{
			this._camera.enabled = true;
		}
	}

	private void OnDisable()
	{
		if (this._overlay)
		{
			this._overlay.enabled = false;
			this._meshRenderer.enabled = false;
		}
		if (this._camera)
		{
			this._camera.enabled = false;
		}
	}

	protected virtual bool ShouldRender()
	{
		if (this.DrawRate > 1 && Time.frameCount % this.DrawRate != this.DrawFrameOffset % this.DrawRate)
		{
			return false;
		}
		if (Camera.main != null)
		{
			for (int i = 0; i < 2; i++)
			{
				Camera.StereoscopicEye stereoscopicEye = (Camera.StereoscopicEye)i;
				GeometryUtility.CalculateFrustumPlanes(Camera.main.GetStereoProjectionMatrix(stereoscopicEye) * Camera.main.GetStereoViewMatrix(stereoscopicEye), OVROverlayCanvas._FrustumPlanes);
				if (GeometryUtility.TestPlanesAABB(OVROverlayCanvas._FrustumPlanes, this._meshRenderer.bounds))
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	private void Update()
	{
		if (this.ShouldRender())
		{
			if (this.ScaleViewport && Camera.main != null)
			{
				float magnitude = (Camera.main.transform.position - base.transform.position).magnitude;
				float num = Mathf.Ceil(this.PixelsPerUnit * Mathf.Max(this._rectTransform.rect.width * base.transform.lossyScale.x, this._rectTransform.rect.height * base.transform.lossyScale.y) / magnitude / 8f * (float)this._renderTexture.height) * 8f;
				num = Mathf.Clamp(num, (float)this.MinTextureSize, (float)this._renderTexture.height);
				float num2 = num - 2f;
				this._camera.orthographicSize = 0.5f * this._rectTransform.rect.height * this._rectTransform.localScale.y * num / num2;
				float num3 = this._rectTransform.rect.width / this._rectTransform.rect.height;
				float num4 = num2 * num3;
				float num5 = Mathf.Ceil((num4 + 2f) * 0.5f) * 2f / (float)this._renderTexture.width;
				float num6 = num / (float)this._renderTexture.height;
				float num7 = ((this.Opacity == OVROverlayCanvas.DrawMode.Opaque) ? 1.001f : 0f);
				float num8 = (num4 - num7) / (float)this._renderTexture.width;
				float num9 = (num2 - num7) / (float)this._renderTexture.height;
				this._camera.rect = new Rect((1f - num5) / 2f, (1f - num6) / 2f, num5, num6);
				Rect rect = new Rect(0.5f - 0.5f * num8, 0.5f - 0.5f * num9, num8, num9);
				this._defaultMat.mainTextureOffset = rect.min;
				this._defaultMat.mainTextureScale = rect.size;
				this._overlay.overrideTextureRectMatrix = true;
				rect.y = 1f - rect.height - rect.y;
				Rect rect2 = new Rect(0f, 0f, 1f, 1f);
				this._overlay.SetSrcDestRects(rect, rect, rect2, rect2);
			}
			this._camera.Render();
		}
	}

	public bool overlayEnabled
	{
		get
		{
			return this._overlay && this._overlay.enabled;
		}
		set
		{
			if (this._overlay)
			{
				this._overlay.enabled = value;
				this._defaultMat.color = (value ? Color.black : Color.white);
			}
		}
	}

	[SerializeField]
	[HideInInspector]
	private Shader _transparentShader;

	[SerializeField]
	[HideInInspector]
	private Shader _opaqueShader;

	private RectTransform _rectTransform;

	private Canvas _canvas;

	private Camera _camera;

	private OVROverlay _overlay;

	private RenderTexture _renderTexture;

	private MeshRenderer _meshRenderer;

	private Mesh _quad;

	private Material _defaultMat;

	public int MaxTextureSize = 1600;

	public int MinTextureSize = 200;

	public float PixelsPerUnit = 1f;

	public int DrawRate = 1;

	public int DrawFrameOffset;

	public bool Expensive;

	public int Layer;

	public OVROverlayCanvas.DrawMode Opacity = OVROverlayCanvas.DrawMode.OpaqueWithClip;

	private bool ScaleViewport = Application.isMobilePlatform;

	private static readonly Plane[] _FrustumPlanes = new Plane[6];

	public enum DrawMode
	{
		Opaque,
		OpaqueWithClip,
		TransparentDefaultAlpha,
		TransparentCorrectAlpha
	}
}
