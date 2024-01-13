using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework;

public class InteractableToolsInputRouter : MonoBehaviour
{
	private static InteractableToolsInputRouter _instance;

	private bool _leftPinch;

	private bool _rightPinch;

	private HashSet<InteractableTool> _leftHandNearTools = new HashSet<InteractableTool>();

	private HashSet<InteractableTool> _leftHandFarTools = new HashSet<InteractableTool>();

	private HashSet<InteractableTool> _rightHandNearTools = new HashSet<InteractableTool>();

	private HashSet<InteractableTool> _rightHandFarTools = new HashSet<InteractableTool>();

	public static InteractableToolsInputRouter Instance
	{
		get
		{
			if (_instance == null)
			{
				InteractableToolsInputRouter[] array = Object.FindObjectsOfType<InteractableToolsInputRouter>();
				if (array.Length != 0)
				{
					_instance = array[0];
					for (int i = 1; i < array.Length; i++)
					{
						Object.Destroy(array[i].gameObject);
					}
				}
			}
			return _instance;
		}
	}

	public void RegisterInteractableTool(InteractableTool interactableTool)
	{
		if (interactableTool.IsRightHandedTool)
		{
			if (interactableTool.IsFarFieldTool)
			{
				_rightHandFarTools.Add(interactableTool);
			}
			else
			{
				_rightHandNearTools.Add(interactableTool);
			}
		}
		else if (interactableTool.IsFarFieldTool)
		{
			_leftHandFarTools.Add(interactableTool);
		}
		else
		{
			_leftHandNearTools.Add(interactableTool);
		}
	}

	public void UnregisterInteractableTool(InteractableTool interactableTool)
	{
		if (interactableTool.IsRightHandedTool)
		{
			if (interactableTool.IsFarFieldTool)
			{
				_rightHandFarTools.Remove(interactableTool);
			}
			else
			{
				_rightHandNearTools.Remove(interactableTool);
			}
		}
		else if (interactableTool.IsFarFieldTool)
		{
			_leftHandFarTools.Remove(interactableTool);
		}
		else
		{
			_leftHandNearTools.Remove(interactableTool);
		}
	}

	private void Update()
	{
		if (HandsManager.Instance.IsInitialized())
		{
			bool flag = HandsManager.Instance.LeftHand.IsTracked && HandsManager.Instance.LeftHand.HandConfidence == OVRHand.TrackingConfidence.High;
			bool flag2 = HandsManager.Instance.RightHand.IsTracked && HandsManager.Instance.RightHand.HandConfidence == OVRHand.TrackingConfidence.High;
			bool isPointerPoseValid = HandsManager.Instance.LeftHand.IsPointerPoseValid;
			bool isPointerPoseValid2 = HandsManager.Instance.RightHand.IsPointerPoseValid;
			bool flag3 = UpdateToolsAndEnableState(_leftHandNearTools, flag);
			UpdateToolsAndEnableState(_leftHandFarTools, !flag3 && flag && isPointerPoseValid);
			bool flag4 = UpdateToolsAndEnableState(_rightHandNearTools, flag2);
			UpdateToolsAndEnableState(_rightHandFarTools, !flag4 && flag2 && isPointerPoseValid2);
		}
	}

	private bool UpdateToolsAndEnableState(HashSet<InteractableTool> tools, bool toolsAreEnabledThisFrame)
	{
		bool result = UpdateTools(tools, !toolsAreEnabledThisFrame);
		ToggleToolsEnableState(tools, toolsAreEnabledThisFrame);
		return result;
	}

	private bool UpdateTools(HashSet<InteractableTool> tools, bool resetCollisionData = false)
	{
		bool flag = false;
		foreach (InteractableTool tool in tools)
		{
			List<InteractableCollisionInfo> nextIntersectingObjects = tool.GetNextIntersectingObjects();
			if (nextIntersectingObjects.Count > 0 && !resetCollisionData)
			{
				if (!flag)
				{
					flag = nextIntersectingObjects.Count > 0;
				}
				tool.UpdateCurrentCollisionsBasedOnDepth();
				if (tool.IsFarFieldTool)
				{
					KeyValuePair<Interactable, InteractableCollisionInfo> firstCurrentCollisionInfo = tool.GetFirstCurrentCollisionInfo();
					if (tool.ToolInputState == ToolInputState.PrimaryInputUp)
					{
						firstCurrentCollisionInfo.Value.InteractableCollider = firstCurrentCollisionInfo.Key.ActionCollider;
						firstCurrentCollisionInfo.Value.CollisionDepth = InteractableCollisionDepth.Action;
					}
					else
					{
						firstCurrentCollisionInfo.Value.InteractableCollider = firstCurrentCollisionInfo.Key.ContactCollider;
						firstCurrentCollisionInfo.Value.CollisionDepth = InteractableCollisionDepth.Contact;
					}
					tool.FocusOnInteractable(firstCurrentCollisionInfo.Key, firstCurrentCollisionInfo.Value.InteractableCollider);
				}
			}
			else
			{
				tool.DeFocus();
				tool.ClearAllCurrentCollisionInfos();
			}
			tool.UpdateLatestCollisionData();
		}
		return flag;
	}

	private void ToggleToolsEnableState(HashSet<InteractableTool> tools, bool enableState)
	{
		foreach (InteractableTool tool in tools)
		{
			if (tool.EnableState != enableState)
			{
				tool.EnableState = enableState;
			}
		}
	}
}
