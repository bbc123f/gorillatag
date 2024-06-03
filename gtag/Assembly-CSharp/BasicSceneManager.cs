using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

public class BasicSceneManager : MonoBehaviour
{
	private void Start()
	{
		SceneManagerHelper.RequestScenePermission();
		this.LoadSceneAsync();
	}

	private void LoadSceneAsync()
	{
		BasicSceneManager.<LoadSceneAsync>d__1 <LoadSceneAsync>d__;
		<LoadSceneAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<LoadSceneAsync>d__.<>4__this = this;
		<LoadSceneAsync>d__.<>1__state = -1;
		<LoadSceneAsync>d__.<>t__builder.Start<BasicSceneManager.<LoadSceneAsync>d__1>(ref <LoadSceneAsync>d__);
	}

	private Task CreateSceneAnchors(GameObject roomGameObject, List<OVRAnchor> anchors)
	{
		BasicSceneManager.<CreateSceneAnchors>d__2 <CreateSceneAnchors>d__;
		<CreateSceneAnchors>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<CreateSceneAnchors>d__.roomGameObject = roomGameObject;
		<CreateSceneAnchors>d__.anchors = anchors;
		<CreateSceneAnchors>d__.<>1__state = -1;
		<CreateSceneAnchors>d__.<>t__builder.Start<BasicSceneManager.<CreateSceneAnchors>d__2>(ref <CreateSceneAnchors>d__);
		return <CreateSceneAnchors>d__.<>t__builder.Task;
	}

	public BasicSceneManager()
	{
	}

