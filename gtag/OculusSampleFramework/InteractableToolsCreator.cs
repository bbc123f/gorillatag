using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
				OVRSkeleton handSkeletonToAttachTo = (isRightHand ? handsManagerObj.RightHandSkeleton : handsManagerObj.LeftHandSkeleton);
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

		public InteractableToolsCreator()
		{
		}

		[SerializeField]
		private Transform[] LeftHandTools;

		[SerializeField]
		private Transform[] RightHandTools;

		[CompilerGenerated]
		private sealed class <AttachToolsToHands>d__3 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <AttachToolsToHands>d__3(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
				int num = this.<>1__state;
				if (num == -3 || num == 2)
				{
					try
					{
					}
					finally
					{
						this.<>m__Finally1();
					}
				}
			}

			bool IEnumerator.MoveNext()
			{
				bool flag;
				try
				{
					int num = this.<>1__state;
					InteractableToolsCreator interactableToolsCreator = this;
					switch (num)
					{
					case 0:
						this.<>1__state = -1;
						handsManagerObj = null;
						break;
					case 1:
						this.<>1__state = -1;
						break;
					case 2:
						this.<>1__state = -3;
						goto IL_11A;
					default:
						return false;
					}
					if (!((handsManagerObj = HandsManager.Instance) == null) && handsManagerObj.IsInitialized())
					{
						HashSet<Transform> hashSet = new HashSet<Transform>();
						foreach (Transform transform in toolObjects)
						{
							hashSet.Add(transform.transform);
						}
						enumerator = hashSet.GetEnumerator();
						this.<>1__state = -3;
						goto IL_155;
					}
					this.<>2__current = null;
					this.<>1__state = 1;
					return true;
					IL_11A:
					if (handSkeletonToAttachTo == null || handSkeletonToAttachTo.Bones == null)
					{
						this.<>2__current = null;
						this.<>1__state = 2;
						return true;
					}
					interactableToolsCreator.AttachToolToHandTransform(toolObject, isRightHand);
					handSkeletonToAttachTo = null;
					toolObject = null;
					IL_155:
					if (enumerator.MoveNext())
					{
						toolObject = enumerator.Current;
						handSkeletonToAttachTo = (isRightHand ? handsManagerObj.RightHandSkeleton : handsManagerObj.LeftHandSkeleton);
						goto IL_11A;
					}
					this.<>m__Finally1();
					enumerator = default(HashSet<Transform>.Enumerator);
					flag = false;
				}
				catch
				{
					this.System.IDisposable.Dispose();
					throw;
				}
				return flag;
			}

			private void <>m__Finally1()
			{
				this.<>1__state = -1;
				((IDisposable)enumerator).Dispose();
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

			public Transform[] toolObjects;

			public bool isRightHand;

			public InteractableToolsCreator <>4__this;

			private HandsManager <handsManagerObj>5__2;

			private HashSet<Transform>.Enumerator <>7__wrap2;

			private Transform <toolObject>5__4;

			private OVRSkeleton <handSkeletonToAttachTo>5__5;
		}
	}
}
