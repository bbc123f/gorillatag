using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

public class PrefabSceneManager : MonoBehaviour
{
	private void Start()
	{
		SceneManagerHelper.RequestScenePermission();
		this.LoadSceneAsync();
		base.StartCoroutine(this.UpdateAnchorsPeriodically());
	}

	private void LoadSceneAsync()
	{
		PrefabSceneManager.<LoadSceneAsync>d__7 <LoadSceneAsync>d__;
		<LoadSceneAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<LoadSceneAsync>d__.<>4__this = this;
		<LoadSceneAsync>d__.<>1__state = -1;
		<LoadSceneAsync>d__.<>t__builder.Start<PrefabSceneManager.<LoadSceneAsync>d__7>(ref <LoadSceneAsync>d__);
	}

	private Task CreateSceneAnchors(GameObject roomGameObject, OVRRoomLayout roomLayout, List<OVRAnchor> anchors)
	{
		PrefabSceneManager.<CreateSceneAnchors>d__8 <CreateSceneAnchors>d__;
		<CreateSceneAnchors>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<CreateSceneAnchors>d__.<>4__this = this;
		<CreateSceneAnchors>d__.roomGameObject = roomGameObject;
		<CreateSceneAnchors>d__.roomLayout = roomLayout;
		<CreateSceneAnchors>d__.anchors = anchors;
		<CreateSceneAnchors>d__.<>1__state = -1;
		<CreateSceneAnchors>d__.<>t__builder.Start<PrefabSceneManager.<CreateSceneAnchors>d__8>(ref <CreateSceneAnchors>d__);
		return <CreateSceneAnchors>d__.<>t__builder.Task;
	}

	private IEnumerator UpdateAnchorsPeriodically()
	{
		for (;;)
		{
			foreach (ValueTuple<GameObject, OVRLocatable> valueTuple in this._locatableObjects)
			{
				GameObject item = valueTuple.Item1;
				OVRLocatable item2 = valueTuple.Item2;
				new SceneManagerHelper(item).SetLocation(item2, null);
			}
			yield return new WaitForSeconds(this.UpdateFrequencySeconds);
		}
		yield break;
	}

	public PrefabSceneManager()
	{
	}

	[CompilerGenerated]
	private Task <LoadSceneAsync>b__7_0(OVRAnchor room)
	{
		PrefabSceneManager.<<LoadSceneAsync>b__7_0>d <<LoadSceneAsync>b__7_0>d;
		<<LoadSceneAsync>b__7_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
		<<LoadSceneAsync>b__7_0>d.<>4__this = this;
		<<LoadSceneAsync>b__7_0>d.room = room;
		<<LoadSceneAsync>b__7_0>d.<>1__state = -1;
		<<LoadSceneAsync>b__7_0>d.<>t__builder.Start<PrefabSceneManager.<<LoadSceneAsync>b__7_0>d>(ref <<LoadSceneAsync>b__7_0>d);
		return <<LoadSceneAsync>b__7_0>d.<>t__builder.Task;
	}

	public GameObject WallPrefab;

	public GameObject CeilingPrefab;

	public GameObject FloorPrefab;

	public GameObject FallbackPrefab;

	public float UpdateFrequencySeconds = 5f;

