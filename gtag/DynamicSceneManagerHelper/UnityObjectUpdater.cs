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
		public async Task<GameObject> CreateUnityObject(OVRAnchor anchor, GameObject parent)
		{
			OVRRoomLayout ovrroomLayout;
			GameObject gameObject;
			OVRLocatable locatable;
			if (anchor.TryGetComponent<OVRRoomLayout>(out ovrroomLayout))
			{
				gameObject = new GameObject(string.Format("Room-{0}", anchor.Uuid));
			}
			else if (!anchor.TryGetComponent<OVRLocatable>(out locatable))
			{
				gameObject = null;
			}
			else
			{
				await locatable.SetEnabledAsync(true, 0.0);
				string text = "other";
				OVRSemanticLabels ovrsemanticLabels;
				if (anchor.TryGetComponent<OVRSemanticLabels>(out ovrsemanticLabels))
				{
					text = ovrsemanticLabels.Labels;
				}
				GameObject gameObject2 = new GameObject(text);
				if (parent != null)
				{
					gameObject2.transform.SetParent(parent.transform);
				}
				SceneManagerHelper sceneManagerHelper = new SceneManagerHelper(gameObject2);
				sceneManagerHelper.SetLocation(locatable, null);
				OVRBounded2D ovrbounded2D;
				if (anchor.TryGetComponent<OVRBounded2D>(out ovrbounded2D) && ovrbounded2D.IsEnabled)
				{
					sceneManagerHelper.CreatePlane(ovrbounded2D);
				}
				OVRBounded3D ovrbounded3D;
				if (anchor.TryGetComponent<OVRBounded3D>(out ovrbounded3D) && ovrbounded3D.IsEnabled)
				{
					sceneManagerHelper.CreateVolume(ovrbounded3D);
				}
				OVRTriangleMesh ovrtriangleMesh;
				if (anchor.TryGetComponent<OVRTriangleMesh>(out ovrtriangleMesh) && ovrtriangleMesh.IsEnabled)
				{
					sceneManagerHelper.CreateMesh(ovrtriangleMesh);
				}
				gameObject = gameObject2;
			}
			return gameObject;
		}

		public void UpdateUnityObject(OVRAnchor anchor, GameObject gameObject)
		{
			SceneManagerHelper sceneManagerHelper = new SceneManagerHelper(gameObject);
			OVRLocatable ovrlocatable;
			if (anchor.TryGetComponent<OVRLocatable>(out ovrlocatable))
			{
				sceneManagerHelper.SetLocation(ovrlocatable, null);
			}
			OVRBounded2D ovrbounded2D;
			if (anchor.TryGetComponent<OVRBounded2D>(out ovrbounded2D) && ovrbounded2D.IsEnabled)
			{
				sceneManagerHelper.UpdatePlane(ovrbounded2D);
			}
			OVRBounded3D ovrbounded3D;
			if (anchor.TryGetComponent<OVRBounded3D>(out ovrbounded3D) && ovrbounded3D.IsEnabled)
			{
				sceneManagerHelper.UpdateVolume(ovrbounded3D);
			}
			OVRTriangleMesh ovrtriangleMesh;
			if (anchor.TryGetComponent<OVRTriangleMesh>(out ovrtriangleMesh) && ovrtriangleMesh.IsEnabled)
			{
				sceneManagerHelper.UpdateMesh(ovrtriangleMesh);
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
				int num2;
				int num = num2;
				GameObject gameObject;
				try
				{
					OVRTask<bool>.Awaiter awaiter;
					if (num != 0)
					{
						OVRRoomLayout ovrroomLayout;
						if (anchor.TryGetComponent<OVRRoomLayout>(out ovrroomLayout))
						{
							gameObject = new GameObject(string.Format("Room-{0}", anchor.Uuid));
							goto IL_1AF;
						}
						if (!anchor.TryGetComponent<OVRLocatable>(out locatable))
						{
							gameObject = null;
							goto IL_1AF;
						}
						awaiter = locatable.SetEnabledAsync(true, 0.0).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num2 = 0;
							OVRTask<bool>.Awaiter awaiter2 = awaiter;
							this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, UnityObjectUpdater.<CreateUnityObject>d__0>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						OVRTask<bool>.Awaiter awaiter2;
						awaiter = awaiter2;
						awaiter2 = default(OVRTask<bool>.Awaiter);
						num2 = -1;
					}
					awaiter.GetResult();
					string text = "other";
					OVRSemanticLabels ovrsemanticLabels;
					if (anchor.TryGetComponent<OVRSemanticLabels>(out ovrsemanticLabels))
					{
						text = ovrsemanticLabels.Labels;
					}
					GameObject gameObject2 = new GameObject(text);
					if (parent != null)
					{
						gameObject2.transform.SetParent(parent.transform);
					}
					SceneManagerHelper sceneManagerHelper = new SceneManagerHelper(gameObject2);
					sceneManagerHelper.SetLocation(locatable, null);
					OVRBounded2D ovrbounded2D;
					if (anchor.TryGetComponent<OVRBounded2D>(out ovrbounded2D) && ovrbounded2D.IsEnabled)
					{
						sceneManagerHelper.CreatePlane(ovrbounded2D);
					}
					OVRBounded3D ovrbounded3D;
					if (anchor.TryGetComponent<OVRBounded3D>(out ovrbounded3D) && ovrbounded3D.IsEnabled)
					{
						sceneManagerHelper.CreateVolume(ovrbounded3D);
					}
					OVRTriangleMesh ovrtriangleMesh;
					if (anchor.TryGetComponent<OVRTriangleMesh>(out ovrtriangleMesh) && ovrtriangleMesh.IsEnabled)
					{
						sceneManagerHelper.CreateMesh(ovrtriangleMesh);
					}
					gameObject = gameObject2;
				}
				catch (Exception ex)
				{
					num2 = -2;
					this.<>t__builder.SetException(ex);
					return;
				}
				IL_1AF:
				num2 = -2;
				this.<>t__builder.SetResult(gameObject);
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
