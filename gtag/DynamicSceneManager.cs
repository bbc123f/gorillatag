using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DynamicSceneManagerHelper;
using UnityEngine;

public class DynamicSceneManager : MonoBehaviour
{
	private void Start()
	{
		SceneManagerHelper.RequestScenePermission();
		base.StartCoroutine(this.UpdateScenePeriodically());
	}

	private void Update()
	{
		if (OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.Active))
		{
			SceneManagerHelper.RequestSceneCapture();
		}
	}

	private IEnumerator UpdateScenePeriodically()
	{
		for (;;)
		{
			yield return new WaitForSeconds(this.UpdateFrequencySeconds);
			this._updateSceneTask = this.UpdateScene();
			yield return new WaitUntil(() => this._updateSceneTask.IsCompleted);
		}
		yield break;
	}

	private async Task UpdateScene()
	{
		SceneSnapshot sceneSnapshot = await this.LoadSceneSnapshotAsync();
		SceneSnapshot currentSnapshot = sceneSnapshot;
		List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> list = new SnapshotComparer(this._snapshot, currentSnapshot).Compare();
		await this.UpdateUnityObjects(list, currentSnapshot);
		this._snapshot = currentSnapshot;
	}

	private async Task<SceneSnapshot> LoadSceneSnapshotAsync()
	{
		SceneSnapshot snapshot = new SceneSnapshot();
		List<OVRAnchor> rooms = new List<OVRAnchor>();
		await OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms, OVRSpace.StorageLocation.Local, 1024, 0.0);
		foreach (OVRAnchor room in rooms)
		{
			OVRAnchorContainer ovranchorContainer;
			if (room.TryGetComponent<OVRAnchorContainer>(out ovranchorContainer))
			{
				List<OVRAnchor> children = new List<OVRAnchor>();
				await ovranchorContainer.FetchChildrenAsync(children);
				snapshot.Anchors.Add(room, new SceneSnapshot.Data
				{
					Children = children
				});
				foreach (OVRAnchor ovranchor in children)
				{
					SceneSnapshot.Data data = new SceneSnapshot.Data();
					OVRBounded2D ovrbounded2D;
					if (ovranchor.TryGetComponent<OVRBounded2D>(out ovrbounded2D) && ovrbounded2D.IsEnabled)
					{
						data.Rect = new Rect?(ovrbounded2D.BoundingBox);
					}
					OVRBounded3D ovrbounded3D;
					if (ovranchor.TryGetComponent<OVRBounded3D>(out ovrbounded3D) && ovrbounded3D.IsEnabled)
					{
						data.Bounds = new Bounds?(ovrbounded3D.BoundingBox);
					}
					snapshot.Anchors.Add(ovranchor, data);
				}
				children = null;
				room = default(OVRAnchor);
			}
		}
		List<OVRAnchor>.Enumerator enumerator = default(List<OVRAnchor>.Enumerator);
		return snapshot;
	}

	private async Task UpdateUnityObjects(List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> changes, SceneSnapshot newSnapshot)
	{
		if (changes.Any<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>>())
		{
			UnityObjectUpdater updater = new UnityObjectUpdater();
			List<OVRAnchor> list = this.FilterChanges(changes, SnapshotComparer.ChangeType.New);
			List<OVRAnchor> changesMissing = this.FilterChanges(changes, SnapshotComparer.ChangeType.Missing);
			List<OVRAnchor> changesId = this.FilterChanges(changes, SnapshotComparer.ChangeType.ChangedId);
			List<OVRAnchor> changesBounds = this.FilterChanges(changes, SnapshotComparer.ChangeType.ChangedBounds);
			foreach (OVRAnchor ovranchor in list)
			{
				GameObject gameObject;
				this._sceneGameObjects.TryGetValue(this.GetParentAnchor(ovranchor, newSnapshot), out gameObject);
				Dictionary<OVRAnchor, GameObject> dictionary = this._sceneGameObjects;
				OVRAnchor ovranchor2 = ovranchor;
				GameObject gameObject2 = await updater.CreateUnityObject(ovranchor, gameObject);
				dictionary.Add(ovranchor2, gameObject2);
				dictionary = null;
				ovranchor2 = default(OVRAnchor);
			}
			List<OVRAnchor>.Enumerator enumerator = default(List<OVRAnchor>.Enumerator);
			foreach (OVRAnchor ovranchor3 in changesMissing)
			{
				Object.Destroy(this._sceneGameObjects[ovranchor3]);
				this._sceneGameObjects.Remove(ovranchor3);
			}
			foreach (ValueTuple<OVRAnchor, OVRAnchor> valueTuple in this.FindAnchorPairs(changesId, newSnapshot))
			{
				OVRAnchor item = valueTuple.Item1;
				OVRAnchor item2 = valueTuple.Item2;
				this._sceneGameObjects.Add(item2, this._sceneGameObjects[item]);
				this._sceneGameObjects.Remove(item);
			}
			foreach (OVRAnchor ovranchor4 in changesBounds)
			{
				updater.UpdateUnityObject(ovranchor4, this._sceneGameObjects[ovranchor4]);
			}
		}
	}

	private List<OVRAnchor> FilterChanges(List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> changes, SnapshotComparer.ChangeType changeType)
	{
		return (from tuple in changes
			where tuple.Item2 == changeType
			select tuple.Item1).ToList<OVRAnchor>();
	}

	private List<ValueTuple<OVRAnchor, OVRAnchor>> FindAnchorPairs(List<OVRAnchor> allAnchors, SceneSnapshot newSnapshot)
	{
		IEnumerable<OVRAnchor> enumerable = allAnchors.Where(new Func<OVRAnchor, bool>(this._snapshot.Contains));
		IEnumerable<OVRAnchor> enumerable2 = allAnchors.Where(new Func<OVRAnchor, bool>(newSnapshot.Contains));
		List<ValueTuple<OVRAnchor, OVRAnchor>> list = new List<ValueTuple<OVRAnchor, OVRAnchor>>();
		foreach (OVRAnchor ovranchor in enumerable)
		{
			foreach (OVRAnchor ovranchor2 in enumerable2)
			{
				if (this.AreAnchorsEqual(this._snapshot.Anchors[ovranchor], newSnapshot.Anchors[ovranchor2]))
				{
					list.Add(new ValueTuple<OVRAnchor, OVRAnchor>(ovranchor, ovranchor2));
					break;
				}
			}
		}
		return list;
	}

	private bool AreAnchorsEqual(SceneSnapshot.Data anchor1Data, SceneSnapshot.Data anchor2Data)
	{
		return anchor1Data.Children != null && anchor2Data.Children != null && (anchor1Data.Children.Any(new Func<OVRAnchor, bool>(anchor2Data.Children.Contains)) || anchor2Data.Children.Any(new Func<OVRAnchor, bool>(anchor1Data.Children.Contains)));
	}

	private OVRAnchor GetParentAnchor(OVRAnchor childAnchor, SceneSnapshot snapshot)
	{
		foreach (KeyValuePair<OVRAnchor, SceneSnapshot.Data> keyValuePair in snapshot.Anchors)
		{
			List<OVRAnchor> children = keyValuePair.Value.Children;
			if (children != null && children.Contains(childAnchor))
			{
				return keyValuePair.Key;
			}
		}
		return OVRAnchor.Null;
	}

	public DynamicSceneManager()
	{
	}

	[CompilerGenerated]
	private bool <UpdateScenePeriodically>b__6_0()
	{
		return this._updateSceneTask.IsCompleted;
	}

	public float UpdateFrequencySeconds = 5f;

	private SceneSnapshot _snapshot = new SceneSnapshot();

	private Dictionary<OVRAnchor, GameObject> _sceneGameObjects = new Dictionary<OVRAnchor, GameObject>();

	private Task _updateSceneTask;

	[CompilerGenerated]
	[Serializable]
	private sealed class <>c
	{
		// Note: this type is marked as 'beforefieldinit'.
		static <>c()
		{
		}

		public <>c()
		{
		}

		internal OVRAnchor <FilterChanges>b__10_1(ValueTuple<OVRAnchor, SnapshotComparer.ChangeType> tuple)
		{
			return tuple.Item1;
		}

		public static readonly DynamicSceneManager.<>c <>9 = new DynamicSceneManager.<>c();

		public static Func<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>, OVRAnchor> <>9__10_1;
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass10_0
	{
		public <>c__DisplayClass10_0()
		{
		}

		internal bool <FilterChanges>b__0(ValueTuple<OVRAnchor, SnapshotComparer.ChangeType> tuple)
		{
			return tuple.Item2 == this.changeType;
		}

		public SnapshotComparer.ChangeType changeType;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <LoadSceneSnapshotAsync>d__8 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			SceneSnapshot sceneSnapshot;
			try
			{
				OVRTask<bool>.Awaiter awaiter;
				OVRTask<bool>.Awaiter awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_AB;
					}
					snapshot = new SceneSnapshot();
					rooms = new List<OVRAnchor>();
					awaiter = OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms, OVRSpace.StorageLocation.Local, 1024, 0.0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (num2 = 0);
						awaiter2 = awaiter;
						this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, DynamicSceneManager.<LoadSceneSnapshotAsync>d__8>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = awaiter2;
					awaiter2 = default(OVRTask<bool>.Awaiter);
					num = (num2 = -1);
				}
				awaiter.GetResult();
				enumerator = rooms.GetEnumerator();
				IL_AB:
				try
				{
					if (num == 1)
					{
						awaiter = awaiter2;
						awaiter2 = default(OVRTask<bool>.Awaiter);
						num = (num2 = -1);
						goto IL_141;
					}
					IL_221:
					while (enumerator.MoveNext())
					{
						room = enumerator.Current;
						OVRAnchorContainer ovranchorContainer;
						if (room.TryGetComponent<OVRAnchorContainer>(out ovranchorContainer))
						{
							children = new List<OVRAnchor>();
							awaiter = ovranchorContainer.FetchChildrenAsync(children).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (num2 = 1);
								awaiter2 = awaiter;
								this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, DynamicSceneManager.<LoadSceneSnapshotAsync>d__8>(ref awaiter, ref this);
								return;
							}
							goto IL_141;
						}
					}
					goto IL_249;
					IL_141:
					awaiter.GetResult();
					snapshot.Anchors.Add(room, new SceneSnapshot.Data
					{
						Children = children
					});
					List<OVRAnchor>.Enumerator enumerator2 = children.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							OVRAnchor ovranchor = enumerator2.Current;
							SceneSnapshot.Data data = new SceneSnapshot.Data();
							OVRBounded2D ovrbounded2D;
							if (ovranchor.TryGetComponent<OVRBounded2D>(out ovrbounded2D) && ovrbounded2D.IsEnabled)
							{
								data.Rect = new Rect?(ovrbounded2D.BoundingBox);
							}
							OVRBounded3D ovrbounded3D;
							if (ovranchor.TryGetComponent<OVRBounded3D>(out ovrbounded3D) && ovrbounded3D.IsEnabled)
							{
								data.Bounds = new Bounds?(ovrbounded3D.BoundingBox);
							}
							snapshot.Anchors.Add(ovranchor, data);
						}
					}
					finally
					{
						if (num < 0)
						{
							((IDisposable)enumerator2).Dispose();
						}
					}
					children = null;
					room = default(OVRAnchor);
					goto IL_221;
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				IL_249:
				enumerator = default(List<OVRAnchor>.Enumerator);
				sceneSnapshot = snapshot;
			}
			catch (Exception ex)
			{
				num2 = -2;
				snapshot = null;
				rooms = null;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
			snapshot = null;
			rooms = null;
			this.<>t__builder.SetResult(sceneSnapshot);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<SceneSnapshot> <>t__builder;

		private SceneSnapshot <snapshot>5__2;

		private List<OVRAnchor> <rooms>5__3;

		private OVRTask<bool>.Awaiter <>u__1;

		private List<OVRAnchor>.Enumerator <>7__wrap3;

		private OVRAnchor <room>5__5;

		private List<OVRAnchor> <children>5__6;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <UpdateScene>d__7 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			DynamicSceneManager dynamicSceneManager = this;
			try
			{
				TaskAwaiter taskAwaiter;
				TaskAwaiter<SceneSnapshot> taskAwaiter3;
				if (num != 0)
				{
					if (num == 1)
					{
						TaskAwaiter taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter);
						num2 = -1;
						goto IL_F0;
					}
					taskAwaiter3 = dynamicSceneManager.LoadSceneSnapshotAsync().GetAwaiter();
					if (!taskAwaiter3.IsCompleted)
					{
						num2 = 0;
						TaskAwaiter<SceneSnapshot> taskAwaiter4 = taskAwaiter3;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<SceneSnapshot>, DynamicSceneManager.<UpdateScene>d__7>(ref taskAwaiter3, ref this);
						return;
					}
				}
				else
				{
					TaskAwaiter<SceneSnapshot> taskAwaiter4;
					taskAwaiter3 = taskAwaiter4;
					taskAwaiter4 = default(TaskAwaiter<SceneSnapshot>);
					num2 = -1;
				}
				SceneSnapshot result = taskAwaiter3.GetResult();
				currentSnapshot = result;
				List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> list = new SnapshotComparer(dynamicSceneManager._snapshot, currentSnapshot).Compare();
				taskAwaiter = dynamicSceneManager.UpdateUnityObjects(list, currentSnapshot).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					num2 = 1;
					TaskAwaiter taskAwaiter2 = taskAwaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, DynamicSceneManager.<UpdateScene>d__7>(ref taskAwaiter, ref this);
					return;
				}
				IL_F0:
				taskAwaiter.GetResult();
				dynamicSceneManager._snapshot = currentSnapshot;
			}
			catch (Exception ex)
			{
				num2 = -2;
				currentSnapshot = null;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
			currentSnapshot = null;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public DynamicSceneManager <>4__this;

		private SceneSnapshot <currentSnapshot>5__2;

		private TaskAwaiter<SceneSnapshot> <>u__1;

		private TaskAwaiter <>u__2;
	}

	[CompilerGenerated]
	private sealed class <UpdateScenePeriodically>d__6 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <UpdateScenePeriodically>d__6(int <>1__state)
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
			DynamicSceneManager dynamicSceneManager = this;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				break;
			case 1:
				this.<>1__state = -1;
				dynamicSceneManager._updateSceneTask = dynamicSceneManager.UpdateScene();
				this.<>2__current = new WaitUntil(() => dynamicSceneManager._updateSceneTask.IsCompleted);
				this.<>1__state = 2;
				return true;
			case 2:
				this.<>1__state = -1;
				break;
			default:
				return false;
			}
			this.<>2__current = new WaitForSeconds(dynamicSceneManager.UpdateFrequencySeconds);
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

		public DynamicSceneManager <>4__this;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <UpdateUnityObjects>d__9 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			DynamicSceneManager dynamicSceneManager = this;
			try
			{
				if (num != 0)
				{
					if (!changes.Any<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>>())
					{
						goto IL_2DA;
					}
					updater = new UnityObjectUpdater();
					List<OVRAnchor> list = dynamicSceneManager.FilterChanges(changes, SnapshotComparer.ChangeType.New);
					changesMissing = dynamicSceneManager.FilterChanges(changes, SnapshotComparer.ChangeType.Missing);
					changesId = dynamicSceneManager.FilterChanges(changes, SnapshotComparer.ChangeType.ChangedId);
					changesBounds = dynamicSceneManager.FilterChanges(changes, SnapshotComparer.ChangeType.ChangedBounds);
					enumerator = list.GetEnumerator();
				}
				try
				{
					if (num != 0)
					{
						goto IL_155;
					}
					TaskAwaiter<GameObject> taskAwaiter2;
					TaskAwaiter<GameObject> taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<GameObject>);
					num = (num2 = -1);
					IL_126:
					GameObject result = taskAwaiter.GetResult();
					dictionary.Add(ovranchor2, result);
					dictionary = null;
					ovranchor2 = default(OVRAnchor);
					IL_155:
					if (enumerator.MoveNext())
					{
						OVRAnchor ovranchor3 = enumerator.Current;
						GameObject gameObject;
						dynamicSceneManager._sceneGameObjects.TryGetValue(dynamicSceneManager.GetParentAnchor(ovranchor3, newSnapshot), out gameObject);
						dictionary = dynamicSceneManager._sceneGameObjects;
						ovranchor2 = ovranchor3;
						taskAwaiter = updater.CreateUnityObject(ovranchor3, gameObject).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							num = (num2 = 0);
							taskAwaiter2 = taskAwaiter;
							this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<GameObject>, DynamicSceneManager.<UpdateUnityObjects>d__9>(ref taskAwaiter, ref this);
							return;
						}
						goto IL_126;
					}
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				enumerator = default(List<OVRAnchor>.Enumerator);
				List<OVRAnchor>.Enumerator enumerator2 = changesMissing.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						OVRAnchor ovranchor4 = enumerator2.Current;
						Object.Destroy(dynamicSceneManager._sceneGameObjects[ovranchor4]);
						dynamicSceneManager._sceneGameObjects.Remove(ovranchor4);
					}
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)enumerator2).Dispose();
					}
				}
				List<ValueTuple<OVRAnchor, OVRAnchor>>.Enumerator enumerator3 = dynamicSceneManager.FindAnchorPairs(changesId, newSnapshot).GetEnumerator();
				try
				{
					while (enumerator3.MoveNext())
					{
						ValueTuple<OVRAnchor, OVRAnchor> valueTuple = enumerator3.Current;
						OVRAnchor item = valueTuple.Item1;
						OVRAnchor item2 = valueTuple.Item2;
						dynamicSceneManager._sceneGameObjects.Add(item2, dynamicSceneManager._sceneGameObjects[item]);
						dynamicSceneManager._sceneGameObjects.Remove(item);
					}
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)enumerator3).Dispose();
					}
				}
				enumerator2 = changesBounds.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						OVRAnchor ovranchor5 = enumerator2.Current;
						updater.UpdateUnityObject(ovranchor5, dynamicSceneManager._sceneGameObjects[ovranchor5]);
					}
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)enumerator2).Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				num2 = -2;
				updater = null;
				changesMissing = null;
				changesId = null;
				changesBounds = null;
				this.<>t__builder.SetException(ex);
				return;
			}
			IL_2DA:
			num2 = -2;
			updater = null;
			changesMissing = null;
			changesId = null;
			changesBounds = null;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> changes;

		public DynamicSceneManager <>4__this;

		public SceneSnapshot newSnapshot;

		private UnityObjectUpdater <updater>5__2;

		private List<OVRAnchor> <changesMissing>5__3;

		private List<OVRAnchor> <changesId>5__4;

		private List<OVRAnchor> <changesBounds>5__5;

		private List<OVRAnchor>.Enumerator <>7__wrap5;

		private Dictionary<OVRAnchor, GameObject> <>7__wrap6;

		private OVRAnchor <>7__wrap7;

		private TaskAwaiter<GameObject> <>u__1;
	}
}
