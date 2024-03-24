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

	private async void LoadSceneAsync()
	{
		List<OVRAnchor> rooms = new List<OVRAnchor>();
		await OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms, OVRSpace.StorageLocation.Local, 1024, 0.0);
		if (rooms.Count == 0)
		{
			TaskAwaiter<bool> taskAwaiter = SceneManagerHelper.RequestSceneCapture().GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				return;
			}
			await OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms, OVRSpace.StorageLocation.Local, 1024, 0.0);
		}
		await Task.WhenAll(rooms.Select(async delegate(OVRAnchor room)
		{
			roomObject = new GameObject(string.Format("Room-{0}", room.Uuid));
			OVRAnchorContainer ovranchorContainer;
			if (room.TryGetComponent<OVRAnchorContainer>(out ovranchorContainer))
			{
				children = new List<OVRAnchor>();
				await ovranchorContainer.FetchChildrenAsync(children);
				await this.CreateSceneAnchors(roomObject, children);
			}
		}).ToList<Task>());
	}

	private async Task CreateSceneAnchors(GameObject roomGameObject, List<OVRAnchor> anchors)
	{
		BasicSceneManager.<>c__DisplayClass2_0 CS$<>8__locals1 = new BasicSceneManager.<>c__DisplayClass2_0();
		CS$<>8__locals1.roomGameObject = roomGameObject;
		await Task.WhenAll(anchors.Select(delegate(OVRAnchor anchor)
		{
			BasicSceneManager.<>c__DisplayClass2_0.<<CreateSceneAnchors>b__0>d <<CreateSceneAnchors>b__0>d;
			<<CreateSceneAnchors>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<CreateSceneAnchors>b__0>d.<>4__this = CS$<>8__locals1;
			<<CreateSceneAnchors>b__0>d.anchor = anchor;
			<<CreateSceneAnchors>b__0>d.<>1__state = -1;
			<<CreateSceneAnchors>b__0>d.<>t__builder.Start<BasicSceneManager.<>c__DisplayClass2_0.<<CreateSceneAnchors>b__0>d>(ref <<CreateSceneAnchors>b__0>d);
			return <<CreateSceneAnchors>b__0>d.<>t__builder.Task;
		}).ToList<Task>());
	}

	public BasicSceneManager()
	{
	}

	[CompilerGenerated]
	private async Task <LoadSceneAsync>b__1_0(OVRAnchor room)
	{
		GameObject roomObject = new GameObject(string.Format("Room-{0}", room.Uuid));
		OVRAnchorContainer ovranchorContainer;
		if (room.TryGetComponent<OVRAnchorContainer>(out ovranchorContainer))
		{
			List<OVRAnchor> children = new List<OVRAnchor>();
			await ovranchorContainer.FetchChildrenAsync(children);
			await this.CreateSceneAnchors(roomObject, children);
		}
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <<LoadSceneAsync>b__1_0>d : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			BasicSceneManager basicSceneManager = this;
			try
			{
				TaskAwaiter taskAwaiter;
				OVRTask<bool>.Awaiter awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						TaskAwaiter taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter);
						num2 = -1;
						goto IL_126;
					}
					roomObject = new GameObject(string.Format("Room-{0}", room.Uuid));
					OVRAnchorContainer ovranchorContainer;
					if (!room.TryGetComponent<OVRAnchorContainer>(out ovranchorContainer))
					{
						goto IL_156;
					}
					children = new List<OVRAnchor>();
					awaiter = ovranchorContainer.FetchChildrenAsync(children).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num2 = 0;
						OVRTask<bool>.Awaiter awaiter2 = awaiter;
						this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, BasicSceneManager.<<LoadSceneAsync>b__1_0>d>(ref awaiter, ref this);
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
				taskAwaiter = basicSceneManager.CreateSceneAnchors(roomObject, children).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					num2 = 1;
					TaskAwaiter taskAwaiter2 = taskAwaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, BasicSceneManager.<<LoadSceneAsync>b__1_0>d>(ref taskAwaiter, ref this);
					return;
				}
				IL_126:
				taskAwaiter.GetResult();
			}
			catch (Exception ex)
			{
				num2 = -2;
				roomObject = null;
				children = null;
				this.<>t__builder.SetException(ex);
				return;
			}
			IL_156:
			num2 = -2;
			roomObject = null;
			children = null;
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

		internal async Task <CreateSceneAnchors>b__0(OVRAnchor anchor)
		{
			OVRLocatable locatable;
			if (anchor.TryGetComponent<OVRLocatable>(out locatable))
			{
				await locatable.SetEnabledAsync(true, 0.0);
				string text = "other";
				OVRSemanticLabels ovrsemanticLabels;
				if (anchor.TryGetComponent<OVRSemanticLabels>(out ovrsemanticLabels))
				{
					text = ovrsemanticLabels.Labels;
				}
				GameObject gameObject = new GameObject(text);
				gameObject.transform.SetParent(this.roomGameObject.transform);
				SceneManagerHelper sceneManagerHelper = new SceneManagerHelper(gameObject);
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
			}
		}

		public GameObject roomGameObject;

		[StructLayout(LayoutKind.Auto)]
		private struct <<CreateSceneAnchors>b__0>d : IAsyncStateMachine
		{
			void IAsyncStateMachine.MoveNext()
			{
				int num2;
				int num = num2;
				BasicSceneManager.<>c__DisplayClass2_0 CS$<>8__locals1 = this;
				try
				{
					OVRTask<bool>.Awaiter awaiter;
					if (num != 0)
					{
						if (!anchor.TryGetComponent<OVRLocatable>(out locatable))
						{
							goto IL_146;
						}
						awaiter = locatable.SetEnabledAsync(true, 0.0).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num2 = 0;
							OVRTask<bool>.Awaiter awaiter2 = awaiter;
							this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, BasicSceneManager.<>c__DisplayClass2_0.<<CreateSceneAnchors>b__0>d>(ref awaiter, ref this);
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
					GameObject gameObject = new GameObject(text);
					gameObject.transform.SetParent(CS$<>8__locals1.roomGameObject.transform);
					SceneManagerHelper sceneManagerHelper = new SceneManagerHelper(gameObject);
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
				}
				catch (Exception ex)
				{
					num2 = -2;
					this.<>t__builder.SetException(ex);
					return;
				}
				IL_146:
				num2 = -2;
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
			int num2;
			int num = num2;
			try
			{
				TaskAwaiter taskAwaiter;
				if (num != 0)
				{
					BasicSceneManager.<>c__DisplayClass2_0 CS$<>8__locals1 = new BasicSceneManager.<>c__DisplayClass2_0();
					CS$<>8__locals1.roomGameObject = roomGameObject;
					taskAwaiter = Task.WhenAll(anchors.Select(delegate(OVRAnchor anchor)
					{
						BasicSceneManager.<>c__DisplayClass2_0.<<CreateSceneAnchors>b__0>d <<CreateSceneAnchors>b__0>d;
						<<CreateSceneAnchors>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<CreateSceneAnchors>b__0>d.<>4__this = CS$<>8__locals1;
						<<CreateSceneAnchors>b__0>d.anchor = anchor;
						<<CreateSceneAnchors>b__0>d.<>1__state = -1;
						<<CreateSceneAnchors>b__0>d.<>t__builder.Start<BasicSceneManager.<>c__DisplayClass2_0.<<CreateSceneAnchors>b__0>d>(ref <<CreateSceneAnchors>b__0>d);
						return <<CreateSceneAnchors>b__0>d.<>t__builder.Task;
					}).ToList<Task>()).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						num2 = 0;
						TaskAwaiter taskAwaiter2 = taskAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, BasicSceneManager.<CreateSceneAnchors>d__2>(ref taskAwaiter, ref this);
						return;
					}
				}
				else
				{
					TaskAwaiter taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter);
					num2 = -1;
				}
				taskAwaiter.GetResult();
			}
			catch (Exception ex)
			{
				num2 = -2;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
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
			int num2;
			int num = num2;
			BasicSceneManager basicSceneManager = this;
			try
			{
				OVRTask<bool>.Awaiter awaiter;
				TaskAwaiter<bool> taskAwaiter3;
				TaskAwaiter taskAwaiter4;
				switch (num)
				{
				case 0:
				{
					OVRTask<bool>.Awaiter awaiter2;
					awaiter = awaiter2;
					awaiter2 = default(OVRTask<bool>.Awaiter);
					num2 = -1;
					break;
				}
				case 1:
					taskAwaiter3 = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
					num2 = -1;
					goto IL_108;
				case 2:
				{
					OVRTask<bool>.Awaiter awaiter2;
					awaiter = awaiter2;
					awaiter2 = default(OVRTask<bool>.Awaiter);
					num2 = -1;
					goto IL_181;
				}
				case 3:
				{
					TaskAwaiter taskAwaiter5;
					taskAwaiter4 = taskAwaiter5;
					taskAwaiter5 = default(TaskAwaiter);
					num2 = -1;
					goto IL_1F8;
				}
				default:
					rooms = new List<OVRAnchor>();
					awaiter = OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms, OVRSpace.StorageLocation.Local, 1024, 0.0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num2 = 0;
						OVRTask<bool>.Awaiter awaiter2 = awaiter;
						this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, BasicSceneManager.<LoadSceneAsync>d__1>(ref awaiter, ref this);
						return;
					}
					break;
				}
				awaiter.GetResult();
				if (rooms.Count != 0)
				{
					goto IL_189;
				}
				taskAwaiter3 = SceneManagerHelper.RequestSceneCapture().GetAwaiter();
				if (!taskAwaiter3.IsCompleted)
				{
					num2 = 1;
					taskAwaiter2 = taskAwaiter3;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, BasicSceneManager.<LoadSceneAsync>d__1>(ref taskAwaiter3, ref this);
					return;
				}
				IL_108:
				if (!taskAwaiter3.GetResult())
				{
					goto IL_221;
				}
				awaiter = OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms, OVRSpace.StorageLocation.Local, 1024, 0.0).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num2 = 2;
					OVRTask<bool>.Awaiter awaiter2 = awaiter;
					this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, BasicSceneManager.<LoadSceneAsync>d__1>(ref awaiter, ref this);
					return;
				}
				IL_181:
				awaiter.GetResult();
				IL_189:
				taskAwaiter4 = Task.WhenAll(rooms.Select(delegate(OVRAnchor room)
				{
					BasicSceneManager.<<LoadSceneAsync>b__1_0>d <<LoadSceneAsync>b__1_0>d;
					<<LoadSceneAsync>b__1_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<LoadSceneAsync>b__1_0>d.<>4__this = basicSceneManager;
					<<LoadSceneAsync>b__1_0>d.room = room;
					<<LoadSceneAsync>b__1_0>d.<>1__state = -1;
					<<LoadSceneAsync>b__1_0>d.<>t__builder.Start<BasicSceneManager.<<LoadSceneAsync>b__1_0>d>(ref <<LoadSceneAsync>b__1_0>d);
					return <<LoadSceneAsync>b__1_0>d.<>t__builder.Task;
				}).ToList<Task>()).GetAwaiter();
				if (!taskAwaiter4.IsCompleted)
				{
					num2 = 3;
					TaskAwaiter taskAwaiter5 = taskAwaiter4;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, BasicSceneManager.<LoadSceneAsync>d__1>(ref taskAwaiter4, ref this);
					return;
				}
				IL_1F8:
				taskAwaiter4.GetResult();
			}
			catch (Exception ex)
			{
				num2 = -2;
				rooms = null;
				this.<>t__builder.SetException(ex);
				return;
			}
			IL_221:
			num2 = -2;
			rooms = null;
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
