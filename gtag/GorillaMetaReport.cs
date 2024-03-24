using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using Oculus.Platform;
using Oculus.Platform.Models;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Valve.VR;

public class GorillaMetaReport : MonoBehaviour
{
	private void Awake()
	{
		this.closeButton = this.closeButtonObject.GetComponent<GorillaReportButton>();
		this.currentScoreboard = this.sourceScoreboard.GetComponentInChildren<GorillaScoreBoard>();
		this.currentScoreboard.gameObject.SetActive(false);
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

	private void OnDisable()
	{
		this.localPlayer.inOverlay = false;
		base.StopAllCoroutines();
	}

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

	private IEnumerator Submitted()
	{
		yield return new WaitForSeconds(1.5f);
		this.Teardown();
		yield break;
	}

	private IEnumerator LockoutButtonPress()
	{
		this.canPress = false;
		yield return new WaitForSeconds(0.75f);
		this.canPress = true;
		yield break;
	}

	private void DuplicateScoreboard(GameObject scoreboard)
	{
		this.currentScoreboard.gameObject.SetActive(true);
		if (GorillaScoreboardTotalUpdater.instance != null)
		{
			GorillaScoreboardTotalUpdater.instance.UpdateScoreboard(this.currentScoreboard);
		}
		this.currentScoreboard.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
		this.reportScoreboard.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
	}

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

	private void ToggleLevelVisibility(bool state)
	{
		for (int i = 0; i < this.levelRenderers.Count; i++)
		{
			MeshRenderer meshRenderer = this.levelRenderers[i];
			if (!(meshRenderer == null))
			{
				meshRenderer.enabled = state;
			}
		}
	}

	private void GetLevelVisibility()
	{
		this.levelRenderers.Clear();
		ZoneRootRegister[] array = Object.FindObjectsOfType<ZoneRootRegister>();
		for (int i = 0; i < array.Length; i++)
		{
			foreach (MeshRenderer meshRenderer in array[i].GetComponentsInChildren<MeshRenderer>())
			{
				if (meshRenderer.enabled)
				{
					this.levelRenderers.Add(meshRenderer);
				}
			}
		}
	}

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
		this.reportScoreboard.transform.position = Vector3.zero;
		this.currentScoreboard.gameObject.SetActive(false);
		foreach (object obj in this.reportScoreboard.transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(false);
			if (transform.GetComponent<Renderer>())
			{
				transform.GetComponent<Renderer>().enabled = false;
			}
		}
		foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
		{
			gorillaPlayerScoreboardLine.doneReporting = false;
		}
		GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
	}

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
				this.currentScoreboard.gameObject.SetActive(false);
				base.StartCoroutine(this.Submitted());
			}
		}
	}

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
			this.currentScoreboard.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
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
			if (this.currentScoreboard != null)
			{
				this.currentScoreboard.transform.SetPositionAndRotation(Vector3.Lerp(this.currentScoreboard.transform.position, base.transform.position, num3), Quaternion.Lerp(this.currentScoreboard.transform.rotation, base.transform.rotation, num3));
			}
			if (num3 >= 1f)
			{
				this.isMoving = false;
				this.movementTime = 0f;
			}
		}
	}

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

	public GorillaMetaReport()
	{
	}

	[CompilerGenerated]
	private void <Awake>b__27_0(Message<PlatformInitialize> message)
	{
		if (!message.IsError)
		{
			AbuseReport.SetReportButtonPressedNotificationCallback(new Message<string>.Callback(this.OnReportButtonIntentNotif));
		}
	}

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

	private List<MeshRenderer> levelRenderers = new List<MeshRenderer>(30000);

	public GameObject buttons;

	public GorillaScoreBoard currentScoreboard;

	private Rigidbody playerRB;

	private Transform savedLeftTransform;

	private Transform savedRightTransform;

	public bool menuButtonPress;

	private GameObject newScoreboard;

	private InputDevice leftController;

	public bool testPress;

	private Vector3 positionStore;

	public float radius;

	public float maxDistance;

	public LayerMask layerMask;

	public bool isMoving;

	public float movementTime;

	[CompilerGenerated]
	private sealed class <LockoutButtonPress>d__31 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <LockoutButtonPress>d__31(int <>1__state)
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
			GorillaMetaReport gorillaMetaReport = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				gorillaMetaReport.canPress = false;
				this.<>2__current = new WaitForSeconds(0.75f);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			gorillaMetaReport.canPress = true;
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

		public GorillaMetaReport <>4__this;
	}

	[CompilerGenerated]
	private sealed class <Submitted>d__30 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <Submitted>d__30(int <>1__state)
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
			GorillaMetaReport gorillaMetaReport = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				this.<>2__current = new WaitForSeconds(1.5f);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			gorillaMetaReport.Teardown();
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

		public GorillaMetaReport <>4__this;
	}
}