	[CompilerGenerated]
	private Task <LoadSceneAsync>b__1_0(OVRAnchor room)
	{
		BasicSceneManager.<<LoadSceneAsync>b__1_0>d <<LoadSceneAsync>b__1_0>d;
		<<LoadSceneAsync>b__1_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
		<<LoadSceneAsync>b__1_0>d.<>4__this = this;
		<<LoadSceneAsync>b__1_0>d.room = room;
		<<LoadSceneAsync>b__1_0>d.<>1__state = -1;
		<<LoadSceneAsync>b__1_0>d.<>t__builder.Start<BasicSceneManager.<<LoadSceneAsync>b__1_0>d>(ref <<LoadSceneAsync>b__1_0>d);
		return <<LoadSceneAsync>b__1_0>d.<>t__builder.Task;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <<LoadSceneAsync>b__1_0>d : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			BasicSceneManager basicSceneManager = this.<>4__this;
			try
			{
				TaskAwaiter awaiter;
				OVRTask<bool>.Awaiter awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = this.<>u__2;
						this.<>u__2 = default(TaskAwaiter);
						this.<>1__state = -1;
						goto IL_126;
					}
					this.<roomObject>5__2 = new GameObject(string.Format("Room-{0}", this.room.Uuid));
					OVRAnchorContainer ovranchorContainer;
					if (!this.room.TryGetComponent<OVRAnchorContainer>(out ovranchorContainer))
					{
						goto IL_156;
					}
					this.<children>5__3 = new List<OVRAnchor>();
					awaiter2 = ovranchorContainer.FetchChildrenAsync(this.<children>5__3).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter2;
						this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, BasicSceneManager.<<LoadSceneAsync>b__1_0>d>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = this.<>u__1;
					this.<>u__1 = default(OVRTask<bool>.Awaiter);
					this.<>1__state = -1;
				}
				awaiter2.GetResult();
				awaiter = basicSceneManager.CreateSceneAnchors(this.<roomObject>5__2, this.<children>5__3).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					this.<>1__state = 1;
					this.<>u__2 = awaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, BasicSceneManager.<<LoadSceneAsync>b__1_0>d>(ref awaiter, ref this);
					return;
				}
				IL_126:
				awaiter.GetResult();
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<roomObject>5__2 = null;
				this.<children>5__3 = null;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_156:
			this.<>1__state = -2;
			this.<roomObject>5__2 = null;
			this.<children>5__3 = null;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public OVRAnchor room;

		public BasicSceneManager <>4__this;

		private GameObject <roomObject>5__2;

		private List<OVRAnchor> <children>5__3;

		private OVRTask<bool>.Awaiter <>u__1;

		private TaskAwaiter <>u__2;
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass2_0
	{
		public <>c__DisplayClass2_0()
		{
		}

		internal Task <CreateSceneAnchors>b__0(OVRAnchor anchor)
		{
			BasicSceneManager.<>c__DisplayClass2_0.<<CreateSceneAnchors>b__0>d <<CreateSceneAnchors>b__0>d;
			<<CreateSceneAnchors>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<CreateSceneAnchors>b__0>d.<>4__this = this;
			<<CreateSceneAnchors>b__0>d.anchor = anchor;
			<<CreateSceneAnchors>b__0>d.<>1__state = -1;
			<<CreateSceneAnchors>b__0>d.<>t__builder.Start<BasicSceneManager.<>c__DisplayClass2_0.<<CreateSceneAnchors>b__0>d>(ref <<CreateSceneAnchors>b__0>d);
			return <<CreateSceneAnchors>b__0>d.<>t__builder.Task;
		}

		public GameObject roomGameObject;

		[StructLayout(LayoutKind.Auto)]
		private struct <<CreateSceneAnchors>b__0>d : IAsyncStateMachine
		{
			void IAsyncStateMachine.MoveNext()
			{
				int num = this.<>1__state;
				BasicSceneManager.<>c__DisplayClass2_0 CS$<>8__locals1 = this.<>4__this;
				try
				{
					OVRTask<bool>.Awaiter awaiter;
					if (num != 0)
					{
						if (!this.anchor.TryGetComponent<OVRLocatable>(out this.<locatable>5__2))
						{
							goto IL_146;
						}
						awaiter = this.<locatable>5__2.SetEnabledAsync(true, 0.0).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							this.<>1__state = 0;
							this.<>u__1 = awaiter;
							this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, BasicSceneManager.<>c__DisplayClass2_0.<<CreateSceneAnchors>b__0>d>(ref awaiter, ref this);
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
					gameObject.transform.SetParent(CS$<>8__locals1.roomGameObject.transform);
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
				}
				catch (Exception exception)
				{
					this.<>1__state = -2;
					this.<>t__builder.SetException(exception);
					return;
				}
				IL_146:
				this.<>1__state = -2;
				this.<>t__builder.SetResult();
			}

			[DebuggerHidden]
			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				this.<>t__builder.SetStateMachine(stateMachine);
			}

			public int <>1__state;

			public AsyncTaskMethodBuilder <>t__builder;

			public OVRAnchor anchor;

			public BasicSceneManager.<>c__DisplayClass2_0 <>4__this;

			private OVRLocatable <locatable>5__2;

			private OVRTask<bool>.Awaiter <>u__1;
		}
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <CreateSceneAnchors>d__2 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					BasicSceneManager.<>c__DisplayClass2_0 CS$<>8__locals1 = new BasicSceneManager.<>c__DisplayClass2_0();
					CS$<>8__locals1.roomGameObject = this.roomGameObject;
					awaiter = Task.WhenAll(this.anchors.Select(new Func<OVRAnchor, Task>(CS$<>8__locals1.<CreateSceneAnchors>b__0)).ToList<Task>()).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, BasicSceneManager.<CreateSceneAnchors>d__2>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter);
					this.<>1__state = -1;
				}
				awaiter.GetResult();
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public GameObject roomGameObject;

		public List<OVRAnchor> anchors;

		private TaskAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <LoadSceneAsync>d__1 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			BasicSceneManager @object = this.<>4__this;
			try
			{
				OVRTask<bool>.Awaiter awaiter;
				TaskAwaiter<bool> awaiter2;
				TaskAwaiter awaiter3;
				switch (num)
				{
				case 0:
					awaiter = this.<>u__1;
					this.<>u__1 = default(OVRTask<bool>.Awaiter);
					this.<>1__state = -1;
					break;
				case 1:
					awaiter2 = this.<>u__2;
					this.<>u__2 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
					goto IL_108;
				case 2:
					awaiter = this.<>u__1;
					this.<>u__1 = default(OVRTask<bool>.Awaiter);
					this.<>1__state = -1;
					goto IL_181;
				case 3:
					awaiter3 = this.<>u__3;
					this.<>u__3 = default(TaskAwaiter);
					this.<>1__state = -1;
					goto IL_1F8;
				default:
					this.<rooms>5__2 = new List<OVRAnchor>();
					awaiter = OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(this.<rooms>5__2, OVRSpace.StorageLocation.Local, 1024, 0.0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, BasicSceneManager.<LoadSceneAsync>d__1>(ref awaiter, ref this);
						return;
					}
					break;
				}
				awaiter.GetResult();
				if (this.<rooms>5__2.Count != 0)
				{
					goto IL_189;
				}
				awaiter2 = SceneManagerHelper.RequestSceneCapture().GetAwaiter();
				if (!awaiter2.IsCompleted)
				{
					this.<>1__state = 1;
					this.<>u__2 = awaiter2;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, BasicSceneManager.<LoadSceneAsync>d__1>(ref awaiter2, ref this);
					return;
				}
				IL_108:
				if (!awaiter2.GetResult())
				{
					goto IL_221;
				}
				awaiter = OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(this.<rooms>5__2, OVRSpace.StorageLocation.Local, 1024, 0.0).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					this.<>1__state = 2;
					this.<>u__1 = awaiter;
					this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, BasicSceneManager.<LoadSceneAsync>d__1>(ref awaiter, ref this);
					return;
				}
				IL_181:
				awaiter.GetResult();
				IL_189:
				awaiter3 = Task.WhenAll(this.<rooms>5__2.Select(delegate(OVRAnchor room)
				{
					BasicSceneManager.<<LoadSceneAsync>b__1_0>d <<LoadSceneAsync>b__1_0>d;
					<<LoadSceneAsync>b__1_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<LoadSceneAsync>b__1_0>d.<>4__this = @object;
					<<LoadSceneAsync>b__1_0>d.room = room;
					<<LoadSceneAsync>b__1_0>d.<>1__state = -1;
					<<LoadSceneAsync>b__1_0>d.<>t__builder.Start<BasicSceneManager.<<LoadSceneAsync>b__1_0>d>(ref <<LoadSceneAsync>b__1_0>d);
					return <<LoadSceneAsync>b__1_0>d.<>t__builder.Task;
				}).ToList<Task>()).GetAwaiter();
				if (!awaiter3.IsCompleted)
				{
					this.<>1__state = 3;
					this.<>u__3 = awaiter3;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, BasicSceneManager.<LoadSceneAsync>d__1>(ref awaiter3, ref this);
					return;
				}
				IL_1F8:
				awaiter3.GetResult();
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<rooms>5__2 = null;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_221:
			this.<>1__state = -2;
			this.<rooms>5__2 = null;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public BasicSceneManager <>4__this;

		private List<OVRAnchor> <rooms>5__2;

		private OVRTask<bool>.Awaiter <>u__1;

		private TaskAwaiter<bool> <>u__2;

		private TaskAwaiter <>u__3;
	}
}
