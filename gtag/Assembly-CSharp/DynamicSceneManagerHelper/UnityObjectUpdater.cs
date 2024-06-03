using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace DynamicSceneManagerHelper
{
	internal class UnityObjectUpdater
	{
		public Task<GameObject> CreateUnityObject(OVRAnchor anchor, GameObject parent)
		{
			UnityObjectUpdater.<CreateUnityObject>d__0 <CreateUnityObject>d__;
			<CreateUnityObject>d__.<>t__builder = AsyncTaskMethodBuilder<GameObject>.Create();
			<CreateUnityObject>d__.anchor = anchor;
			<CreateUnityObject>d__.parent = parent;
			<CreateUnityObject>d__.<>1__state = -1;
			<CreateUnityObject>d__.<>t__builder.Start<UnityObjectUpdater.<CreateUnityObject>d__0>(ref <CreateUnityObject>d__);
			return <CreateUnityObject>d__.<>t__builder.Task;
		}

		public void UpdateUnityObject(OVRAnchor anchor, GameObject gameObject)
		{
			SceneManagerHelper sceneManagerHelper = new SceneManagerHelper(gameObject);
			OVRLocatable locatable;
			if (anchor.TryGetComponent<OVRLocatable>(out locatable))
			{
				sceneManagerHelper.SetLocation(locatable, null);
			}
			OVRBounded2D bounds;
			if (anchor.TryGetComponent<OVRBounded2D>(out bounds) && bounds.IsEnabled)
			{
				sceneManagerHelper.UpdatePlane(bounds);
			}
			OVRBounded3D bounds2;
			if (anchor.TryGetComponent<OVRBounded3D>(out bounds2) && bounds2.IsEnabled)
			{
				sceneManagerHelper.UpdateVolume(bounds2);
			}
			OVRTriangleMesh mesh;
			if (anchor.TryGetComponent<OVRTriangleMesh>(out mesh) && mesh.IsEnabled)
			{
				sceneManagerHelper.UpdateMesh(mesh);
			}
		}

		public UnityObjectUpdater()
		{
		}

		[CompilerGenerated]
		[StructLayout(LayoutKind.Auto)]
		private struct <CreateUnityObject>d__0 : IAsyncStateMachine
		{
			void IAsyncStateMachine.MoveNext()
			{
				int num = this.<>1__state;
				GameObject result;
				try
				{
					OVRTask<bool>.Awaiter awaiter;
					if (num != 0)
					{
						OVRRoomLayout ovrroomLayout;
						if (this.anchor.TryGetComponent<OVRRoomLayout>(out ovrroomLayout))
						{
							result = new GameObject(string.Format("Room-{0}", this.anchor.Uuid));
							goto IL_1AF;
						}
						if (!this.anchor.TryGetComponent<OVRLocatable>(out this.<locatable>5__2))
						{
							result = null;
							goto IL_1AF;
						}
						awaiter = this.<locatable>5__2.SetEnabledAsync(true, 0.0).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							this.<>1__state = 0;
							this.<>u__1 = awaiter;
							this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, UnityObjectUpdater.<CreateUnityObject>d__0>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = this.<>u__1;
						this.<>u__1 = default(OVRTask<bool>.Awaiter);
						this.<>1__state = -1;
					}
					awaiter.GetResult();
					string name = "other";
					OVRSemanticLabels ovrsemanticLabels;
					if (this.anchor.TryGetComponent<OVRSemanticLabels>(out ovrsemanticLabels))
					{
						name = ovrsemanticLabels.Labels;
					}
					GameObject gameObject = new GameObject(name);
					if (this.parent != null)
					{
						gameObject.transform.SetParent(this.parent.transform);
					}
					SceneManagerHelper sceneManagerHelper = new SceneManagerHelper(gameObject);
					sceneManagerHelper.SetLocation(this.<locatable>5__2, null);
					OVRBounded2D bounds;
					if (this.anchor.TryGetComponent<OVRBounded2D>(out bounds) && bounds.IsEnabled)
					{
						sceneManagerHelper.CreatePlane(bounds);
					}
					OVRBounded3D bounds2;
					if (this.anchor.TryGetComponent<OVRBounded3D>(out bounds2) && bounds2.IsEnabled)
					{
						sceneManagerHelper.CreateVolume(bounds2);
					}
					OVRTriangleMesh mesh;
					if (this.anchor.TryGetComponent<OVRTriangleMesh>(out mesh) && mesh.IsEnabled)
					{
						sceneManagerHelper.CreateMesh(mesh);
					}
					result = gameObject;
				}
				catch (Exception exception)
				{
					this.<>1__state = -2;
					this.<>t__builder.SetException(exception);
					return;
				}
				IL_1AF:
				this.<>1__state = -2;
				this.<>t__builder.SetResult(result);
			}

			[DebuggerHidden]
			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				this.<>t__builder.SetStateMachine(stateMachine);
			}

			public int <>1__state;

			public AsyncTaskMethodBuilder<GameObject> <>t__builder;

			public OVRAnchor anchor;

			public GameObject parent;

			private OVRLocatable <locatable>5__2;

			private OVRTask<bool>.Awaiter <>u__1;
		}
	}
}
