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

	private void UpdateScene()
	{
		SnapshotSceneManager.<UpdateScene>d__5 <UpdateScene>d__;
		<UpdateScene>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<UpdateScene>d__.<>4__this = this;
		<UpdateScene>d__.<>1__state = -1;
		<UpdateScene>d__.<>t__builder.Start<SnapshotSceneManager.<UpdateScene>d__5>(ref <UpdateScene>d__);
	}

	private Task<SnapshotSceneManager.SceneSnapshot> LoadSceneSnapshotAsync()
	{
		SnapshotSceneManager.<LoadSceneSnapshotAsync>d__6 <LoadSceneSnapshotAsync>d__;
		<LoadSceneSnapshotAsync>d__.<>t__builder = AsyncTaskMethodBuilder<SnapshotSceneManager.SceneSnapshot>.Create();
		<LoadSceneSnapshotAsync>d__.<>1__state = -1;
		<LoadSceneSnapshotAsync>d__.<>t__builder.Start<SnapshotSceneManager.<LoadSceneSnapshotAsync>d__6>(ref <LoadSceneSnapshotAsync>d__);
		return <LoadSceneSnapshotAsync>d__.<>t__builder.Task;
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

		public Task<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>> Compare()
		{
			SnapshotSceneManager.SnapshotComparer.<Compare>d__8 <Compare>d__;
			<Compare>d__.<>t__builder = AsyncTaskMethodBuilder<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>>.Create();
			<Compare>d__.<>4__this = this;
			<Compare>d__.<>1__state = -1;
			<Compare>d__.<>t__builder.Start<SnapshotSceneManager.SnapshotComparer.<Compare>d__8>(ref <Compare>d__);
			return <Compare>d__.<>t__builder.Task;
		}

		private Task CheckRoomChanges(List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>> changes)
		{
			SnapshotSceneManager.SnapshotComparer.<CheckRoomChanges>d__9 <CheckRoomChanges>d__;
			<CheckRoomChanges>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CheckRoomChanges>d__.<>4__this = this;
			<CheckRoomChanges>d__.changes = changes;
			<CheckRoomChanges>d__.<>1__state = -1;
			<CheckRoomChanges>d__.<>t__builder.Start<SnapshotSceneManager.SnapshotComparer.<CheckRoomChanges>d__9>(ref <CheckRoomChanges>d__);
			return <CheckRoomChanges>d__.<>t__builder.Task;
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
				int num = this.<>1__state;
				SnapshotSceneManager.SnapshotComparer snapshotComparer = this.<>4__this;
				try
				{
					if (num != 0)
					{
						this.<i>5__2 = 0;
						goto IL_192;
					}
					OVRTask<bool>.Awaiter awaiter = this.<>u__1;
					this.<>u__1 = default(OVRTask<bool>.Awaiter);
					num = (this.<>1__state = -1);
					IL_E4:
					if (awaiter.GetResult())
					{
						List<OVRAnchor> list = (this.<change>5__4 == SnapshotSceneManager.SnapshotComparer.ChangeType.New) ? snapshotComparer.BaseSnapshot.Anchors : snapshotComparer.NewSnapshot.Anchors;
						List<OVRAnchor>.Enumerator enumerator = this.<childAnchors>5__5.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								OVRAnchor item = enumerator.Current;
								if (list.Contains(item))
								{
									this.changes[this.<i>5__2] = new ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>(this.<anchor>5__3, SnapshotSceneManager.SnapshotComparer.ChangeType.Changed);
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
						this.<anchor>5__3 = default(OVRAnchor);
						this.<childAnchors>5__5 = null;
					}
					IL_180:
					int num2 = this.<i>5__2;
					this.<i>5__2 = num2 + 1;
					IL_192:
					if (this.<i>5__2 < this.changes.Count)
					{
						ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType> valueTuple = this.changes[this.<i>5__2];
						this.<anchor>5__3 = valueTuple.Item1;
						this.<change>5__4 = valueTuple.Item2;
						OVRRoomLayout ovrroomLayout;
						if (!this.<anchor>5__3.TryGetComponent<OVRRoomLayout>(out ovrroomLayout) || !ovrroomLayout.IsEnabled || this.<change>5__4 == SnapshotSceneManager.SnapshotComparer.ChangeType.Changed)
						{
							goto IL_180;
						}
						this.<childAnchors>5__5 = new List<OVRAnchor>();
						awaiter = ovrroomLayout.FetchLayoutAnchorsAsync(this.<childAnchors>5__5).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (this.<>1__state = 0);
							this.<>u__1 = awaiter;
							this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, SnapshotSceneManager.SnapshotComparer.<CheckRoomChanges>d__9>(ref awaiter, ref this);
							return;
						}
						goto IL_E4;
					}
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
				int num = this.<>1__state;
				SnapshotSceneManager.SnapshotComparer snapshotComparer = this.<>4__this;
				List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>> result;
				try
				{
					TaskAwaiter awaiter;
					if (num != 0)
					{
						this.<changes>5__2 = new List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>();
						List<OVRAnchor>.Enumerator enumerator = snapshotComparer.BaseSnapshot.Anchors.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								OVRAnchor ovranchor = enumerator.Current;
								if (!snapshotComparer.NewSnapshot.Anchors.Contains(ovranchor))
								{
									this.<changes>5__2.Add(new ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>(ovranchor, SnapshotSceneManager.SnapshotComparer.ChangeType.Missing));
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
									this.<changes>5__2.Add(new ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>(ovranchor2, SnapshotSceneManager.SnapshotComparer.ChangeType.New));
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
						awaiter = snapshotComparer.CheckRoomChanges(this.<changes>5__2).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (this.<>1__state = 0);
							this.<>u__1 = awaiter;
							this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, SnapshotSceneManager.SnapshotComparer.<Compare>d__8>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = this.<>u__1;
						this.<>u__1 = default(TaskAwaiter);
						num = (this.<>1__state = -1);
					}
					awaiter.GetResult();
					result = this.<changes>5__2;
				}
				catch (Exception exception)
				{
					this.<>1__state = -2;
					this.<changes>5__2 = null;
					this.<>t__builder.SetException(exception);
					return;
				}
				this.<>1__state = -2;
				this.<changes>5__2 = null;
				this.<>t__builder.SetResult(result);
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
			int num = this.<>1__state;
			SnapshotSceneManager.SceneSnapshot result;
			try
			{
				OVRTask<bool>.Awaiter awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_AB;
					}
					this.<snapshot>5__2 = new SnapshotSceneManager.SceneSnapshot();
					this.<rooms>5__3 = new List<OVRAnchor>();
					awaiter = OVRAnchor.FetchAnchorsAsync<OVRRoomLayout>(this.<rooms>5__3, OVRSpace.StorageLocation.Local, 1024, 0.0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (this.<>1__state = 0);
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, SnapshotSceneManager.<LoadSceneSnapshotAsync>d__6>(ref awaiter, ref this);
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
					IL_188:
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
								this.<>t__builder.AwaitOnCompleted<OVRTask<bool>.Awaiter, SnapshotSceneManager.<LoadSceneSnapshotAsync>d__6>(ref awaiter, ref this);
								return;
							}
							goto IL_141;
						}
					}
					goto IL_1B0;
					IL_141:
					awaiter.GetResult();
					this.<snapshot>5__2.Anchors.Add(this.<room>5__5);
					this.<snapshot>5__2.Anchors.AddRange(this.<children>5__6);
					this.<children>5__6 = null;
					this.<room>5__5 = default(OVRAnchor);
					goto IL_188;
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)this.<>7__wrap3).Dispose();
					}
				}
				IL_1B0:
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
			int num = this.<>1__state;
			SnapshotSceneManager snapshotSceneManager = this.<>4__this;
			try
			{
				TaskAwaiter<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>> awaiter;
				TaskAwaiter<SnapshotSceneManager.SceneSnapshot> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = this.<>u__2;
						this.<>u__2 = default(TaskAwaiter<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>>);
						num = (this.<>1__state = -1);
						goto IL_E7;
					}
					awaiter2 = snapshotSceneManager.LoadSceneSnapshotAsync().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (this.<>1__state = 0);
						this.<>u__1 = awaiter2;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<SnapshotSceneManager.SceneSnapshot>, SnapshotSceneManager.<UpdateScene>d__5>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter<SnapshotSceneManager.SceneSnapshot>);
					num = (this.<>1__state = -1);
				}
				SnapshotSceneManager.SceneSnapshot result = awaiter2.GetResult();
				this.<currentSnapshot>5__2 = result;
				awaiter = new SnapshotSceneManager.SnapshotComparer(snapshotSceneManager._snapshot, this.<currentSnapshot>5__2).Compare().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = (this.<>1__state = 1);
					this.<>u__2 = awaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>>>, SnapshotSceneManager.<UpdateScene>d__5>(ref awaiter, ref this);
					return;
				}
				IL_E7:
				List<ValueTuple<OVRAnchor, SnapshotSceneManager.SnapshotComparer.ChangeType>> result2 = awaiter.GetResult();
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
				snapshotSceneManager._snapshot = this.<currentSnapshot>5__2;
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
