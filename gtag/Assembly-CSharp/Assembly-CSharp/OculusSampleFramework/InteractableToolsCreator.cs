using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	public class InteractableToolsCreator : MonoBehaviour
	{
		private void Awake()
		{
			if (this.LeftHandTools != null && this.LeftHandTools.Length != 0)
			{
				base.StartCoroutine(this.AttachToolsToHands(this.LeftHandTools, false));
			}
			if (this.RightHandTools != null && this.RightHandTools.Length != 0)
			{
				base.StartCoroutine(this.AttachToolsToHands(this.RightHandTools, true));
			}
		}

		private IEnumerator AttachToolsToHands(Transform[] toolObjects, bool isRightHand)
		{
			HandsManager handsManagerObj = null;
			while ((handsManagerObj = HandsManager.Instance) == null || !handsManagerObj.IsInitialized())
			{
				yield return null;
			}
			HashSet<Transform> hashSet = new HashSet<Transform>();
			foreach (Transform transform in toolObjects)
			{
				hashSet.Add(transform.transform);
			}
			foreach (Transform toolObject in hashSet)
			{
				OVRSkeleton handSkeletonToAttachTo = isRightHand ? handsManagerObj.RightHandSkeleton : handsManagerObj.LeftHandSkeleton;
				while (handSkeletonToAttachTo == null || handSkeletonToAttachTo.Bones == null)
				{
					yield return null;
				}
				this.AttachToolToHandTransform(toolObject, isRightHand);
				handSkeletonToAttachTo = null;
				toolObject = null;
			}
			HashSet<Transform>.Enumerator enumerator = default(HashSet<Transform>.Enumerator);
			yield break;
			yield break;
		}

		private void AttachToolToHandTransform(Transform tool, bool isRightHanded)
		{
			Transform transform = Object.Instantiate<Transform>(tool).transform;
			transform.localPosition = Vector3.zero;
			InteractableTool component = transform.GetComponent<InteractableTool>();
			component.IsRightHandedTool = isRightHanded;
			component.Initialize();
		}

		[SerializeField]
		private Transform[] LeftHandTools;

		[SerializeField]
		private Transform[] RightHandTools;
	}
}
