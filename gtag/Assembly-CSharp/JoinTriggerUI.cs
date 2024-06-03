using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

public class JoinTriggerUI : MonoBehaviour
{
	private void Awake()
	{
		this.joinTriggerRef.TryResolve<GorillaNetworkJoinTrigger>(out this.joinTrigger);
		this.milestoneRendererRef.TryResolve<MeshRenderer>(out this.milestoneRenderer);
		this.screenBGRendererRef.TryResolve<MeshRenderer>(out this.screenBGRenderer);
		this.screenTextRef.TryResolve<Text>(out this.screenText);
	}

	private void Start()
	{
		this.didStart = true;
		this.OnEnable();
	}

	private void OnEnable()
	{
		if (this.didStart)
		{
			this.joinTrigger.RegisterUI(this);
		}
	}

	private void OnDisable()
	{
		this.joinTrigger.UnregisterUI(this);
	}

	public void SetState(JoinTriggerVisualState state, Func<string> oldRoom, Func<string> newRoom)
	{
		switch (state)
		{
		case JoinTriggerVisualState.ConnectionError:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_Error;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_Error;
			this.screenText.text = (this.template.showFullErrorMessages ? GorillaScoreboardTotalUpdater.instance.offlineTextErrorString : this.template.ScreenText_Error);
			return;
		case JoinTriggerVisualState.AlreadyInRoom:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_AlreadyInRoom;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_AlreadyInRoom;
			this.screenText.text = this.template.ScreenText_AlreadyInRoom.GetText(oldRoom, newRoom);
			return;
		case JoinTriggerVisualState.InPrivateRoom:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_InPrivateRoom;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_InPrivateRoom;
			this.screenText.text = this.template.ScreenText_InPrivateRoom.GetText(oldRoom, newRoom);
			return;
		case JoinTriggerVisualState.NotConnectedSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_NotConnectedSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_NotConnectedSoloJoin;
			this.screenText.text = this.template.ScreenText_NotConnectedSoloJoin.GetText(oldRoom, newRoom);
			return;
		case JoinTriggerVisualState.LeaveRoomAndSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_LeaveRoomAndSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_LeaveRoomAndSoloJoin;
			this.screenText.text = this.template.ScreenText_LeaveRoomAndSoloJoin.GetText(oldRoom, newRoom);
			return;
		case JoinTriggerVisualState.LeaveRoomAndPartyJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_LeaveRoomAndGroupJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_LeaveRoomAndGroupJoin;
			this.screenText.text = this.template.ScreenText_LeaveRoomAndGroupJoin.GetText(oldRoom, newRoom);
			return;
		case JoinTriggerVisualState.AbandonPartyAndSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_AbandonPartyAndSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_AbandonPartyAndSoloJoin;
			this.screenText.text = this.template.ScreenText_AbandonPartyAndSoloJoin.GetText(oldRoom, newRoom);
			return;
		default:
			return;
		}
	}

	public JoinTriggerUI()
	{
	}

	[SerializeField]
	private XSceneRef joinTriggerRef;

	private GorillaNetworkJoinTrigger joinTrigger;

	[SerializeField]
	private XSceneRef milestoneRendererRef;

	private MeshRenderer milestoneRenderer;

	[SerializeField]
	private XSceneRef screenBGRendererRef;

	private MeshRenderer screenBGRenderer;

	[SerializeField]
	private XSceneRef screenTextRef;

	private Text screenText;

	[SerializeField]
	private JoinTriggerUITemplate template;

	private bool didStart;
}
