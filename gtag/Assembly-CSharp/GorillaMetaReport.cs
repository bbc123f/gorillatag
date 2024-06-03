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
using Valve.VR;

public class GorillaMetaReport : MonoBehaviour
{
	private Player localPlayer
	{
		get
		{
			return Player.Instance;
		}
	}

	private void Start()
	{
		this.localPlayer.inOverlay = false;
		Core.AsyncInitialize(null).OnComplete(delegate(Message<PlatformInitialize> message)
		{
			if (!message.IsError)
			{
				AbuseReport.SetReportButtonPressedNotificationCallback(new Message<string>.Callback(this.OnReportButtonIntentNotif));
			}
		});
		base.gameObject.SetActive(false);
	}

	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
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

	private void DuplicateScoreboard()
	{
		this.currentScoreboard.gameObject.SetActive(true);
		if (GorillaScoreboardTotalUpdater.instance != null)
		{
			GorillaScoreboardTotalUpdater.instance.UpdateScoreboard(this.currentScoreboard);
		}
		Vector3 position;
		Quaternion rotation;
		Vector3 vector;
		this.GetIdealScreenPositionRotation(out position, out rotation, out vector);
		this.currentScoreboard.transform.SetPositionAndRotation(position, rotation);
		this.reportScoreboard.transform.SetPositionAndRotation(position, rotation);
	}

	private void ToggleLevelVisibility(bool state)
	{
		Camera component = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
		if (state)
		{
			component.cullingMask = this.savedCullingLayers;
			return;
		}
		this.savedCullingLayers = component.cullingMask;
		component.cullingMask = this.visibleLayers;
	}

	private void Teardown()
	{
		this.ReportText.GetComponent<Text>().text = "NOT CURRENTLY CONNECTED TO A ROOM";
		this.ReportText.SetActive(false);
		this.localPlayer.inOverlay = false;
		this.localPlayer.disableMovement = false;
		this.closeButton.selected = false;
		this.closeButton.isOn = false;
		this.closeButton.UpdateColor();
		this.localPlayer.InReportMenu = false;
		this.ToggleLevelVisibility(true);
		base.gameObject.SetActive(false);
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

	private void GetIdealScreenPositionRotation(out Vector3 position, out Quaternion rotation, out Vector3 scale)
	{
		GameObject mainCamera = GorillaTagger.Instance.mainCamera;
		rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
		scale = this.localPlayer.turnParent.transform.localScale;
		position = mainCamera.transform.position + rotation * this.playerLocalScreenPosition * scale.x;
	}

	private void StartOverlay()
	{
		Vector3 position;
		Quaternion rotation;
		Vector3 localScale;
		this.GetIdealScreenPositionRotation(out position, out rotation, out localScale);
		this.currentScoreboard.transform.localScale = localScale;
		this.reportScoreboard.transform.localScale = localScale;
		this.leftHandObject.transform.localScale = localScale;
		this.rightHandObject.transform.localScale = localScale;
		this.occluder.transform.localScale = localScale;
		if (this.localPlayer.InReportMenu && !PhotonNetwork.InRoom)
		{
			return;
		}
		this.localPlayer.InReportMenu = true;
		this.localPlayer.disableMovement = true;
		this.localPlayer.inOverlay = true;
		base.gameObject.SetActive(true);
		if (PhotonNetwork.InRoom)
		{
			this.DuplicateScoreboard();
		}
		else
		{
			this.ReportText.SetActive(true);
			this.reportScoreboard.transform.SetPositionAndRotation(position, rotation);
			this.currentScoreboard.transform.SetPositionAndRotation(position, rotation);
		}
		this.ToggleLevelVisibility(false);
		this.rightHandObject.transform.SetPositionAndRotation(this.localPlayer.rightControllerTransform.position, this.localPlayer.rightControllerTransform.rotation);
		this.leftHandObject.transform.SetPositionAndRotation(this.localPlayer.leftControllerTransform.position, this.localPlayer.leftControllerTransform.rotation);
	}

	private void CheckDistance()
	{
		Vector3 b;
		Quaternion b2;
		Vector3 vector;
		this.GetIdealScreenPositionRotation(out b, out b2, out vector);
		float num = Vector3.Distance(this.reportScoreboard.transform.position, b);
		float num2 = 1f;
		if (num > num2 && !this.isMoving)
		{
			this.isMoving = true;
			this.movementTime = 0f;
		}
		if (this.isMoving)
		{
			this.movementTime += Time.deltaTime;
			float num3 = this.movementTime;
			this.reportScoreboard.transform.SetPositionAndRotation(Vector3.Lerp(this.reportScoreboard.transform.position, b, num3), Quaternion.Lerp(this.reportScoreboard.transform.rotation, b2, num3));
			if (this.currentScoreboard != null)
			{
				this.currentScoreboard.transform.SetPositionAndRotation(Vector3.Lerp(this.currentScoreboard.transform.position, b, num3), Quaternion.Lerp(this.currentScoreboard.transform.rotation, b2, num3));
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
		if (this.blockButtonsUntilTimestamp > Time.time)
		{
			return;
		}
		if (SteamVR_Actions.gorillaTag_System.GetState(SteamVR_Input_Sources.LeftHand) && this.localPlayer.InReportMenu)
		{
			this.Teardown();
			this.blockButtonsUntilTimestamp = Time.time + 0.75f;
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
	private void <Start>b__14_0(Message<PlatformInitialize> message)
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
	private GameObject ReportText;

	[SerializeField]
	private LayerMask visibleLayers;

	[SerializeField]
	private GorillaReportButton closeButton;

	[SerializeField]
	private GameObject leftHandObject;

	[SerializeField]
	private GameObject rightHandObject;

	[SerializeField]
	private Vector3 playerLocalScreenPosition;

	private float blockButtonsUntilTimestamp;

	[SerializeField]
	private GorillaScoreBoard currentScoreboard;

	private int savedCullingLayers;

	public bool testPress;

	public bool isMoving;

	private float movementTime;

	[CompilerGenerated]
	private sealed class <Submitted>d__17 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <Submitted>d__17(int <>1__state)
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
