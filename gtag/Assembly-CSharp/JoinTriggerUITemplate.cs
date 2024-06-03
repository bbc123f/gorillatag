using System;
using UnityEngine;

[CreateAssetMenu(fileName = "JoinTriggerUITemplate", menuName = "ScriptableObjects/JoinTriggerUITemplate")]
public class JoinTriggerUITemplate : ScriptableObject
{
	public JoinTriggerUITemplate()
	{
	}

	public Material Milestone_Error;

	public Material Milestone_AlreadyInRoom;

	public Material Milestone_InPrivateRoom;

	public Material Milestone_NotConnectedSoloJoin;

	public Material Milestone_LeaveRoomAndSoloJoin;

	public Material Milestone_LeaveRoomAndGroupJoin;

	public Material Milestone_AbandonPartyAndSoloJoin;

	public Material ScreenBG_Error;

	public Material ScreenBG_AlreadyInRoom;

	public Material ScreenBG_InPrivateRoom;

	public Material ScreenBG_NotConnectedSoloJoin;

	public Material ScreenBG_LeaveRoomAndSoloJoin;

	public Material ScreenBG_LeaveRoomAndGroupJoin;

	public Material ScreenBG_AbandonPartyAndSoloJoin;

	public string ScreenText_Error;

	public bool showFullErrorMessages;

	public JoinTriggerUITemplate.FormattedString ScreenText_AlreadyInRoom;

	public JoinTriggerUITemplate.FormattedString ScreenText_InPrivateRoom;

	public JoinTriggerUITemplate.FormattedString ScreenText_NotConnectedSoloJoin;

	public JoinTriggerUITemplate.FormattedString ScreenText_LeaveRoomAndSoloJoin;

	public JoinTriggerUITemplate.FormattedString ScreenText_LeaveRoomAndGroupJoin;

	public JoinTriggerUITemplate.FormattedString ScreenText_AbandonPartyAndSoloJoin;

	[Serializable]
	public struct FormattedString
	{
		public string GetText(string oldRoom, string newRoom)
		{
			if (this.formatter == null)
			{
				this.formatter = StringFormatter.Parse(this.formatText);
			}
			return this.formatter.Format(oldRoom, newRoom);
		}

		public string GetText(Func<string> oldRoom, Func<string> newRoom)
		{
			if (this.formatter == null)
			{
				this.formatter = StringFormatter.Parse(this.formatText);
			}
			return this.formatter.Format(oldRoom, newRoom);
		}

		[TextArea]
		[SerializeField]
		private string formatText;

		[NonSerialized]
		private StringFormatter formatter;
	}
}
