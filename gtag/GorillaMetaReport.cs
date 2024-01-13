using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using Oculus.Platform;
using Oculus.Platform.Models;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class GorillaMetaReport : MonoBehaviour
{
	[SerializeField]
	private GameObject occluder;

	[SerializeField]
	private GameObject reportScoreboard;

	[SerializeField]
	private GameObject sourceScoreboard;

	[SerializeField]
	private GameObject ReportText;

	[SerializeField]
	private GameObject gorillaUI;

	[SerializeField]
	public GameObject tempLevel;

	[SerializeField]
	private Player localPlayer;

	[SerializeField]
	private GameObject closeButtonObject;

	[SerializeField]
	private GameObject leftHandObject;

	[SerializeField]
	private GameObject rightHandObject;

	[SerializeField]
	private GameObject rightTriggerCollider;

	[SerializeField]
	private GameObject leftTriggerCollider;

	[SerializeField]
	private GameObject rightFingerTip;

	[SerializeField]
	private GameObject leftFingerTip;

	private GorillaReportButton closeButton;

	private bool canPress = true;

	private Dictionary<MeshRenderer, bool> levelRenderers = new Dictionary<MeshRenderer, bool>();

	public GameObject buttons;

	public GorillaScoreBoard currentScoreboard;

	private Rigidbody playerRB;

	private Transform savedLeftTransform;

	private Transform savedRightTransform;

	public bool menuButtonPress;

	private GameObject newScoreboard;

	private InputDevice leftController;

	private Vector3 positionStore;

	public float radius;

	public float maxDistance;

	public LayerMask layerMask;

	public bool isMoving;

	public float movementTime;

	private void Awake()
	{
		closeButton = closeButtonObject.GetComponent<GorillaReportButton>();
		currentScoreboard = sourceScoreboard.GetComponent<GorillaScoreboardSpawner>().currentScoreboard;
		localPlayer.inOverlay = false;
		playerRB = localPlayer.GetComponent<Rigidbody>();
		Core.AsyncInitialize().OnComplete(delegate(Message<PlatformInitialize> message)
		{
			if (!message.IsError)
			{
				AbuseReport.SetReportButtonPressedNotificationCallback(OnReportButtonIntentNotif);
			}
		});
	}

	private void OnDisable()
	{
		localPlayer.inOverlay = false;
		StopAllCoroutines();
	}

	private void OnReportButtonIntentNotif(Message<string> message)
	{
		if (message.IsError)
		{
			AbuseReport.ReportRequestHandled(ReportRequestResponse.Unhandled);
		}
		else if (!PhotonNetwork.InRoom)
		{
			ReportText.SetActive(value: true);
			AbuseReport.ReportRequestHandled(ReportRequestResponse.Handled);
			StartOverlay();
		}
		else if (!message.IsError)
		{
			AbuseReport.ReportRequestHandled(ReportRequestResponse.Handled);
			StartOverlay();
		}
	}

	private IEnumerator Submitted()
	{
		yield return new WaitForSeconds(1.5f);
		Teardown();
	}

	private IEnumerator LockoutButtonPress()
	{
		canPress = false;
		yield return new WaitForSeconds(0.75f);
		canPress = true;
	}

	private void DuplicateScoreboard(GameObject scoreboard)
	{
		newScoreboard = Object.Instantiate(scoreboard, base.transform.position, base.transform.rotation);
		currentScoreboard = newScoreboard.GetComponent<GorillaScoreboardSpawner>().currentScoreboard;
		newScoreboard.transform.localScale /= 1000f;
		newScoreboard.transform.SetParent(gorillaUI.transform);
		newScoreboard.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
		reportScoreboard.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
	}

	private void HandToggle(bool state)
	{
		leftHandObject.SetActive(state);
		rightHandObject.SetActive(state);
		if (state)
		{
			savedLeftTransform = leftTriggerCollider.GetComponent<TransformFollow>().transformToFollow;
			savedRightTransform = rightTriggerCollider.GetComponent<TransformFollow>().transformToFollow;
			leftTriggerCollider.GetComponent<TransformFollow>().transformToFollow = leftFingerTip.transform;
			rightTriggerCollider.GetComponent<TransformFollow>().transformToFollow = rightFingerTip.transform;
		}
	}

	private void ToggleLevelVisibility(bool state)
	{
		foreach (KeyValuePair<MeshRenderer, bool> levelRenderer in levelRenderers)
		{
			levelRenderer.Key.enabled = state;
		}
	}

	private void GetLevelVisibility()
	{
		MeshRenderer[] componentsInChildren = tempLevel.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			if (meshRenderer.enabled)
			{
				levelRenderers.Add(meshRenderer, value: true);
			}
		}
	}

	private void Teardown()
	{
		HandToggle(state: false);
		leftTriggerCollider.GetComponent<TransformFollow>().transformToFollow = savedLeftTransform;
		rightTriggerCollider.GetComponent<TransformFollow>().transformToFollow = savedRightTransform;
		ReportText.GetComponent<Text>().text = "NOT CURRENTLY CONNECTED TO A ROOM";
		ReportText.SetActive(value: false);
		localPlayer.inOverlay = false;
		localPlayer.disableMovement = false;
		closeButton.selected = false;
		closeButton.isOn = false;
		closeButton.UpdateColor();
		localPlayer.InReportMenu = false;
		ToggleLevelVisibility(state: true);
		levelRenderers.Clear();
		occluder.GetComponent<Renderer>().enabled = false;
		if (newScoreboard != null)
		{
			Object.Destroy(newScoreboard);
		}
		reportScoreboard.transform.position = Vector3.zero;
		foreach (Transform item in reportScoreboard.transform)
		{
			item.gameObject.SetActive(value: false);
			if ((bool)item.GetComponent<Renderer>())
			{
				item.GetComponent<Renderer>().enabled = false;
			}
		}
	}

	private void CheckReportSubmit()
	{
		if (currentScoreboard == null)
		{
			return;
		}
		foreach (GorillaPlayerScoreboardLine line in currentScoreboard.lines)
		{
			if (line.doneReporting)
			{
				ReportText.SetActive(value: true);
				ReportText.GetComponent<Text>().text = "REPORTED " + line.playerNameValue;
				newScoreboard.SetActive(value: false);
				StartCoroutine(Submitted());
			}
		}
	}

	private void StartOverlay()
	{
		if (!PhotonNetwork.InRoom)
		{
			localPlayer.InReportMenu = true;
			localPlayer.disableMovement = true;
			localPlayer.inOverlay = true;
			positionStore = localPlayer.transform.position;
			occluder.GetComponent<Renderer>().enabled = true;
			ReportText.SetActive(value: true);
			foreach (Transform item in reportScoreboard.transform)
			{
				if ((bool)item.GetComponent<Renderer>())
				{
					item.gameObject.SetActive(value: true);
					item.GetComponent<Renderer>().enabled = true;
				}
			}
			reportScoreboard.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
			GetLevelVisibility();
			ToggleLevelVisibility(state: false);
			rightHandObject.transform.SetPositionAndRotation(localPlayer.rightControllerTransform.position, localPlayer.rightControllerTransform.rotation);
			leftHandObject.transform.SetPositionAndRotation(localPlayer.leftControllerTransform.position, localPlayer.leftControllerTransform.rotation);
			HandToggle(state: true);
		}
		if (localPlayer.InReportMenu || !PhotonNetwork.InRoom)
		{
			return;
		}
		localPlayer.InReportMenu = true;
		localPlayer.disableMovement = true;
		localPlayer.inOverlay = true;
		positionStore = localPlayer.transform.position;
		occluder.GetComponent<Renderer>().enabled = true;
		foreach (Transform item2 in reportScoreboard.transform)
		{
			if ((bool)item2.GetComponent<Renderer>())
			{
				item2.gameObject.SetActive(value: true);
				item2.GetComponent<Renderer>().enabled = true;
			}
		}
		GetLevelVisibility();
		ToggleLevelVisibility(state: false);
		DuplicateScoreboard(sourceScoreboard);
		rightHandObject.transform.SetPositionAndRotation(localPlayer.rightControllerTransform.position, localPlayer.rightControllerTransform.rotation);
		leftHandObject.transform.SetPositionAndRotation(localPlayer.leftControllerTransform.position, localPlayer.leftControllerTransform.rotation);
		HandToggle(state: true);
	}

	private void CheckDistance()
	{
		float num = Vector3.Distance(reportScoreboard.transform.position, base.transform.position);
		float num2 = 1f;
		if (num > num2 && !isMoving)
		{
			isMoving = true;
			movementTime = 0f;
		}
		if (isMoving)
		{
			movementTime += Time.deltaTime;
			float num3 = Mathf.Clamp01(movementTime / 1f);
			reportScoreboard.transform.SetPositionAndRotation(Vector3.Lerp(reportScoreboard.transform.position, base.transform.position, num3), Quaternion.Lerp(reportScoreboard.transform.rotation, base.transform.rotation, num3));
			if (newScoreboard != null)
			{
				newScoreboard.transform.SetPositionAndRotation(Vector3.Lerp(newScoreboard.transform.position, base.transform.position, num3), Quaternion.Lerp(newScoreboard.transform.rotation, base.transform.rotation, num3));
			}
			if (num3 >= 1f)
			{
				isMoving = false;
				movementTime = 0f;
			}
		}
	}

	private void Update()
	{
		if (canPress)
		{
			leftController = ControllerInputPoller.instance.leftControllerDevice;
			leftController.TryGetFeatureValue(CommonUsages.menuButton, out menuButtonPress);
			if (menuButtonPress && localPlayer.InReportMenu)
			{
				Teardown();
				StartCoroutine(LockoutButtonPress());
			}
			if (localPlayer.InReportMenu)
			{
				localPlayer.inOverlay = true;
				occluder.transform.position = GorillaTagger.Instance.mainCamera.transform.position;
				rightHandObject.transform.SetPositionAndRotation(localPlayer.rightControllerTransform.position, localPlayer.rightControllerTransform.rotation);
				leftHandObject.transform.SetPositionAndRotation(localPlayer.leftControllerTransform.position, localPlayer.leftControllerTransform.rotation);
				CheckDistance();
				CheckReportSubmit();
			}
			if (closeButton.selected)
			{
				Teardown();
			}
		}
	}
}
