using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace OculusSampleFramework
{
	public class OVROverlaySample : MonoBehaviour
	{
		private void Start()
		{
			DebugUIBuilder.instance.AddLabel("OVROverlay Sample", 0);
			DebugUIBuilder.instance.AddDivider(0);
			DebugUIBuilder.instance.AddLabel("Level Loading Example", 0);
			DebugUIBuilder.instance.AddButton("Simulate Level Load", new DebugUIBuilder.OnClick(this.TriggerLoad), -1, 0, false);
			DebugUIBuilder.instance.AddButton("Destroy Cubes", new DebugUIBuilder.OnClick(this.TriggerUnload), -1, 0, false);
			DebugUIBuilder.instance.AddDivider(0);
			DebugUIBuilder.instance.AddLabel("OVROverlay vs. Application Render Comparison", 0);
			DebugUIBuilder.instance.AddRadio("OVROverlay", "group", delegate(Toggle t)
			{
				this.RadioPressed("OVROverlayID", "group", t);
			}, 0).GetComponentInChildren<Toggle>();
			this.applicationRadioButton = DebugUIBuilder.instance.AddRadio("Application", "group", delegate(Toggle t)
			{
				this.RadioPressed("ApplicationID", "group", t);
			}, 0).GetComponentInChildren<Toggle>();
			this.noneRadioButton = DebugUIBuilder.instance.AddRadio("None", "group", delegate(Toggle t)
			{
				this.RadioPressed("NoneID", "group", t);
			}, 0).GetComponentInChildren<Toggle>();
			DebugUIBuilder.instance.Show();
			this.CameraAndRenderTargetSetup();
			this.cameraRenderOverlay.enabled = true;
			this.cameraRenderOverlay.currentOverlayShape = OVROverlay.OverlayShape.Quad;
			this.spawnedCubes.Capacity = this.numObjectsPerLevel * this.numLevels;
		}

		private void Update()
		{
			if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Active) || OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Active))
			{
				if (this.inMenu)
				{
					DebugUIBuilder.instance.Hide();
				}
				else
				{
					DebugUIBuilder.instance.Show();
				}
				this.inMenu = !this.inMenu;
			}
			if (Input.GetKeyDown(KeyCode.A))
			{
				this.TriggerLoad();
			}
		}

		private void ActivateWorldGeo()
		{
			this.worldspaceGeoParent.SetActive(true);
			this.uiGeoParent.SetActive(false);
			this.uiCamera.SetActive(false);
			this.cameraRenderOverlay.enabled = false;
			this.renderingLabelOverlay.enabled = true;
			this.renderingLabelOverlay.textures[0] = this.applicationLabelTexture;
			Debug.Log("Switched to ActivateWorldGeo");
		}

		private void ActivateOVROverlay()
		{
			this.worldspaceGeoParent.SetActive(false);
			this.uiCamera.SetActive(true);
			this.cameraRenderOverlay.enabled = true;
			this.uiGeoParent.SetActive(true);
			this.renderingLabelOverlay.enabled = true;
			this.renderingLabelOverlay.textures[0] = this.compositorLabelTexture;
			Debug.Log("Switched to ActivateOVROVerlay");
		}

		private void ActivateNone()
		{
			this.worldspaceGeoParent.SetActive(false);
			this.uiCamera.SetActive(false);
			this.cameraRenderOverlay.enabled = false;
			this.uiGeoParent.SetActive(false);
			this.renderingLabelOverlay.enabled = false;
			Debug.Log("Switched to ActivateNone");
		}

		private void TriggerLoad()
		{
			base.StartCoroutine(this.WaitforOVROverlay());
		}

		private IEnumerator WaitforOVROverlay()
		{
			Transform transform = this.mainCamera.transform;
			Transform transform2 = this.loadingTextQuadOverlay.transform;
			Vector3 vector = transform.position + transform.forward * this.distanceFromCamToLoadText;
			vector.y = transform.position.y;
			transform2.position = vector;
			this.cubemapOverlay.enabled = true;
			this.loadingTextQuadOverlay.enabled = true;
			this.noneRadioButton.isOn = true;
			yield return new WaitForSeconds(0.1f);
			this.ClearObjects();
			this.SimulateLevelLoad();
			this.cubemapOverlay.enabled = false;
			this.loadingTextQuadOverlay.enabled = false;
			yield return null;
			yield break;
		}

		private void TriggerUnload()
		{
			this.ClearObjects();
			this.applicationRadioButton.isOn = true;
		}

		private void CameraAndRenderTargetSetup()
		{
			float x = this.cameraRenderOverlay.transform.localScale.x;
			float y = this.cameraRenderOverlay.transform.localScale.y;
			float z = this.cameraRenderOverlay.transform.localScale.z;
			float num = 2160f;
			float num2 = 1200f;
			float num3 = num * 0.5f;
			float num4 = num2;
			float num5 = this.mainCamera.GetComponent<Camera>().fieldOfView / 2f;
			float num6 = 2f * z * Mathf.Tan(0.017453292f * num5);
			float num7 = num4 / num6 * x;
			float num8 = num6 * this.mainCamera.GetComponent<Camera>().aspect;
			float num9 = num3 / num8 * x;
			float num10 = y / 2f;
			float num11 = x / y;
			this.uiCamera.GetComponent<Camera>().orthographicSize = num10;
			this.uiCamera.GetComponent<Camera>().aspect = num11;
			if (this.uiCamera.GetComponent<Camera>().targetTexture != null)
			{
				this.uiCamera.GetComponent<Camera>().targetTexture.Release();
			}
			RenderTexture renderTexture = new RenderTexture((int)num9 * 2, (int)num7 * 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
			Debug.Log("Created RT of resolution w: " + num9.ToString() + " and h: " + num7.ToString());
			renderTexture.hideFlags = HideFlags.DontSave;
			renderTexture.useMipMap = true;
			renderTexture.filterMode = FilterMode.Trilinear;
			renderTexture.anisoLevel = 4;
			renderTexture.autoGenerateMips = true;
			this.uiCamera.GetComponent<Camera>().targetTexture = renderTexture;
			this.cameraRenderOverlay.textures[0] = renderTexture;
		}

		private void SimulateLevelLoad()
		{
			int num = 0;
			for (int i = 0; i < this.numLoopsTrigger; i++)
			{
				num++;
			}
			Debug.Log("Finished " + num.ToString() + " Loops");
			Vector3 position = this.mainCamera.transform.position;
			position.y = 0.5f;
			for (int j = 0; j < this.numLevels; j++)
			{
				for (int k = 0; k < this.numObjectsPerLevel; k++)
				{
					float num2 = (float)k * 3.1415927f * 2f / (float)this.numObjectsPerLevel;
					float num3 = ((k % 2 == 0) ? 1.5f : 1f);
					Vector3 vector = new Vector3(Mathf.Cos(num2), 0f, Mathf.Sin(num2)) * this.cubeSpawnRadius * num3;
					vector.y = (float)j * this.heightBetweenItems;
					GameObject gameObject = Object.Instantiate<GameObject>(this.prefabForLevelLoadSim, vector + position, Quaternion.identity);
					Transform transform = gameObject.transform;
					transform.LookAt(position);
					Vector3 eulerAngles = transform.rotation.eulerAngles;
					eulerAngles.x = 0f;
					transform.rotation = Quaternion.Euler(eulerAngles);
					this.spawnedCubes.Add(gameObject);
				}
			}
		}

		private void ClearObjects()
		{
			for (int i = 0; i < this.spawnedCubes.Count; i++)
			{
				Object.DestroyImmediate(this.spawnedCubes[i]);
			}
			this.spawnedCubes.Clear();
			GC.Collect();
		}

		public void RadioPressed(string radioLabel, string group, Toggle t)
		{
			if (string.Compare(radioLabel, "OVROverlayID") == 0)
			{
				this.ActivateOVROverlay();
				return;
			}
			if (string.Compare(radioLabel, "ApplicationID") == 0)
			{
				this.ActivateWorldGeo();
				return;
			}
			if (string.Compare(radioLabel, "NoneID") == 0)
			{
				this.ActivateNone();
			}
		}

		public OVROverlaySample()
		{
		}

		[CompilerGenerated]
		private void <Start>b__24_0(Toggle t)
		{
			this.RadioPressed("OVROverlayID", "group", t);
		}

		[CompilerGenerated]
		private void <Start>b__24_1(Toggle t)
		{
			this.RadioPressed("ApplicationID", "group", t);
		}

		[CompilerGenerated]
		private void <Start>b__24_2(Toggle t)
		{
			this.RadioPressed("NoneID", "group", t);
		}

		private bool inMenu;

		private const string ovrOverlayID = "OVROverlayID";

		private const string applicationID = "ApplicationID";

		private const string noneID = "NoneID";

		private Toggle applicationRadioButton;

		private Toggle noneRadioButton;

		[Header("App vs Compositor Comparison Settings")]
		public GameObject mainCamera;

		public GameObject uiCamera;

		public GameObject uiGeoParent;

		public GameObject worldspaceGeoParent;

		public OVROverlay cameraRenderOverlay;

		public OVROverlay renderingLabelOverlay;

		public Texture applicationLabelTexture;

		public Texture compositorLabelTexture;

		[Header("Level Loading Sim Settings")]
		public GameObject prefabForLevelLoadSim;

		public OVROverlay cubemapOverlay;

		public OVROverlay loadingTextQuadOverlay;

		public float distanceFromCamToLoadText;

		public float cubeSpawnRadius;

		public float heightBetweenItems;

		public int numObjectsPerLevel;

		public int numLevels;

		public int numLoopsTrigger = 500000000;

		private List<GameObject> spawnedCubes = new List<GameObject>();

		[CompilerGenerated]
		private sealed class <WaitforOVROverlay>d__30 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <WaitforOVROverlay>d__30(int <>1__state)
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
				OVROverlaySample ovroverlaySample = this;
				switch (num)
				{
				case 0:
				{
					this.<>1__state = -1;
					Transform transform = ovroverlaySample.mainCamera.transform;
					Transform transform2 = ovroverlaySample.loadingTextQuadOverlay.transform;
					Vector3 vector = transform.position + transform.forward * ovroverlaySample.distanceFromCamToLoadText;
					vector.y = transform.position.y;
					transform2.position = vector;
					ovroverlaySample.cubemapOverlay.enabled = true;
					ovroverlaySample.loadingTextQuadOverlay.enabled = true;
					ovroverlaySample.noneRadioButton.isOn = true;
					this.<>2__current = new WaitForSeconds(0.1f);
					this.<>1__state = 1;
					return true;
				}
				case 1:
					this.<>1__state = -1;
					ovroverlaySample.ClearObjects();
					ovroverlaySample.SimulateLevelLoad();
					ovroverlaySample.cubemapOverlay.enabled = false;
					ovroverlaySample.loadingTextQuadOverlay.enabled = false;
					this.<>2__current = null;
					this.<>1__state = 2;
					return true;
				case 2:
					this.<>1__state = -1;
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

			public OVROverlaySample <>4__this;
		}
	}
}
