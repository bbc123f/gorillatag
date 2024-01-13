using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

public class OculusSpatializerUnity : MonoBehaviour
{
	public delegate void AudioRaycastCallback(Vector3 origin, Vector3 direction, out Vector3 point, out Vector3 normal, IntPtr data);

	public LayerMask layerMask = -1;

	public bool visualizeRoom = true;

	private bool roomVisualizationInitialized;

	public int raysPerSecond = 256;

	public float roomInterpSpeed = 0.9f;

	public float maxWallDistance = 50f;

	public int rayCacheSize = 512;

	public bool dynamicReflectionsEnabled = true;

	private float particleSize = 0.2f;

	private float particleOffset = 0.1f;

	private GameObject room;

	private Renderer[] wallRenderer = new Renderer[6];

	private float[] dims = new float[3] { 1f, 1f, 1f };

	private float[] coefs = new float[6];

	private const int HIT_COUNT = 2048;

	private Vector3[] points = new Vector3[2048];

	private Vector3[] normals = new Vector3[2048];

	private ParticleSystem sys;

	private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[2048];

	private static LayerMask gLayerMask = -1;

	private const string strOSP = "AudioPluginOculusSpatializer";

	private static Vector3 swapHandedness(Vector3 vec)
	{
		return new Vector3(vec.x, vec.y, 0f - vec.z);
	}

	[MonoPInvokeCallback(typeof(AudioRaycastCallback))]
	private static void AudioRaycast(Vector3 origin, Vector3 direction, out Vector3 point, out Vector3 normal, IntPtr data)
	{
		point = Vector3.zero;
		normal = Vector3.zero;
		if (Physics.Raycast(swapHandedness(origin), swapHandedness(direction), out var hitInfo, 1000f, gLayerMask.value))
		{
			point = swapHandedness(hitInfo.point);
			normal = swapHandedness(hitInfo.normal);
		}
	}

	private void Start()
	{
		OSP_Unity_AssignRaycastCallback(AudioRaycast, IntPtr.Zero);
	}

	private void OnDestroy()
	{
		OSP_Unity_AssignRaycastCallback(IntPtr.Zero, IntPtr.Zero);
	}

	private void Update()
	{
		if (dynamicReflectionsEnabled)
		{
			OSP_Unity_AssignRaycastCallback(AudioRaycast, IntPtr.Zero);
		}
		else
		{
			OSP_Unity_AssignRaycastCallback(IntPtr.Zero, IntPtr.Zero);
		}
		OSP_Unity_SetDynamicRoomRaysPerSecond(raysPerSecond);
		OSP_Unity_SetDynamicRoomInterpSpeed(roomInterpSpeed);
		OSP_Unity_SetDynamicRoomMaxWallDistance(maxWallDistance);
		OSP_Unity_SetDynamicRoomRaysRayCacheSize(rayCacheSize);
		gLayerMask = layerMask;
		OSP_Unity_UpdateRoomModel(1f);
		if (!visualizeRoom)
		{
			return;
		}
		if (!roomVisualizationInitialized)
		{
			inititalizeRoomVisualization();
			roomVisualizationInitialized = true;
		}
		OSP_Unity_GetRoomDimensions(dims, coefs, out var position);
		position.z *= -1f;
		Vector3 vector = new Vector3(dims[0], dims[1], dims[2]);
		float sqrMagnitude = vector.sqrMagnitude;
		if (!float.IsNaN(sqrMagnitude) && 0f < sqrMagnitude && sqrMagnitude < 1000000f)
		{
			base.transform.localScale = vector * 0.999f;
		}
		base.transform.position = position;
		OSP_Unity_GetRaycastHits(points, normals, 2048);
		for (int i = 0; i < 2048; i++)
		{
			if (points[i] == Vector3.zero)
			{
				points[i].y = -10000f;
			}
			points[i].z *= -1f;
			normals[i].z *= -1f;
			particles[i].position = points[i] + normals[i] * particleOffset;
			if (normals[i] != Vector3.zero)
			{
				particles[i].rotation3D = Quaternion.LookRotation(normals[i]).eulerAngles;
			}
			particles[i].startSize = particleSize;
			particles[i].startColor = new Color(0.8156863f, 0.14901961f, 58f / 85f, 1f);
		}
		for (int j = 0; j < 6; j++)
		{
			Color value = Color.Lerp(Color.red, Color.green, coefs[j]);
			wallRenderer[j].material.SetColor("_TintColor", value);
		}
		sys.SetParticles(particles, particles.Length);
	}

