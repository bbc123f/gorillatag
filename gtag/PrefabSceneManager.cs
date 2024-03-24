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
				if (room.TryGetComponent<OVRRoomLayout>(out roomLayout))
				{
					children = new List<OVRAnchor>();
					await ovranchorContainer.FetchChildrenAsync(children);
					await this.CreateSceneAnchors(roomObject, roomLayout, children);
				}
			}
		}).ToList<Task>());
	}

	private async Task CreateSceneAnchors(GameObject roomGameObject, OVRRoomLayout roomLayout, List<OVRAnchor> anchors)
	{
		PrefabSceneManager.<>c__DisplayClass8_0 CS$<>8__locals1 = new PrefabSceneManager.<>c__DisplayClass8_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.roomGameObject = roomGameObject;
		roomLayout.TryGetRoomLayout(out CS$<>8__locals1.ceilingUuid, out CS$<>8__locals1.floorUuid, out CS$<>8__locals1.wallUuids);
		await Task.WhenAll(anchors.Select(delegate(OVRAnchor anchor)
		{
			PrefabSceneManager.<>c__DisplayClass8_0.<<CreateSceneAnchors>b__0>d <<CreateSceneAnchors>b__0>d;
			<<CreateSceneAnchors>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<CreateSceneAnchors>b__0>d.<>4__this = CS$<>8__locals1;
			<<CreateSceneAnchors>b__0>d.anchor = anchor;
			<<CreateSceneAnchors>b__0>d.<>1__state = -1;
			<<CreateSceneAnchors>b__0>d.<>t__builder.Start<PrefabSceneManager.<>c__DisplayClass8_0.<<CreateSceneAnchors>b__0>d>(ref <<CreateSceneAnchors>b__0>d);
			return <<CreateSceneAnchors>b__0>d.<>t__builder.Task;
		}).ToList<Task>());
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
	private async Task <LoadSceneAsync>b__7_0(OVRAnchor room)
	{
		GameObject roomObject = new GameObject(string.Format("Room-{0}", room.Uuid));
		OVRAnchorContainer ovranchorContainer;
		if (room.TryGetComponent<OVRAnchorContainer>(out ovranchorContainer))
		{
			OVRRoomLayout roomLayout;
			if (room.TryGetComponent<OVRRoomLayout>(out roomLayout))
			{
				List<OVRAnchor> children = new List<OVRAnchor>();
				await ovranchorContainer.FetchChildrenAsync(children);
				await this.CreateSceneAnchors(roomObject, roomLayout, children);
			}
		}
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
			int num2;
			int num = num2;
			PrefabSceneManager prefabSceneManager = this;
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
						goto IL_144;
					}
					roomObject = new GameObject(string.Format("Room-{0}", room.Uuid));
					OVRAnchorContainer ovranchorContainer;
					if (!room.TryGetComponent<OVRAnchorContainer>(out ovranchorContainer))
					{
						goto IL_174;
					}
					if (!room.TryGetComponent<OVRRoomLayout>(out roomLayout))
					{
						goto IL_174;
					}
					children = new List<OVRAnchor>();
					awaiter = ovranchorContainer.FetchChildrenAsync(children).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num2 = 0;
						OVRTask<bool>.Awaiter awaiter2 = awaiter;
						this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, PrefabSceneManager.<<LoadSceneAsync>b__7_0>d>(ref awaiter, ref this);
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
				taskAwaiter = prefabSceneManager.CreateSceneAnchors(roomObject, roomLayout, children).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					num2 = 1;
					TaskAwaiter taskAwaiter2 = taskAwaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, PrefabSceneManager.<<LoadSceneAsync>b__7_0>d>(ref taskAwaiter, ref this);
					return;
				}
				IL_144:
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
			IL_174:
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

		internal async Task <CreateSceneAnchors>b__0(OVRAnchor anchor)
		{
			OVRLocatable locatable;
			if (anchor.TryGetComponent<OVRLocatable>(out locatable))
			{
				await locatable.SetEnabledAsync(true, 0.0);
				GameObject gameObject = this.<>4__this.FallbackPrefab;
				if (anchor.Uuid == this.floorUuid)
				{
					gameObject = this.<>4__this.FloorPrefab;
				}
				else if (anchor.Uuid == this.ceilingUuid)
				{
					gameObject = this.<>4__this.CeilingPrefab;
				}
				else if (this.wallUuids.Contains(anchor.Uuid))
				{
					gameObject = this.<>4__this.WallPrefab;
				}
				string text = "other";
				OVRSemanticLabels ovrsemanticLabels;
				if (anchor.TryGetComponent<OVRSemanticLabels>(out ovrsemanticLabels))
				{
					text = ovrsemanticLabels.Labels;
				}
				GameObject gameObject2 = new GameObject(text);
				gameObject2.transform.SetParent(this.roomGameObject.transform);
				new SceneManagerHelper(gameObject2).SetLocation(locatable, null);
				GameObject gameObject3 = Object.Instantiate<GameObject>(gameObject, gameObject2.transform);
				OVRBounded2D ovrbounded2D;
				if (anchor.TryGetComponent<OVRBounded2D>(out ovrbounded2D) && ovrbounded2D.IsEnabled)
				{
					gameObject3.transform.localScale = new Vector3(ovrbounded2D.BoundingBox.size.x, ovrbounded2D.BoundingBox.size.y, 0.01f);
				}
				OVRBounded3D ovrbounded3D;
				if (gameObject == this.<>4__this.FallbackPrefab && anchor.TryGetComponent<OVRBounded3D>(out ovrbounded3D) && ovrbounded3D.IsEnabled)
				{
					gameObject3.transform.localPosition = new Vector3(0f, 0f, -ovrbounded3D.BoundingBox.size.z / 2f);
					gameObject3.transform.localScale = ovrbounded3D.BoundingBox.size;
				}
				this.<>4__this._locatableObjects.Add(new ValueTuple<GameObject, OVRLocatable>(gameObject2, locatable));
			}
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
				int num2;
				int num = num2;
				PrefabSceneManager.<>c__DisplayClass8_0 CS$<>8__locals1 = this;
				try
				{
					OVRTask<bool>.Awaiter awaiter;
					if (num != 0)
					{
						if (!anchor.TryGetComponent<OVRLocatable>(out locatable))
						{
							goto IL_283;
						}
						awaiter = locatable.SetEnabledAsync(true, 0.0).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num2 = 0;
							OVRTask<bool>.Awaiter awaiter2 = awaiter;
							this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, PrefabSceneManager.<>c__DisplayClass8_0.<<CreateSceneAnchors>b__0>d>(ref awaiter, ref this);
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
					GameObject gameObject = CS$<>8__locals1.<>4__this.FallbackPrefab;
					if (anchor.Uuid == CS$<>8__locals1.floorUuid)
					{
						gameObject = CS$<>8__locals1.<>4__this.FloorPrefab;
					}
					else if (anchor.Uuid == CS$<>8__locals1.ceilingUuid)
					{
						gameObject = CS$<>8__locals1.<>4__this.CeilingPrefab;
					}
					else if (CS$<>8__locals1.wallUuids.Contains(anchor.Uuid))
					{
						gameObject = CS$<>8__locals1.<>4__this.WallPrefab;
					}
					string text = "other";
					OVRSemanticLabels ovrsemanticLabels;
					if (anchor.TryGetComponent<OVRSemanticLabels>(out ovrsemanticLabels))
					{
						text = ovrsemanticLabels.Labels;
					}
					GameObject gameObject2 = new GameObject(text);
					gameObject2.transform.SetParent(CS$<>8__locals1.roomGameObject.transform);
					new SceneManagerHelper(gameObject2).SetLocation(locatable, null);
					GameObject gameObject3 = Object.Instantiate<GameObject>(gameObject, gameObject2.transform);
					OVRBounded2D ovrbounded2D;
					if (anchor.TryGetComponent<OVRBounded2D>(out ovrbounded2D) && ovrbounded2D.IsEnabled)
					{
						gameObject3.transform.localScale = new Vector3(ovrbounded2D.BoundingBox.size.x, ovrbounded2D.BoundingBox.size.y, 0.01f);
					}
					OVRBounded3D ovrbounded3D;
					if (gameObject == CS$<>8__locals1.<>4__this.FallbackPrefab && anchor.TryGetComponent<OVRBounded3D>(out ovrbounded3D) && ovrbounded3D.IsEnabled)
					{
						gameObject3.transform.localPosition = new Vector3(0f, 0f, -ovrbounded3D.BoundingBox.size.z / 2f);
						gameObject3.transform.localScale = ovrbounded3D.BoundingBox.size;
					}
					CS$<>8__locals1.<>4__this._locatableObjects.Add(new ValueTuple<GameObject, OVRLocatable>(gameObject2, locatable));
				}
				catch (Exception ex)
				{
					num2 = -2;
					this.<>t__builder.SetException(ex);
					return;
				}
				IL_283:
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
			int num2;
			int num = num2;
			try
			{
				TaskAwaiter taskAwaiter;
				if (num != 0)
				{
					PrefabSceneManager.<>c__DisplayClass8_0 CS$<>8__locals1 = new PrefabSceneManager.<>c__DisplayClass8_0();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.roomGameObject = roomGameObject;
					roomLayout.TryGetRoomLayout(out CS$<>8__locals1.ceilingUuid, out CS$<>8__locals1.floorUuid, out CS$<>8__locals1.wallUuids);
					taskAwaiter = Task.WhenAll(anchors.Select(delegate(OVRAnchor anchor)
					{
						PrefabSceneManager.<>c__DisplayClass8_0.<<CreateSceneAnchors>b__0>d <<CreateSceneAnchors>b__0>d;
						<<CreateSceneAnchors>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<CreateSceneAnchors>b__0>d.<>4__this = CS$<>8__locals1;
						<<CreateSceneAnchors>b__0>d.anchor = anchor;
						<<CreateSceneAnchors>b__0>d.<>1__state = -1;
						<<CreateSceneAnchors>b__0>d.<>t__builder.Start<PrefabSceneManager.<>c__DisplayClass8_0.<<CreateSceneAnchors>b__0>d>(ref <<CreateSceneAnchors>b__0>d);
						return <<CreateSceneAnchors>b__0>d.<>t__builder.Task;
					}).ToList<Task>()).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						num2 = 0;
						TaskAwaiter taskAwaiter2 = taskAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, PrefabSceneManager.<CreateSceneAnchors>d__8>(ref taskAwaiter, ref this);
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
			int num2;
			int num = num2;
			PrefabSceneManager prefabSceneManager = this;
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
						this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, PrefabSceneManager.<LoadSceneAsync>d__7>(ref awaiter, ref this);
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
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, PrefabSceneManager.<LoadSceneAsync>d__7>(ref taskAwaiter3, ref this);
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
					this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, PrefabSceneManager.<LoadSceneAsync>d__7>(ref awaiter, ref this);
					return;
				}
				IL_181:
				awaiter.GetResult();
				IL_189:
				taskAwaiter4 = Task.WhenAll(rooms.Select(delegate(OVRAnchor room)
				{
					PrefabSceneManager.<<LoadSceneAsync>b__7_0>d <<LoadSceneAsync>b__7_0>d;
					<<LoadSceneAsync>b__7_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<LoadSceneAsync>b__7_0>d.<>4__this = prefabSceneManager;
					<<LoadSceneAsync>b__7_0>d.room = room;
					<<LoadSceneAsync>b__7_0>d.<>1__state = -1;
					<<LoadSceneAsync>b__7_0>d.<>t__builder.Start<PrefabSceneManager.<<LoadSceneAsync>b__7_0>d>(ref <<LoadSceneAsync>b__7_0>d);
					return <<LoadSceneAsync>b__7_0>d.<>t__builder.Task;
				}).ToList<Task>()).GetAwaiter();
				if (!taskAwaiter4.IsCompleted)
				{
					num2 = 3;
					TaskAwaiter taskAwaiter5 = taskAwaiter4;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, PrefabSceneManager.<LoadSceneAsync>d__7>(ref taskAwaiter4, ref this);
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
