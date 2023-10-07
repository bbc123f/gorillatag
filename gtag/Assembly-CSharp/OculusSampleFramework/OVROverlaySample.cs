using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OculusSampleFramework
{
	// Token: 0x020002F3 RID: 755
	public class OVROverlaySample : MonoBehaviour
	{
		// Token: 0x0600147A RID: 5242 RVA: 0x00073584 File Offset: 0x00071784
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

		// Token: 0x0600147B RID: 5243 RVA: 0x000736DC File Offset: 0x000718DC
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

		// Token: 0x0600147C RID: 5244 RVA: 0x00073744 File Offset: 0x00071944
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

		// Token: 0x0600147D RID: 5245 RVA: 0x000737AC File Offset: 0x000719AC
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

		// Token: 0x0600147E RID: 5246 RVA: 0x00073814 File Offset: 0x00071A14
		private void ActivateNone()
		{
			this.worldspaceGeoParent.SetActive(false);
			this.uiCamera.SetActive(false);
			this.cameraRenderOverlay.enabled = false;
			this.uiGeoParent.SetActive(false);
			this.renderingLabelOverlay.enabled = false;
			Debug.Log("Switched to ActivateNone");
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x00073867 File Offset: 0x00071A67
		private void TriggerLoad()
		{
			base.StartCoroutine(this.WaitforOVROverlay());
		}

		// Token: 0x06001480 RID: 5248 RVA: 0x00073876 File Offset: 0x00071A76
		private IEnumerator WaitforOVROverlay()
		{
			Transform transform = this.mainCamera.transform;
			Transform transform2 = this.loadingTextQuadOverlay.transform;
			Vector3 position = transform.position + transform.forward * this.distanceFromCamToLoadText;
			position.y = transform.position.y;
			transform2.position = position;
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

		// Token: 0x06001481 RID: 5249 RVA: 0x00073885 File Offset: 0x00071A85
		private void TriggerUnload()
		{
			this.ClearObjects();
			this.applicationRadioButton.isOn = true;
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x0007389C File Offset: 0x00071A9C
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
			float orthographicSize = y / 2f;
			float aspect = x / y;
			this.uiCamera.GetComponent<Camera>().orthographicSize = orthographicSize;
			this.uiCamera.GetComponent<Camera>().aspect = aspect;
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

		// Token: 0x06001483 RID: 5251 RVA: 0x00073A38 File Offset: 0x00071C38
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
					float f = (float)k * 3.1415927f * 2f / (float)this.numObjectsPerLevel;
					float d = (k % 2 == 0) ? 1.5f : 1f;
					Vector3 a = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f)) * this.cubeSpawnRadius * d;
					a.y = (float)j * this.heightBetweenItems;
					GameObject gameObject = Object.Instantiate<GameObject>(this.prefabForLevelLoadSim, a + position, Quaternion.identity);
					Transform transform = gameObject.transform;
					transform.LookAt(position);
					Vector3 eulerAngles = transform.rotation.eulerAngles;
					eulerAngles.x = 0f;
					transform.rotation = Quaternion.Euler(eulerAngles);
					this.spawnedCubes.Add(gameObject);
				}
			}
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x00073B90 File Offset: 0x00071D90
		private void ClearObjects()
		{
			for (int i = 0; i < this.spawnedCubes.Count; i++)
			{
				Object.DestroyImmediate(this.spawnedCubes[i]);
			}
			this.spawnedCubes.Clear();
			GC.Collect();
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x00073BD4 File Offset: 0x00071DD4
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

		// Token: 0x04001735 RID: 5941
		private bool inMenu;

		// Token: 0x04001736 RID: 5942
		private const string ovrOverlayID = "OVROverlayID";

		// Token: 0x04001737 RID: 5943
		private const string applicationID = "ApplicationID";

		// Token: 0x04001738 RID: 5944
		private const string noneID = "NoneID";

		// Token: 0x04001739 RID: 5945
		private Toggle applicationRadioButton;

		// Token: 0x0400173A RID: 5946
		private Toggle noneRadioButton;

		// Token: 0x0400173B RID: 5947
		[Header("App vs Compositor Comparison Settings")]
		public GameObject mainCamera;

		// Token: 0x0400173C RID: 5948
		public GameObject uiCamera;

		// Token: 0x0400173D RID: 5949
		public GameObject uiGeoParent;

		// Token: 0x0400173E RID: 5950
		public GameObject worldspaceGeoParent;

		// Token: 0x0400173F RID: 5951
		public OVROverlay cameraRenderOverlay;

		// Token: 0x04001740 RID: 5952
		public OVROverlay renderingLabelOverlay;

		// Token: 0x04001741 RID: 5953
		public Texture applicationLabelTexture;

		// Token: 0x04001742 RID: 5954
		public Texture compositorLabelTexture;

		// Token: 0x04001743 RID: 5955
		[Header("Level Loading Sim Settings")]
		public GameObject prefabForLevelLoadSim;

		// Token: 0x04001744 RID: 5956
		public OVROverlay cubemapOverlay;

		// Token: 0x04001745 RID: 5957
		public OVROverlay loadingTextQuadOverlay;

		// Token: 0x04001746 RID: 5958
		public float distanceFromCamToLoadText;

		// Token: 0x04001747 RID: 5959
		public float cubeSpawnRadius;

		// Token: 0x04001748 RID: 5960
		public float heightBetweenItems;

		// Token: 0x04001749 RID: 5961
		public int numObjectsPerLevel;

		// Token: 0x0400174A RID: 5962
		public int numLevels;

		// Token: 0x0400174B RID: 5963
		public int numLoopsTrigger = 500000000;

		// Token: 0x0400174C RID: 5964
		private List<GameObject> spawnedCubes = new List<GameObject>();
	}
}