	private void inititalizeRoomVisualization()
	{
		Debug.Log("Oculus Audio dynamic room estimation visualization enabled");
		base.transform.position = Vector3.zero;
		GameObject gameObject = new GameObject("DecalManager");
		gameObject.transform.parent = base.transform;
		sys = gameObject.AddComponent<ParticleSystem>();
		ParticleSystem.MainModule main = sys.main;
		main.simulationSpace = ParticleSystemSimulationSpace.World;
		main.loop = false;
		main.playOnAwake = false;
		ParticleSystem.EmissionModule emission = sys.emission;
		emission.enabled = false;
		ParticleSystem.ShapeModule shape = sys.shape;
		shape.enabled = false;
		ParticleSystemRenderer component = sys.GetComponent<ParticleSystemRenderer>();
		component.renderMode = ParticleSystemRenderMode.Mesh;
		component.material.shader = Shader.Find("Particles/Additive");
		Texture2D texture2D = new Texture2D(64, 64);
		for (int i = 0; i < 32; i++)
		{
			for (int j = 0; j < 32; j++)
			{
				float num = 32 - i;
				float num2 = 32 - j;
				float num3 = Mathf.Sqrt(num * num + num2 * num2);
				float num4 = 2f * num3 / 32f;
				float a = ((num3 < 32f) ? Mathf.Clamp01(Mathf.Sin((float)Math.PI * 2f * num4)) : 0f);
				Color color = new Color(1f, 1f, 1f, a);
				texture2D.SetPixel(i, j, color);
				texture2D.SetPixel(64 - i, j, color);
				texture2D.SetPixel(i, 64 - j, color);
				texture2D.SetPixel(64 - i, 64 - j, color);
			}
		}
		texture2D.Apply();
		component.material.mainTexture = texture2D;
		Mesh mesh = new Mesh();
		mesh.name = "ParticleQuad";
		mesh.vertices = new Vector3[4]
		{
			new Vector3(-0.5f, -0.5f, 0f),
			new Vector3(0.5f, -0.5f, 0f),
			new Vector3(0.5f, 0.5f, 0f),
			new Vector3(-0.5f, 0.5f, 0f)
		};
		mesh.uv = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f)
		};
		mesh.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
		mesh.RecalculateNormals();
		component.mesh = mesh;
		sys.Emit(2048);
		room = new GameObject("RoomVisualizer");
		room.transform.parent = base.transform;
		room.transform.localPosition = Vector3.zero;
		Texture2D texture2D2 = new Texture2D(32, 32);
		Color color2 = new Color(0f, 0f, 0f, 0f);
		for (int k = 0; k < 32; k++)
		{
			for (int l = 0; l < 32; l++)
			{
				texture2D2.SetPixel(k, l, color2);
			}
		}
		for (int m = 0; m < 32; m++)
		{
			Color color3 = Color.white * 0.125f;
			texture2D2.SetPixel(8, m, color3);
			texture2D2.SetPixel(m, 8, color3);
			texture2D2.SetPixel(24, m, color3);
			texture2D2.SetPixel(m, 24, color3);
			color3 *= 2f;
			texture2D2.SetPixel(16, m, color3);
			texture2D2.SetPixel(m, 16, color3);
			color3 *= 2f;
			texture2D2.SetPixel(0, m, color3);
			texture2D2.SetPixel(m, 0, color3);
		}
		texture2D2.Apply();
		for (int n = 0; n < 6; n++)
		{
			Mesh mesh2 = new Mesh();
			mesh2.name = "Plane" + n;
			Vector3[] array = new Vector3[4];
			int num5 = n / 2;
			int num6 = ((n % 2 == 0) ? 1 : (-1));
			for (int num7 = 0; num7 < 4; num7++)
			{
				array[num7][num5] = (float)num6 * 0.5f;
				array[num7][(num5 + 1) % 3] = 0.5f * (float)((num7 == 1 || num7 == 2) ? 1 : (-1));
				array[num7][(num5 + 2) % 3] = 0.5f * (float)((num7 == 2 || num7 == 3) ? 1 : (-1));
			}
			mesh2.vertices = array;
			mesh2.uv = new Vector2[4]
			{
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f)
			};
			mesh2.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
			mesh2.RecalculateNormals();
			GameObject obj = new GameObject("Wall_" + n);
			obj.AddComponent<MeshFilter>().mesh = mesh2;
			MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
			wallRenderer[n] = meshRenderer;
			meshRenderer.material.shader = Shader.Find("Particles/Additive");
			meshRenderer.material.mainTexture = texture2D2;
			meshRenderer.material.mainTextureScale = new Vector2(8f, 8f);
			obj.transform.parent = room.transform;
			room.transform.localPosition = Vector3.zero;
		}
	}

	[DllImport("AudioPluginOculusSpatializer")]
	private static extern int OSP_Unity_AssignRaycastCallback(AudioRaycastCallback callback, IntPtr data);

	[DllImport("AudioPluginOculusSpatializer")]
	private static extern int OSP_Unity_AssignRaycastCallback(IntPtr callback, IntPtr data);

	[DllImport("AudioPluginOculusSpatializer")]
	private static extern int OSP_Unity_SetDynamicRoomRaysPerSecond(int RaysPerSecond);

	[DllImport("AudioPluginOculusSpatializer")]
	private static extern int OSP_Unity_SetDynamicRoomInterpSpeed(float InterpSpeed);

	[DllImport("AudioPluginOculusSpatializer")]
	private static extern int OSP_Unity_SetDynamicRoomMaxWallDistance(float MaxWallDistance);

	[DllImport("AudioPluginOculusSpatializer")]
	private static extern int OSP_Unity_SetDynamicRoomRaysRayCacheSize(int RayCacheSize);

	[DllImport("AudioPluginOculusSpatializer")]
	private static extern int OSP_Unity_UpdateRoomModel(float wetLevel);

	[DllImport("AudioPluginOculusSpatializer")]
	private static extern int OSP_Unity_GetRoomDimensions(float[] roomDimensions, float[] reflectionsCoefs, out Vector3 position);

	[DllImport("AudioPluginOculusSpatializer")]
	private static extern int OSP_Unity_GetRaycastHits(Vector3[] points, Vector3[] normals, int length);
}
