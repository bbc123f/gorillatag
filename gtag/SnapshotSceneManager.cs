using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SnapshotSceneManager : MonoBehaviour
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
			this.UpdateScene();
		}
		yield break;
	}

	private async void UpdateScene()
	{
		SnapshotSceneManager.SceneSnapshot sceneSnapshot = await this.LoadSceneSnapshotAsync();
		SnapshotSceneManager.SceneSnapshot currentSnapshot = sceneSnapshot;
		List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>> list = await new SnapshotSceneManager.SnapshotComparer(this._snapshot, currentSnapshot).Compare();
		StringBuilder stringBuilder = new StringBuilder();
		if (list.Count > 0)
		{
			stringBuilder.AppendLine("---- SCENE SNAPSHOT ----");
			foreach (ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType> valueTuple in list)
			{
				OVRAnchor item = valueTuple.Item1;
				stringBuilder.AppendLine(string.Format("{0}: {1}", valueTuple.Item2, this.AnchorInfo(item)));
			}
			Debug.Log(stringBuilder.ToString());
		}
		this._snapshot = currentSnapshot;
	}

	private async Task<SnapshotSceneManager.SceneSnapshot> LoadSceneSnapshotAsync()
	{
		SnapshotSceneManager.SceneSnapshot snapshot = new SnapshotSceneManager.SceneSnapshot();
		List<OVRAnchor> rooms = new List<OVRAnchor>();
		await OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms, OVRSpace.StorageLocation.Local, 1024, 0.0);
		foreach (OVRAnchor room in rooms)
		{
			OVRAnchorContainer ovranchorContainer;
			if (room.TryGetComponent<OVRAnchorContainer>(out ovranchorContainer))
			{
				List<OVRAnchor> children = new List<OVRAnchor>();
				await ovranchorContainer.FetchChildrenAsync(children);
				snapshot.Anchors.Add(room);
				snapshot.Anchors.AddRange(children);
				children = null;
				room = default(OVRAnchor);
			}
		}
		List<OVRAnchor>.Enumerator enumerator = default(List<OVRAnchor>.Enumerator);
		return snapshot;
	}

	private string AnchorInfo(OVRAnchor anchor)
	{
		OVRRoomLayout ovrroomLayout;
		if (anchor.TryGetComponent<OVRRoomLayout>(out ovrroomLayout) && ovrroomLayout.IsEnabled)
		{
			return string.Format("{0} - ROOM", anchor.Uuid);
		}
		OVRSemanticLabels ovrsemanticLabels;
		if (anchor.TryGetComponent<OVRSemanticLabels>(out ovrsemanticLabels) && ovrsemanticLabels.IsEnabled)
		{
			return string.Format("{0} - {1}", anchor.Uuid, ovrsemanticLabels.Labels);
		}
		return string.Format("{0}", anchor.Uuid);
	}

	public SnapshotSceneManager()
	{
	}

	public float UpdateFrequencySeconds = 5f;

	private SnapshotSceneManager.SceneSnapshot _snapshot = new SnapshotSceneManager.SceneSnapshot();

	private class SceneSnapshot
	{
		public List<OVRAnchor> Anchors
		{
			[CompilerGenerated]
			get
			{
				return this.<Anchors>k__BackingField;
			}
		} = new List<OVRAnchor>();

		public SceneSnapshot()
		{
		}

		[CompilerGenerated]
		private readonly List<OVRAnchor> <Anchors>k__BackingField;
	}

	private class SnapshotComparer
	{
		public SnapshotSceneManager.SceneSnapshot BaseSnapshot
		{
			[CompilerGenerated]
			get
			{
				return this.<BaseSnapshot>k__BackingField;
			}
		}

		public SnapshotSceneManager.SceneSnapshot NewSnapshot
		{
			[CompilerGenerated]
			get
			{
				return this.<NewSnapshot>k__BackingField;
			}
		}

		public SnapshotComparer(SnapshotSceneManager.SceneSnapshot baseSnapshot, SnapshotSceneManager.SceneSnapshot newSnapshot)
		{
			this.BaseSnapshot = baseSnapshot;
			this.NewSnapshot = newSnapshot;
		}

		public async Task<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>> Compare()
		{
			List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>> changes = new List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>();
			foreach (OVRAnchor ovranchor in this.BaseSnapshot.Anchors)
			{
				if (!this.NewSnapshot.Anchors.Contains(ovranchor))
				{
					changes.Add(new ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>(ovranchor, SnapshotSceneManager.SnapshotComparer.ChangeType.Missing));
				}
			}
			foreach (OVRAnchor ovranchor2 in this.NewSnapshot.Anchors)
			{
				if (!this.BaseSnapshot.Anchors.Contains(ovranchor2))
				{
					changes.Add(new ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>(ovranchor2, SnapshotSceneManager.SnapshotComparer.ChangeType.New));
				}
			}
			await this.CheckRoomChanges(changes);
			return changes;
		}

		private async Task CheckRoomChanges(List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>> changes)
		{
			for (int i = 0; i < changes.Count; i++)
			{
				ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType> valueTuple = changes[i];
				OVRAnchor anchor = valueTuple.Item1;
				SnapshotSceneManager.SnapshotComparer.ChangeType change = valueTuple.Item2;
				OVRRoomLayout ovrroomLayout;
				if (anchor.TryGetComponent<OVRRoomLayout>(out ovrroomLayout) && ovrroomLayout.IsEnabled && change != SnapshotSceneManager.SnapshotComparer.ChangeType.Changed)
				{
					List<OVRAnchor> childAnchors = new List<OVRAnchor>();
					OVRTask<bool>.Awaiter awaiter = ovrroomLayout.FetchLayoutAnchorsAsync(childAnchors).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						await awaiter;
						OVRTask<bool>.Awaiter awaiter2;
						awaiter = awaiter2;
						awaiter2 = default(OVRTask<bool>.Awaiter);
					}
					if (awaiter.GetResult())
					{
						List<OVRAnchor> list = ((change == SnapshotSceneManager.SnapshotComparer.ChangeType.New) ? this.BaseSnapshot.Anchors : this.NewSnapshot.Anchors);
						using (List<OVRAnchor>.Enumerator enumerator = childAnchors.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (list.Contains(enumerator.Current))
								{
									changes[i] = new ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>(anchor, SnapshotSceneManager.SnapshotComparer.ChangeType.Changed);
								}
							}
						}
						anchor = default(OVRAnchor);
						childAnchors = null;
					}
				}
			}
		}

		[CompilerGenerated]
		private readonly SnapshotSceneManager.SceneSnapshot <BaseSnapshot>k__BackingField;

		[CompilerGenerated]
		private readonly SnapshotSceneManager.SceneSnapshot <NewSnapshot>k__BackingField;

		public enum ChangeType
		{
			New,
			Missing,
			Changed
		}

		[CompilerGenerated]
		[StructLayout(LayoutKind.Auto)]
		private struct <CheckRoomChanges>d__9 : IAsyncStateMachine
		{
			void IAsyncStateMachine.MoveNext()
			{
				int num2;
				int num = num2;
				SnapshotSceneManager.SnapshotComparer snapshotComparer = this;
				try
				{
					if (num != 0)
					{
						i = 0;
						goto IL_192;
					}
					OVRTask<bool>.Awaiter awaiter3 = awaiter2;
					awaiter2 = default(OVRTask<bool>.Awaiter);
					num = (num2 = -1);
					IL_E4:
					if (awaiter3.GetResult())
					{
						List<OVRAnchor> list = ((change == SnapshotSceneManager.SnapshotComparer.ChangeType.New) ? snapshotComparer.BaseSnapshot.Anchors : snapshotComparer.NewSnapshot.Anchors);
						List<OVRAnchor>.Enumerator enumerator = childAnchors.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								OVRAnchor ovranchor = enumerator.Current;
								if (list.Contains(ovranchor))
								{
									changes[i] = new ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>(anchor, SnapshotSceneManager.SnapshotComparer.ChangeType.Changed);
								}
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator).Dispose();
							}
						}
						anchor = default(OVRAnchor);
						childAnchors = null;
					}
					IL_180:
					int num3 = i;
					i = num3 + 1;
					IL_192:
					if (i < changes.Count)
					{
						ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType> valueTuple = changes[i];
						anchor = valueTuple.Item1;
						change = valueTuple.Item2;
						OVRRoomLayout ovrroomLayout;
						if (!anchor.TryGetComponent<OVRRoomLayout>(out ovrroomLayout) || !ovrroomLayout.IsEnabled || change == SnapshotSceneManager.SnapshotComparer.ChangeType.Changed)
						{
							goto IL_180;
						}
						childAnchors = new List<OVRAnchor>();
						awaiter3 = ovrroomLayout.FetchLayoutAnchorsAsync(childAnchors).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = (num2 = 0);
							awaiter2 = awaiter3;
							this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, SnapshotSceneManager.SnapshotComparer.<CheckRoomChanges>d__9>(ref awaiter3, ref this);
							return;
						}
						goto IL_E4;
					}
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

			public List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>> changes;

			public SnapshotSceneManager.SnapshotComparer <>4__this;

			private int <i>5__2;

			private OVRAnchor <anchor>5__3;

			private SnapshotSceneManager.SnapshotComparer.ChangeType <change>5__4;

			private List<OVRAnchor> <childAnchors>5__5;

			private OVRTask<bool>.Awaiter <>u__1;
		}

		[CompilerGenerated]
		[StructLayout(LayoutKind.Auto)]
		private struct <Compare>d__8 : IAsyncStateMachine
		{
			void IAsyncStateMachine.MoveNext()
			{
				int num2;
				int num = num2;
				SnapshotSceneManager.SnapshotComparer snapshotComparer = this;
				List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>> list;
				try
				{
					TaskAwaiter taskAwaiter;
					if (num != 0)
					{
						changes = new List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>();
						List<OVRAnchor>.Enumerator enumerator = snapshotComparer.BaseSnapshot.Anchors.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								OVRAnchor ovranchor = enumerator.Current;
								if (!snapshotComparer.NewSnapshot.Anchors.Contains(ovranchor))
								{
									changes.Add(new ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>(ovranchor, SnapshotSceneManager.SnapshotComparer.ChangeType.Missing));
								}
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator).Dispose();
							}
						}
						enumerator = snapshotComparer.NewSnapshot.Anchors.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								OVRAnchor ovranchor2 = enumerator.Current;
								if (!snapshotComparer.BaseSnapshot.Anchors.Contains(ovranchor2))
								{
									changes.Add(new ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>(ovranchor2, SnapshotSceneManager.SnapshotComparer.ChangeType.New));
								}
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator).Dispose();
							}
						}
						taskAwaiter = snapshotComparer.CheckRoomChanges(changes).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							num = (num2 = 0);
							TaskAwaiter taskAwaiter2 = taskAwaiter;
							this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, SnapshotSceneManager.SnapshotComparer.<Compare>d__8>(ref taskAwaiter, ref this);
							return;
						}
					}
					else
					{
						TaskAwaiter taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter);
						num = (num2 = -1);
					}
					taskAwaiter.GetResult();
					list = changes;
				}
				catch (Exception ex)
				{
					num2 = -2;
					changes = null;
					this.<>t__builder.SetException(ex);
					return;
				}
				num2 = -2;
				changes = null;
				this.<>t__builder.SetResult(list);
			}

			[DebuggerHidden]
			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				this.<>t__builder.SetStateMachine(stateMachine);
			}

			public int <>1__state;

			public AsyncTaskMethodBuilder<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>> <>t__builder;

			public SnapshotSceneManager.SnapshotComparer <>4__this;

			private List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>> <changes>5__2;

			private TaskAwaiter <>u__1;
		}
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <LoadSceneSnapshotAsync>d__6 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			SnapshotSceneManager.SceneSnapshot sceneSnapshot;
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
					snapshot = new SnapshotSceneManager.SceneSnapshot();
					rooms = new List<OVRAnchor>();
					awaiter = OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(rooms, OVRSpace.StorageLocation.Local, 1024, 0.0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (num2 = 0);
						awaiter2 = awaiter;
						this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, SnapshotSceneManager.<LoadSceneSnapshotAsync>d__6>(ref awaiter, ref this);
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
					IL_188:
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
								this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, SnapshotSceneManager.<LoadSceneSnapshotAsync>d__6>(ref awaiter, ref this);
								return;
							}
							goto IL_141;
						}
					}
					goto IL_1B0;
					IL_141:
					awaiter.GetResult();
					snapshot.Anchors.Add(room);
					snapshot.Anchors.AddRange(children);
					children = null;
					room = default(OVRAnchor);
					goto IL_188;
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				IL_1B0:
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

		public AsyncTaskMethodBuilder<SnapshotSceneManager.SceneSnapshot> <>t__builder;

		private SnapshotSceneManager.SceneSnapshot <snapshot>5__2;

		private List<OVRAnchor> <rooms>5__3;

		private OVRTask<bool>.Awaiter <>u__1;

		private List<OVRAnchor>.Enumerator <>7__wrap3;

		private OVRAnchor <room>5__5;

		private List<OVRAnchor> <children>5__6;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <UpdateScene>d__5 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			SnapshotSceneManager snapshotSceneManager = this;
			try
			{
				TaskAwaiter<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>> taskAwaiter;
				TaskAwaiter<SnapshotSceneManager.SceneSnapshot> taskAwaiter3;
				if (num != 0)
				{
					if (num == 1)
					{
						TaskAwaiter<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>>);
						num = (num2 = -1);
						goto IL_E7;
					}
					taskAwaiter3 = snapshotSceneManager.LoadSceneSnapshotAsync().GetAwaiter();
					if (!taskAwaiter3.IsCompleted)
					{
						num = (num2 = 0);
						TaskAwaiter<SnapshotSceneManager.SceneSnapshot> taskAwaiter4 = taskAwaiter3;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<SnapshotSceneManager.SceneSnapshot>, SnapshotSceneManager.<UpdateScene>d__5>(ref taskAwaiter3, ref this);
						return;
					}
				}
				else
				{
					TaskAwaiter<SnapshotSceneManager.SceneSnapshot> taskAwaiter4;
					taskAwaiter3 = taskAwaiter4;
					taskAwaiter4 = default(TaskAwaiter<SnapshotSceneManager.SceneSnapshot>);
					num = (num2 = -1);
				}
				SnapshotSceneManager.SceneSnapshot result = taskAwaiter3.GetResult();
				currentSnapshot = result;
				taskAwaiter = new SnapshotSceneManager.SnapshotComparer(snapshotSceneManager._snapshot, currentSnapshot).Compare().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					num = (num2 = 1);
					TaskAwaiter<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>> taskAwaiter2 = taskAwaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>>, SnapshotSceneManager.<UpdateScene>d__5>(ref taskAwaiter, ref this);
					return;
				}
				IL_E7:
				List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>> result2 = taskAwaiter.GetResult();
				StringBuilder stringBuilder = new StringBuilder();
				if (result2.Count > 0)
				{
					stringBuilder.AppendLine("---- SCENE SNAPSHOT ----");
					List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>.Enumerator enumerator = result2.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType> valueTuple = enumerator.Current;
							OVRAnchor item = valueTuple.Item1;
							SnapshotSceneManager.SnapshotComparer.ChangeType item2 = valueTuple.Item2;
							stringBuilder.AppendLine(string.Format("{0}: {1}", item2, snapshotSceneManager.AnchorInfo(item)));
						}
					}
					finally
					{
						if (num < 0)
						{
							((IDisposable)enumerator).Dispose();
						}
					}
					Debug.Log(stringBuilder.ToString());
				}
				snapshotSceneManager._snapshot = currentSnapshot;
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

		public AsyncVoidMethodBuilder <>t__builder;

		public SnapshotSceneManager <>4__this;

		private SnapshotSceneManager.SceneSnapshot <currentSnapshot>5__2;

		private TaskAwaiter<SnapshotSceneManager.SceneSnapshot> <>u__1;

		private TaskAwaiter<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>> <>u__2;
	}

	[CompilerGenerated]
	private sealed class <UpdateScenePeriodically>d__4 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <UpdateScenePeriodically>d__4(int <>1__state)
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
			SnapshotSceneManager snapshotSceneManager = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				snapshotSceneManager.UpdateScene();
			}
			else
			{
				this.<>1__state = -1;
			}
			this.<>2__current = new WaitForSeconds(snapshotSceneManager.UpdateFrequencySeconds);
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

		public SnapshotSceneManager <>4__this;
	}
}
