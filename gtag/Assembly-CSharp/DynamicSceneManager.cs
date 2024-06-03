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

	private Task UpdateScene()
	{
		DynamicSceneManager.<UpdateScene>d__7 <UpdateScene>d__;
		<UpdateScene>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<UpdateScene>d__.<>4__this = this;
		<UpdateScene>d__.<>1__state = -1;
		<UpdateScene>d__.<>t__builder.Start<DynamicSceneManager.<UpdateScene>d__7>(ref <UpdateScene>d__);
		return <UpdateScene>d__.<>t__builder.Task;
	}

	private Task<SceneSnapshot> LoadSceneSnapshotAsync()
	{
		DynamicSceneManager.<LoadSceneSnapshotAsync>d__8 <LoadSceneSnapshotAsync>d__;
		<LoadSceneSnapshotAsync>d__.<>t__builder = AsyncTaskMethodBuilder<SceneSnapshot>.Create();
		<LoadSceneSnapshotAsync>d__.<>1__state = -1;
		<LoadSceneSnapshotAsync>d__.<>t__builder.Start<DynamicSceneManager.<LoadSceneSnapshotAsync>d__8>(ref <LoadSceneSnapshotAsync>d__);
		return <LoadSceneSnapshotAsync>d__.<>t__builder.Task;
	}

	private Task UpdateUnityObjects(List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> changes, SceneSnapshot newSnapshot)
	{
		DynamicSceneManager.<UpdateUnityObjects>d__9 <UpdateUnityObjects>d__;
		<UpdateUnityObjects>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<UpdateUnityObjects>d__.<>4__this = this;
		<UpdateUnityObjects>d__.changes = changes;
		<UpdateUnityObjects>d__.newSnapshot = newSnapshot;
		<UpdateUnityObjects>d__.<>1__state = -1;
		<UpdateUnityObjects>d__.<>t__builder.Start<DynamicSceneManager.<UpdateUnityObjects>d__9>(ref <UpdateUnityObjects>d__);
		return <UpdateUnityObjects>d__.<>t__builder.Task;
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
			int num = this.<>1__state;
			SceneSnapshot result;
			try
			{
				OVRTask<bool>.Awaiter awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_AB;
					}
					this.<snapshot>5__2 = new SceneSnapshot();
					this.<rooms>5__3 = new List<OVRAnchor>();
					awaiter = OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(this.<rooms>5__3, OVRSpace.StorageLocation.Local, 1024, 0.0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (this.<>1__state = 0);
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, DynamicSceneManager.<LoadSceneSnapshotAsync>d__8>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = this.<>u__1;
					this.<>u__1 = default(OVRTask<bool>.Awaiter);
					num = (this.<>1__state = -1);
				}
				awaiter.GetResult();
				this.<>7__wrap3 = this.<rooms>5__3.GetEnumerator();
				IL_AB:
				try
				{
					if (num == 1)
					{
						awaiter = this.<>u__1;
						this.<>u__1 = default(OVRTask<bool>.Awaiter);
						num = (this.<>1__state = -1);
						goto IL_141;
					}
					IL_221:
					while (this.<>7__wrap3.MoveNext())
					{
						this.<room>5__5 = this.<>7__wrap3.Current;
						OVRAnchorContainer ovranchorContainer;
						if (this.<room>5__5.TryGetComponent<OVRAnchorContainer>(out ovranchorContainer))
						{
							this.<children>5__6 = new List<OVRAnchor>();
							awaiter = ovranchorContainer.FetchChildrenAsync(this.<children>5__6).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (this.<>1__state = 1);
								this.<>u__1 = awaiter;
								this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, DynamicSceneManager.<LoadSceneSnapshotAsync>d__8>(ref awaiter, ref this);
								return;
							}
							goto IL_141;
						}
					}
					goto IL_249;
					IL_141:
					awaiter.GetResult();
					this.<snapshot>5__2.Anchors.Add(this.<room>5__5, new SceneSnapshot.Data
					{
						Children = this.<children>5__6
					});
					List<OVRAnchor>.Enumerator enumerator = this.<children>5__6.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							OVRAnchor key = enumerator.Current;
							SceneSnapshot.Data data = new SceneSnapshot.Data();
							OVRBounded2D ovrbounded2D;
							if (key.TryGetComponent<OVRBounded2D>(out ovrbounded2D) && ovrbounded2D.IsEnabled)
							{
								data.Rect = new Rect?(ovrbounded2D.BoundingBox);
							}
							OVRBounded3D ovrbounded3D;
							if (key.TryGetComponent<OVRBounded3D>(out ovrbounded3D) && ovrbounded3D.IsEnabled)
							{
								data.Bounds = new Bounds?(ovrbounded3D.BoundingBox);
							}
							this.<snapshot>5__2.Anchors.Add(key, data);
						}
					}
					finally
					{
						if (num < 0)
						{
							((IDisposable)enumerator).Dispose();
						}
					}
					this.<children>5__6 = null;
					this.<room>5__5 = default(OVRAnchor);
					goto IL_221;
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)this.<>7__wrap3).Dispose();
					}
				}
				IL_249:
				this.<>7__wrap3 = default(List<OVRAnchor>.Enumerator);
				result = this.<snapshot>5__2;
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<snapshot>5__2 = null;
				this.<rooms>5__3 = null;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<snapshot>5__2 = null;
			this.<rooms>5__3 = null;
			this.<>t__builder.SetResult(result);
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
			int num = this.<>1__state;
			DynamicSceneManager dynamicSceneManager = this.<>4__this;
			try
			{
				TaskAwaiter awaiter;
				TaskAwaiter<SceneSnapshot> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = this.<>u__2;
						this.<>u__2 = default(TaskAwaiter);
						this.<>1__state = -1;
						goto IL_F0;
					}
					awaiter2 = dynamicSceneManager.LoadSceneSnapshotAsync().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter2;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<SceneSnapshot>, DynamicSceneManager.<UpdateScene>d__7>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter<SceneSnapshot>);
					this.<>1__state = -1;
				}
				SceneSnapshot result = awaiter2.GetResult();
				this.<currentSnapshot>5__2 = result;
				List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> changes = new SnapshotComparer(dynamicSceneManager._snapshot, this.<currentSnapshot>5__2).Compare();
				awaiter = dynamicSceneManager.UpdateUnityObjects(changes, this.<currentSnapshot>5__2).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					this.<>1__state = 1;
					this.<>u__2 = awaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, DynamicSceneManager.<UpdateScene>d__7>(ref awaiter, ref this);
					return;
				}
				IL_F0:
				awaiter.GetResult();
				dynamicSceneManager._snapshot = this.<currentSnapshot>5__2;
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<currentSnapshot>5__2 = null;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<currentSnapshot>5__2 = null;
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
			int num = this.<>1__state;
			DynamicSceneManager dynamicSceneManager = this.<>4__this;
			try
			{
				if (num != 0)
				{
					if (!this.changes.Any<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>>())
					{
						goto IL_2DA;
					}
					this.<updater>5__2 = new UnityObjectUpdater();
					List<OVRAnchor> list = dynamicSceneManager.FilterChanges(this.changes, SnapshotComparer.ChangeType.New);
					this.<changesMissing>5__3 = dynamicSceneManager.FilterChanges(this.changes, SnapshotComparer.ChangeType.Missing);
					this.<changesId>5__4 = dynamicSceneManager.FilterChanges(this.changes, SnapshotComparer.ChangeType.ChangedId);
					this.<changesBounds>5__5 = dynamicSceneManager.FilterChanges(this.changes, SnapshotComparer.ChangeType.ChangedBounds);
					this.<>7__wrap5 = list.GetEnumerator();
				}
				try
				{
					if (num != 0)
					{
						goto IL_155;
					}
					TaskAwaiter<GameObject> awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter<GameObject>);
					num = (this.<>1__state = -1);
					IL_126:
					GameObject result = awaiter.GetResult();
					this.<>7__wrap6.Add(this.<>7__wrap7, result);
					this.<>7__wrap6 = null;
					this.<>7__wrap7 = default(OVRAnchor);
					IL_155:
					if (this.<>7__wrap5.MoveNext())
					{
						OVRAnchor ovranchor = this.<>7__wrap5.Current;
						GameObject parent;
						dynamicSceneManager._sceneGameObjects.TryGetValue(dynamicSceneManager.GetParentAnchor(ovranchor, this.newSnapshot), out parent);
						this.<>7__wrap6 = dynamicSceneManager._sceneGameObjects;
						this.<>7__wrap7 = ovranchor;
						awaiter = this.<updater>5__2.CreateUnityObject(ovranchor, parent).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (this.<>1__state = 0);
							this.<>u__1 = awaiter;
							this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<GameObject>, DynamicSceneManager.<UpdateUnityObjects>d__9>(ref awaiter, ref this);
							return;
						}
						goto IL_126;
					}
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)this.<>7__wrap5).Dispose();
					}
				}
				this.<>7__wrap5 = default(List<OVRAnchor>.Enumerator);
				List<OVRAnchor>.Enumerator enumerator = this.<changesMissing>5__3.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						OVRAnchor key = enumerator.Current;
						Object.Destroy(dynamicSceneManager._sceneGameObjects[key]);
						dynamicSceneManager._sceneGameObjects.Remove(key);
					}
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				List<ValueTuple<OVRAnchor, OVRAnchor>>.Enumerator enumerator2 = dynamicSceneManager.FindAnchorPairs(this.<changesId>5__4, this.newSnapshot).GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						ValueTuple<OVRAnchor, OVRAnchor> valueTuple = enumerator2.Current;
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
						((IDisposable)enumerator2).Dispose();
					}
				}
				enumerator = this.<changesBounds>5__5.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						OVRAnchor ovranchor2 = enumerator.Current;
						this.<updater>5__2.UpdateUnityObject(ovranchor2, dynamicSceneManager._sceneGameObjects[ovranchor2]);
					}
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)enumerator).Dispose();
					}
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<updater>5__2 = null;
				this.<changesMissing>5__3 = null;
				this.<changesId>5__4 = null;
				this.<changesBounds>5__5 = null;
				this.<>t__builder.SetException(exception);
				return;
			}
			IL_2DA:
			this.<>1__state = -2;
			this.<updater>5__2 = null;
			this.<changesMissing>5__3 = null;
			this.<changesId>5__4 = null;
			this.<changesBounds>5__5 = null;
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