	private List<ValueTuple<GameObject, OVRLocatable>> _locatableObjects = new List<ValueTuple<GameObject, OVRLocatable>>();

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <<LoadSceneAsync>b__7_0>d : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			PrefabSceneManager prefabSceneManager = this.<>4__this;
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
						goto IL_144;
					}
					this.<roomObject>5__2 = new GameObject(string.Format("Room-{0}", this.room.Uuid));
					OVRAnchorContainer ovranchorContainer;
					if (!this.room.TryGetComponent<OVRAnchorContainer>(out ovranchorContainer))
					{
						goto IL_174;
					}
					if (!this.room.TryGetComponent<OVRRoomLayout>(out this.<roomLayout>5__3))
					{
						goto IL_174;
					}
					this.<children>5__4 = new List<OVRAnchor>();
					awaiter2 = ovranchorContainer.FetchChildrenAsync(this.<children>5__4).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter2;
						this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, PrefabSceneManager.<<LoadSceneAsync>b__7_0>d>(ref awaiter2, ref this);
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
				awaiter = prefabSceneManager.CreateSceneAnchors(this.<roomObject>5__2, this.<roomLayout>5__3, this.<children>5__4).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					this.<>1__state = 1;
					this.<>u__2 = awaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, PrefabSceneManager.<<LoadSceneAsync>b__7_0>d>(ref awaiter, ref this);
					return;
				}
				IL_144:
				awaiter.GetResult();
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<roomObject>5__2 = null;
				this.<children>5__4 = null;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_174:
			this.<>1__state = -2;
			this.<roomObject>5__2 = null;
			this.<children>5__4 = null;
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

		public PrefabSceneManager <>4__this;

		private GameObject <roomObject>5__2;

		private OVRRoomLayout <roomLayout>5__3;

		private List<OVRAnchor> <children>5__4;

		private OVRTask<bool>.Awaiter <>u__1;

		private TaskAwaiter <>u__2;
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass8_0
	{
		public <>c__DisplayClass8_0()
		{
		}

		internal Task <CreateSceneAnchors>b__0(OVRAnchor anchor)
		{
			PrefabSceneManager.<>c__DisplayClass8_0.<<CreateSceneAnchors>b__0>d <<CreateSceneAnchors>b__0>d;
			<<CreateSceneAnchors>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<CreateSceneAnchors>b__0>d.<>4__this = this;
			<<CreateSceneAnchors>b__0>d.anchor = anchor;
			<<CreateSceneAnchors>b__0>d.<>1__state = -1;
			<<CreateSceneAnchors>b__0>d.<>t__builder.Start<PrefabSceneManager.<>c__DisplayClass8_0.<<CreateSceneAnchors>b__0>d>(ref <<CreateSceneAnchors>b__0>d);
			return <<CreateSceneAnchors>b__0>d.<>t__builder.Task;
		}

		public PrefabSceneManager <>4__this;

		public Guid floorUuid;

		public Guid ceilingUuid;

		public Guid[] wallUuids;

		public GameObject roomGameObject;

		[StructLayout(LayoutKind.Auto)]
		private struct <<CreateSceneAnchors>b__0>d : IAsyncStateMachine
		{
			void IAsyncStateMachine.MoveNext()
			{
				int num = this.<>1__state;
				PrefabSceneManager.<>c__DisplayClass8_0 CS$<>8__locals1 = this.<>4__this;
				try
				{
					OVRTask<bool>.Awaiter awaiter;
					if (num != 0)
					{
						if (!this.anchor.TryGetComponent<OVRLocatable>(out this.<locatable>5__2))
						{
							goto IL_283;
						}
						awaiter = this.<locatable>5__2.SetEnabledAsync(true, 0.0).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							this.<>1__state = 0;
							this.<>u__1 = awaiter;
							this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, PrefabSceneManager.<>c__DisplayClass8_0.<<CreateSceneAnchors>b__0>d>(ref awaiter, ref this);
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
					GameObject gameObject = CS$<>8__locals1.<>4__this.FallbackPrefab;
					if (this.anchor.Uuid == CS$<>8__locals1.floorUuid)
					{
						gameObject = CS$<>8__locals1.<>4__this.FloorPrefab;
					}
					else if (this.anchor.Uuid == CS$<>8__locals1.ceilingUuid)
					{
						gameObject = CS$<>8__locals1.<>4__this.CeilingPrefab;
					}
					else if (CS$<>8__locals1.wallUuids.Contains(this.anchor.Uuid))
					{
						gameObject = CS$<>8__locals1.<>4__this.WallPrefab;
					}
					string name = "other";
					OVRSemanticLabels ovrsemanticLabels;
					if (this.anchor.TryGetComponent<OVRSemanticLabels>(out ovrsemanticLabels))
					{
						name = ovrsemanticLabels.Labels;
					}
					GameObject gameObject2 = new GameObject(name);
					gameObject2.transform.SetParent(CS$<>8__locals1.roomGameObject.transform);
					new SceneManagerHelper(gameObject2).SetLocation(this.<locatable>5__2, null);
					GameObject gameObject3 = Object.Instantiate<GameObject>(gameObject, gameObject2.transform);
					OVRBounded2D ovrbounded2D;
					if (this.anchor.TryGetComponent<OVRBounded2D>(out ovrbounded2D) && ovrbounded2D.IsEnabled)
					{
						gameObject3.transform.localScale = new Vector3(ovrbounded2D.BoundingBox.size.x, ovrbounded2D.BoundingBox.size.y, 0.01f);
					}
					OVRBounded3D ovrbounded3D;
					if (gameObject == CS$<>8__locals1.<>4__this.FallbackPrefab && this.anchor.TryGetComponent<OVRBounded3D>(out ovrbounded3D) && ovrbounded3D.IsEnabled)
					{
						gameObject3.transform.localPosition = new Vector3(0f, 0f, -ovrbounded3D.BoundingBox.size.z / 2f);
						gameObject3.transform.localScale = ovrbounded3D.BoundingBox.size;
					}
					CS$<>8__locals1.<>4__this._locatableObjects.Add(new ValueTuple<GameObject, OVRLocatable>(gameObject2, this.<locatable>5__2));
				}
				catch (Exception exception)
				{
					this.<>1__state = -2;
					this.<>t__builder.SetException(exception);
					return;
				}
				IL_283:
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

			public PrefabSceneManager.<>c__DisplayClass8_0 <>4__this;

			private OVRLocatable <locatable>5__2;

			private OVRTask<bool>.Awaiter <>u__1;
		}
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <CreateSceneAnchors>d__8 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					PrefabSceneManager.<>c__DisplayClass8_0 CS$<>8__locals1 = new PrefabSceneManager.<>c__DisplayClass8_0();
					CS$<>8__locals1.<>4__this = this.<>4__this;
					CS$<>8__locals1.roomGameObject = this.roomGameObject;
					this.roomLayout.TryGetRoomLayout(out CS$<>8__locals1.ceilingUuid, out CS$<>8__locals1.floorUuid, out CS$<>8__locals1.wallUuids);
					awaiter = Task.WhenAll(this.anchors.Select(new Func<OVRAnchor, Task>(CS$<>8__locals1.<CreateSceneAnchors>b__0)).ToList<Task>()).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, PrefabSceneManager.<CreateSceneAnchors>d__8>(ref awaiter, ref this);
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

		public PrefabSceneManager <>4__this;

		public GameObject roomGameObject;

		public OVRRoomLayout roomLayout;

		public List<OVRAnchor> anchors;

		private TaskAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <LoadSceneAsync>d__7 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			PrefabSceneManager @object = this.<>4__this;
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
						this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, PrefabSceneManager.<LoadSceneAsync>d__7>(ref awaiter, ref this);
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
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, PrefabSceneManager.<LoadSceneAsync>d__7>(ref awaiter2, ref this);
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
					this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, PrefabSceneManager.<LoadSceneAsync>d__7>(ref awaiter, ref this);
					return;
				}
				IL_181:
				awaiter.GetResult();
				IL_189:
				awaiter3 = Task.WhenAll(this.<rooms>5__2.Select(delegate(OVRAnchor room)
				{
					PrefabSceneManager.<<LoadSceneAsync>b__7_0>d <<LoadSceneAsync>b__7_0>d;
					<<LoadSceneAsync>b__7_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<LoadSceneAsync>b__7_0>d.<>4__this = @object;
					<<LoadSceneAsync>b__7_0>d.room = room;
					<<LoadSceneAsync>b__7_0>d.<>1__state = -1;
					<<LoadSceneAsync>b__7_0>d.<>t__builder.Start<PrefabSceneManager.<<LoadSceneAsync>b__7_0>d>(ref <<LoadSceneAsync>b__7_0>d);
					return <<LoadSceneAsync>b__7_0>d.<>t__builder.Task;
				}).ToList<Task>()).GetAwaiter();
				if (!awaiter3.IsCompleted)
				{
					this.<>1__state = 3;
					this.<>u__3 = awaiter3;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, PrefabSceneManager.<LoadSceneAsync>d__7>(ref awaiter3, ref this);
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

		public PrefabSceneManager <>4__this;

		private List<OVRAnchor> <rooms>5__2;

		private OVRTask<bool>.Awaiter <>u__1;

		private TaskAwaiter<bool> <>u__2;

		private TaskAwaiter <>u__3;
	}

	[CompilerGenerated]
	private sealed class <UpdateAnchorsPeriodically>d__9 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <UpdateAnchorsPeriodically>d__9(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			PrefabSceneManager prefabSceneManager = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
			}
			else
			{
				this.<>1__state = -1;
			}
			foreach (ValueTuple<GameObject, OVRLocatable> valueTuple in prefabSceneManager._locatableObjects)
			{
				GameObject item = valueTuple.Item1;
				OVRLocatable item2 = valueTuple.Item2;
				new SceneManagerHelper(item).SetLocation(item2, null);
			}
			this.<>2__current = new WaitForSeconds(prefabSceneManager.UpdateFrequencySeconds);
			this.<>1__state = 1;
			return true;
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

		public PrefabSceneManager <>4__this;
	}
}
