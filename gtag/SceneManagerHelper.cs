using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering;

public class SceneManagerHelper
{
	public GameObject AnchorGameObject
	{
		[CompilerGenerated]
		get
		{
			return this.<AnchorGameObject>k__BackingField;
		}
	}

	public SceneManagerHelper(GameObject gameObject)
	{
		this.AnchorGameObject = gameObject;
	}

	public void SetLocation(OVRLocatable locatable, Camera camera = null)
	{
		OVRLocatable.TrackingSpacePose trackingSpacePose;
		if (!locatable.TryGetSceneAnchorPose(out trackingSpacePose))
		{
			return;
		}
		Camera camera2 = ((camera == null) ? Camera.main : camera);
		Vector3? vector = trackingSpacePose.ComputeWorldPosition(camera2);
		Quaternion? quaternion = trackingSpacePose.ComputeWorldRotation(camera2);
		if (vector != null && quaternion != null)
		{
			this.AnchorGameObject.transform.SetPositionAndRotation(vector.Value, quaternion.Value);
		}
	}

	public void CreatePlane(OVRBounded2D bounds)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject.name = "Plane";
		gameObject.transform.SetParent(this.AnchorGameObject.transform, false);
		gameObject.transform.localScale = new Vector3(bounds.BoundingBox.size.x, bounds.BoundingBox.size.y, 0.01f);
		gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
	}

	public void UpdatePlane(OVRBounded2D bounds)
	{
		Transform transform = this.AnchorGameObject.transform.Find("Plane");
		if (transform == null)
		{
			this.CreatePlane(bounds);
			return;
		}
		transform.transform.localScale = new Vector3(bounds.BoundingBox.size.x, bounds.BoundingBox.size.y, 0.01f);
	}

	public void CreateVolume(OVRBounded3D bounds)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject.name = "Volume";
		gameObject.transform.SetParent(this.AnchorGameObject.transform, false);
		gameObject.transform.localPosition = new Vector3(0f, 0f, -bounds.BoundingBox.size.z / 2f);
		gameObject.transform.localScale = bounds.BoundingBox.size;
		gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
	}

	public void UpdateVolume(OVRBounded3D bounds)
	{
		Transform transform = this.AnchorGameObject.transform.Find("Volume");
		if (transform == null)
		{
			this.CreateVolume(bounds);
			return;
		}
		transform.transform.localPosition = new Vector3(0f, 0f, -bounds.BoundingBox.size.z / 2f);
		transform.transform.localScale = bounds.BoundingBox.size;
	}

	public void CreateMesh(OVRTriangleMesh mesh)
	{
		int num;
		int num2;
		if (!mesh.TryGetCounts(out num, out num2))
		{
			return;
		}
		using (NativeArray<Vector3> nativeArray = new NativeArray<Vector3>(num, Allocator.Temp, NativeArrayOptions.ClearMemory))
		{
			using (NativeArray<int> nativeArray2 = new NativeArray<int>(num2 * 3, Allocator.Temp, NativeArrayOptions.ClearMemory))
			{
				if (mesh.TryGetMesh(nativeArray, nativeArray2))
				{
					Mesh mesh2 = new Mesh();
					mesh2.indexFormat = IndexFormat.UInt32;
					mesh2.SetVertices<Vector3>(nativeArray);
					mesh2.SetTriangles(nativeArray2.ToArray(), 0);
					GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
					gameObject.name = "Mesh";
					gameObject.transform.SetParent(this.AnchorGameObject.transform, false);
					gameObject.GetComponent<MeshFilter>().sharedMesh = mesh2;
					gameObject.GetComponent<MeshCollider>().sharedMesh = mesh2;
					gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
				}
			}
		}
	}

	public void UpdateMesh(OVRTriangleMesh mesh)
	{
		Transform transform = this.AnchorGameObject.transform.Find("Mesh");
		if (transform != null)
		{
			Object.Destroy(transform);
		}
		this.CreateMesh(mesh);
	}

	public static async Task<bool> RequestSceneCapture()
	{
		bool flag;
		if (SceneManagerHelper.SceneCaptureRunning)
		{
			flag = false;
		}
		else
		{
			SceneManagerHelper.SceneCaptureRunning = true;
			bool waiting = true;
			Action<ulong, bool> onCaptured = delegate(ulong id, bool success)
			{
				waiting = false;
			};
			flag = await Task.Run<bool>(delegate
			{
				OVRManager.SceneCaptureComplete += onCaptured;
				ulong num;
				if (!OVRPlugin.RequestSceneCapture("", out num))
				{
					OVRManager.SceneCaptureComplete -= onCaptured;
					SceneManagerHelper.SceneCaptureRunning = false;
					return false;
				}
				while (waiting)
				{
					Task.Delay(200);
				}
				OVRManager.SceneCaptureComplete -= onCaptured;
				SceneManagerHelper.SceneCaptureRunning = false;
				return true;
			});
		}
		return flag;
	}

	public static void RequestScenePermission()
	{
		if (!Permission.HasUserAuthorizedPermission("com.oculus.permission.USE_SCENE"))
		{
			Permission.RequestUserPermission("com.oculus.permission.USE_SCENE");
		}
	}

	[CompilerGenerated]
	private readonly GameObject <AnchorGameObject>k__BackingField;

	private static bool SceneCaptureRunning;

	[CompilerGenerated]
	private sealed class <>c__DisplayClass11_0
	{
		public <>c__DisplayClass11_0()
		{
		}

		internal void <RequestSceneCapture>b__0(ulong id, bool success)
		{
			this.waiting = false;
		}

		internal bool <RequestSceneCapture>b__1()
		{
			OVRManager.SceneCaptureComplete += this.onCaptured;
			ulong num;
			if (!OVRPlugin.RequestSceneCapture("", out num))
			{
				OVRManager.SceneCaptureComplete -= this.onCaptured;
				SceneManagerHelper.SceneCaptureRunning = false;
				return false;
			}
			while (this.waiting)
			{
				Task.Delay(200);
			}
			OVRManager.SceneCaptureComplete -= this.onCaptured;
			SceneManagerHelper.SceneCaptureRunning = false;
			return true;
		}

		public bool waiting;

		public Action<ulong, bool> onCaptured;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <RequestSceneCapture>d__11 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			bool flag;
			try
			{
				TaskAwaiter<bool> taskAwaiter;
				if (num != 0)
				{
					SceneManagerHelper.<>c__DisplayClass11_0 CS$<>8__locals1 = new SceneManagerHelper.<>c__DisplayClass11_0();
					if (SceneManagerHelper.SceneCaptureRunning)
					{
						flag = false;
						goto IL_BC;
					}
					SceneManagerHelper.SceneCaptureRunning = true;
					CS$<>8__locals1.waiting = true;
					CS$<>8__locals1.onCaptured = delegate(ulong id, bool success)
					{
						CS$<>8__locals1.waiting = false;
					};
					taskAwaiter = Task.Run<bool>(delegate
					{
						OVRManager.SceneCaptureComplete += CS$<>8__locals1.onCaptured;
						ulong num3;
						if (!OVRPlugin.RequestSceneCapture("", out num3))
						{
							OVRManager.SceneCaptureComplete -= CS$<>8__locals1.onCaptured;
							SceneManagerHelper.SceneCaptureRunning = false;
							return false;
						}
						while (CS$<>8__locals1.waiting)
						{
							Task.Delay(200);
						}
						OVRManager.SceneCaptureComplete -= CS$<>8__locals1.onCaptured;
						SceneManagerHelper.SceneCaptureRunning = false;
						return true;
					}).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						num2 = 0;
						TaskAwaiter<bool> taskAwaiter2 = taskAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, SceneManagerHelper.<RequestSceneCapture>d__11>(ref taskAwaiter, ref this);
						return;
					}
				}
				else
				{
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
					num2 = -1;
				}
				flag = taskAwaiter.GetResult();
			}
			catch (Exception ex)
			{
				num2 = -2;
				this.<>t__builder.SetException(ex);
				return;
			}
			IL_BC:
			num2 = -2;
			this.<>t__builder.SetResult(flag);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<bool> <>t__builder;

		private TaskAwaiter<bool> <>u__1;
	}
}
