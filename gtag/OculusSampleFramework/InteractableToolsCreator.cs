using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework;

public class InteractableToolsCreator : MonoBehaviour
{
	[SerializeField]
	private Transform[] LeftHandTools;

	[SerializeField]
	private Transform[] RightHandTools;

	private void Awake()
	{
		if (LeftHandTools != null && LeftHandTools.Length != 0)
		{
			StartCoroutine(AttachToolsToHands(LeftHandTools, isRightHand: false));
		}
		if (RightHandTools != null && RightHandTools.Length != 0)
		{
			StartCoroutine(AttachToolsToHands(RightHandTools, isRightHand: true));
		}
	}

	private IEnumerator AttachToolsToHands(Transform[] toolObjects, bool isRightHand)
	{
		HandsManager handsManagerObj;
		while (true)
		{
			HandsManager instance;
			handsManagerObj = (instance = HandsManager.Instance);
			if (!(instance == null) && handsManagerObj.IsInitialized())
			{
				break;
			}
			yield return null;
		}
		HashSet<Transform> hashSet = new HashSet<Transform>();
		foreach (Transform transform in toolObjects)
		{
			hashSet.Add(transform.transform);
		}
		foreach (Transform toolObject in hashSet)
		{
			OVRSkeleton handSkeletonToAttachTo = (isRightHand ? handsManagerObj.RightHandSkeleton : handsManagerObj.LeftHandSkeleton);
			while (handSkeletonToAttachTo == null || handSkeletonToAttachTo.Bones == null)
			{
				yield return null;
			}
			AttachToolToHandTransform(toolObject, isRightHand);
		}
	}

	private void AttachToolToHandTransform(Transform tool, bool isRightHanded)
	{
		Transform obj = Object.Instantiate(tool).transform;
		obj.localPosition = Vector3.zero;
		InteractableTool component = obj.GetComponent<InteractableTool>();
		component.IsRightHandedTool = isRightHanded;
		component.Initialize();
	}
}
