using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using Oculus.Platform;
using Oculus.Platform.Models;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x020000D4 RID: 212
public class GorillaMetaReport : MonoBehaviour
{
	// Token: 0x060004AE RID: 1198 RVA: 0x0001DAB8 File Offset: 0x0001BCB8
	private void Awake()
	{
		this.closeButton = this.closeButtonObject.GetComponent<GorillaReportButton>();
		this.currentScoreboard = this.sourceScoreboard.GetComponent<GorillaScoreboardSpawner>().currentScoreboard;
		this.localPlayer.inOverlay = false;
		this.playerRB = this.localPlayer.GetComponent<Rigidbody>();
		Core.AsyncInitialize(null).OnComplete(delegate(Message<PlatformInitialize> message)
		{
			if (!message.IsError)
			{
				AbuseReport.SetReportButtonPressedNotificationCallback(new Message<string>.Callback(this.OnReportButtonIntentNotif));
			}
		});
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x0001DB21 File Offset: 0x0001BD21
	private void OnDisable()
	{
		this.localPlayer.inOverlay = false;
		base.StopAllCoroutines();
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x0001DB38 File Offset: 0x0001BD38
	private void OnReportButtonIntentNotif(Message<string> message)
	{
		if (message.IsError)
		{
			AbuseReport.ReportRequestHandled(ReportRequestResponse.Unhandled);
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			this.ReportText.SetActive(true);
			AbuseReport.ReportRequestHandled(ReportRequestResponse.Handled);
			this.StartOverlay();
			return;
		}
		if (!message.IsError)
		{
			AbuseReport.ReportRequestHandled(ReportRequestResponse.Handled);
			this.StartOverlay();
		}
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x0001DB8B File Offset: 0x0001BD8B
	private IEnumerator Submitted()
	{
		yield return new WaitForSeconds(1.5f);
		this.Teardown();
		yield break;
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x0001DB9A File Offset: 0x0001BD9A
	private IEnumerator LockoutButtonPress()
	{
		this.canPress = false;
		yield return new WaitForSeconds(0.75f);
		this.canPress = true;
		yield break;
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x0001DBAC File Offset: 0x0001BDAC
	private void DuplicateScoreboard(GameObject scoreboard)
	{
		this.newScoreboard = Object.Instantiate<GameObject>(scoreboard, base.transform.position, base.transform.rotation);
		this.currentScoreboard = this.newScoreboard.GetComponent<GorillaScoreboardSpawner>().currentScoreboard;
		this.newScoreboard.transform.localScale /= 1000f;
		this.newScoreboard.transform.SetParent(this.gorillaUI.transform);
		this.newScoreboard.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
		this.reportScoreboard.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x0001DC78 File Offset: 0x0001BE78
	private void HandToggle(bool state)
	{
		this.leftHandObject.SetActive(state);
		this.rightHandObject.SetActive(state);
		if (state)
		{
			this.savedLeftTransform = this.leftTriggerCollider.GetComponent<TransformFollow>().transformToFollow;
			this.savedRightTransform = this.rightTriggerCollider.GetComponent<TransformFollow>().transformToFollow;
			this.leftTriggerCollider.GetComponent<TransformFollow>().transformToFollow = this.leftFingerTip.transform;
			this.rightTriggerCollider.GetComponent<TransformFollow>().transformToFollow = this.rightFingerTip.transform;
		}
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x0001DD04 File Offset: 0x0001BF04
	private void ToggleLevelVisibility(bool state)
	{
		foreach (KeyValuePair<MeshRenderer, bool> keyValuePair in this.levelRenderers)
		{
			keyValuePair.Key.enabled = state;
		}
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x0001DD60 File Offset: 0x0001BF60
	private void GetLevelVisibility()
	{
		foreach (MeshRenderer meshRenderer in this.tempLevel.GetComponentsInChildren<MeshRenderer>())
		{
			if (meshRenderer.enabled)
			{
				this.levelRenderers.Add(meshRenderer, true);
			}
		}
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x0001DDA0 File Offset: 0x0001BFA0
	private void Teardown()
	{
		this.HandToggle(false);
		this.leftTriggerCollider.GetComponent<TransformFollow>().transformToFollow = this.savedLeftTransform;
		this.rightTriggerCollider.GetComponent<TransformFollow>().transformToFollow = this.savedRightTransform;
		this.ReportText.GetComponent<Text>().text = "NOT CURRENTLY CONNECTED TO A ROOM";
		this.ReportText.SetActive(false);
		this.localPlayer.inOverlay = false;
		this.localPlayer.disableMovement = false;
		this.closeButton.selected = false;
		this.closeButton.isOn = false;
		this.closeButton.UpdateColor();
		this.localPlayer.InReportMenu = false;
		this.ToggleLevelVisibility(true);
		this.levelRenderers.Clear();
		this.occluder.GetComponent<Renderer>().enabled = false;
		if (this.newScoreboard != null)
		{
			Object.Destroy(this.newScoreboard);
		}
		this.reportScoreboard.transform.position = Vector3.zero;
		foreach (object obj in this.reportScoreboard.transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(false);
			if (transform.GetComponent<Renderer>())
			{
				transform.GetComponent<Renderer>().enabled = false;
			}
		}
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x0001DF08 File Offset: 0x0001C108
	private void CheckReportSubmit()
	{
		if (this.currentScoreboard == null)
		{
			return;
		}
		foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
		{
			if (gorillaPlayerScoreboardLine.doneReporting)
			{
				this.ReportText.SetActive(true);
				this.ReportText.GetComponent<Text>().text = "REPORTED " + gorillaPlayerScoreboardLine.playerNameValue;
				this.newScoreboard.SetActive(false);
				base.StartCoroutine(this.Submitted());
			}
		}
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x0001DFB8 File Offset: 0x0001C1B8
	private void StartOverlay()
	{
		if (!PhotonNetwork.InRoom)
		{
			this.localPlayer.InReportMenu = true;
			this.localPlayer.disableMovement = true;
			this.localPlayer.inOverlay = true;
			this.positionStore = this.localPlayer.transform.position;
			this.occluder.GetComponent<Renderer>().enabled = true;
			this.ReportText.SetActive(true);
			foreach (object obj in this.reportScoreboard.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.GetComponent<Renderer>())
				{
					transform.gameObject.SetActive(true);
					transform.GetComponent<Renderer>().enabled = true;
				}
			}
			this.reportScoreboard.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
			this.GetLevelVisibility();
			this.ToggleLevelVisibility(false);
			this.rightHandObject.transform.SetPositionAndRotation(this.localPlayer.rightControllerTransform.position, this.localPlayer.rightControllerTransform.rotation);
			this.leftHandObject.transform.SetPositionAndRotation(this.localPlayer.leftControllerTransform.position, this.localPlayer.leftControllerTransform.rotation);
			this.HandToggle(true);
		}
		if (!this.localPlayer.InReportMenu && PhotonNetwork.InRoom)
		{
			this.localPlayer.InReportMenu = true;
			this.localPlayer.disableMovement = true;
			this.localPlayer.inOverlay = true;
			this.positionStore = this.localPlayer.transform.position;
			this.occluder.GetComponent<Renderer>().enabled = true;
			foreach (object obj2 in this.reportScoreboard.transform)
			{
				Transform transform2 = (Transform)obj2;
				if (transform2.GetComponent<Renderer>())
				{
					transform2.gameObject.SetActive(true);
					transform2.GetComponent<Renderer>().enabled = true;
				}
			}
			this.GetLevelVisibility();
			this.ToggleLevelVisibility(false);
			this.DuplicateScoreboard(this.sourceScoreboard);
			this.rightHandObject.transform.SetPositionAndRotation(this.localPlayer.rightControllerTransform.position, this.localPlayer.rightControllerTransform.rotation);
			this.leftHandObject.transform.SetPositionAndRotation(this.localPlayer.leftControllerTransform.position, this.localPlayer.leftControllerTransform.rotation);
			this.HandToggle(true);
		}
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x0001E280 File Offset: 0x0001C480
	private void CheckDistance()
	{
		float num = Vector3.Distance(this.reportScoreboard.transform.position, base.transform.position);
		float num2 = 1f;
		if (num > num2 && !this.isMoving)
		{
			this.isMoving = true;
			this.movementTime = 0f;
		}
		if (this.isMoving)
		{
			this.movementTime += Time.deltaTime;
			float num3 = Mathf.Clamp01(this.movementTime / 1f);
			this.reportScoreboard.transform.SetPositionAndRotation(Vector3.Lerp(this.reportScoreboard.transform.position, base.transform.position, num3), Quaternion.Lerp(this.reportScoreboard.transform.rotation, base.transform.rotation, num3));
			if (this.newScoreboard != null)
			{
				this.newScoreboard.transform.SetPositionAndRotation(Vector3.Lerp(this.newScoreboard.transform.position, base.transform.position, num3), Quaternion.Lerp(this.newScoreboard.transform.rotation, base.transform.rotation, num3));
			}
			if (num3 >= 1f)
			{
				this.isMoving = false;
				this.movementTime = 0f;
			}
		}
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x0001E3CC File Offset: 0x0001C5CC
	private void Update()
	{
		if (!this.canPress)
		{
			return;
		}
		this.leftController = ControllerInputPoller.instance.leftControllerDevice;
		this.leftController.TryGetFeatureValue(CommonUsages.menuButton, out this.menuButtonPress);
		this.menuButtonPress = SteamVR_Actions.gorillaTag_System.GetState(SteamVR_Input_Sources.LeftHand);
		if (this.menuButtonPress && this.localPlayer.InReportMenu)
		{
			this.Teardown();
			base.StartCoroutine(this.LockoutButtonPress());
		}
		if (this.localPlayer.InReportMenu)
		{
			this.localPlayer.inOverlay = true;
			this.occluder.transform.position = GorillaTagger.Instance.mainCamera.transform.position;
			this.rightHandObject.transform.SetPositionAndRotation(this.localPlayer.rightControllerTransform.position, this.localPlayer.rightControllerTransform.rotation);
			this.leftHandObject.transform.SetPositionAndRotation(this.localPlayer.leftControllerTransform.position, this.localPlayer.leftControllerTransform.rotation);
			this.CheckDistance();
			this.CheckReportSubmit();
		}
		if (this.closeButton.selected)
		{
			this.Teardown();
		}
		if (this.testPress)
		{
			this.testPress = false;
			this.StartOverlay();
		}
	}

	// Token: 0x04000559 RID: 1369
	[SerializeField]
	private GameObject occluder;

	// Token: 0x0400055A RID: 1370
	[SerializeField]
	private GameObject reportScoreboard;

	// Token: 0x0400055B RID: 1371
	[SerializeField]
	private GameObject sourceScoreboard;

	// Token: 0x0400055C RID: 1372
	[SerializeField]
	private GameObject ReportText;

	// Token: 0x0400055D RID: 1373
	[SerializeField]
	private GameObject gorillaUI;

	// Token: 0x0400055E RID: 1374
	[SerializeField]
	public GameObject tempLevel;

	// Token: 0x0400055F RID: 1375
	[SerializeField]
	private Player localPlayer;

	// Token: 0x04000560 RID: 1376
	[SerializeField]
	private GameObject closeButtonObject;

	// Token: 0x04000561 RID: 1377
	[SerializeField]
	private GameObject leftHandObject;

	// Token: 0x04000562 RID: 1378
	[SerializeField]
	private GameObject rightHandObject;

	// Token: 0x04000563 RID: 1379
	[SerializeField]
	private GameObject rightTriggerCollider;

	// Token: 0x04000564 RID: 1380
	[SerializeField]
	private GameObject leftTriggerCollider;

	// Token: 0x04000565 RID: 1381
	[SerializeField]
	private GameObject rightFingerTip;

	// Token: 0x04000566 RID: 1382
	[SerializeField]
	private GameObject leftFingerTip;

	// Token: 0x04000567 RID: 1383
	private GorillaReportButton closeButton;

	// Token: 0x04000568 RID: 1384
	private bool canPress = true;

	// Token: 0x04000569 RID: 1385
	private Dictionary<MeshRenderer, bool> levelRenderers = new Dictionary<MeshRenderer, bool>();

	// Token: 0x0400056A RID: 1386
	public GameObject buttons;

	// Token: 0x0400056B RID: 1387
	public GorillaScoreBoard currentScoreboard;

	// Token: 0x0400056C RID: 1388
	private Rigidbody playerRB;

	// Token: 0x0400056D RID: 1389
	private Transform savedLeftTransform;

	// Token: 0x0400056E RID: 1390
	private Transform savedRightTransform;

	// Token: 0x0400056F RID: 1391
	public bool menuButtonPress;

	// Token: 0x04000570 RID: 1392
	private GameObject newScoreboard;

	// Token: 0x04000571 RID: 1393
	private InputDevice leftController;

	// Token: 0x04000572 RID: 1394
	public bool testPress;

	// Token: 0x04000573 RID: 1395
	private Vector3 positionStore;

	// Token: 0x04000574 RID: 1396
	public float radius;

	// Token: 0x04000575 RID: 1397
	public float maxDistance;

	// Token: 0x04000576 RID: 1398
	public LayerMask layerMask;

	// Token: 0x04000577 RID: 1399
	public bool isMoving;

	// Token: 0x04000578 RID: 1400
	public float movementTime;
}
