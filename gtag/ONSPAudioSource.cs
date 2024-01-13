using System.Runtime.InteropServices;
using UnityEngine;

public class ONSPAudioSource : MonoBehaviour
{
	private enum Parameters
	{
		P_GAIN,
		P_USEINVSQR,
		P_NEAR,
		P_FAR,
		P_RADIUS,
		P_DISABLE_RFL,
		P_VSPEAKERMODE,
		P_AMBISTAT,
		P_READONLY_GLOBAL_RFL_ENABLED,
		P_READONLY_NUM_VOICES,
		P_SENDLEVEL,
		P_NUM
	}

	public const string strONSPS = "AudioPluginOculusSpatializer";

	[SerializeField]
	private bool enableSpatialization = true;

	[SerializeField]
	private float gain;

	[SerializeField]
	private bool useInvSqr;

	[SerializeField]
	private float near = 0.25f;

	[SerializeField]
	private float far = 250f;

	[SerializeField]
	private float volumetricRadius;

	[SerializeField]
	private float reverbSend;

	[SerializeField]
	private bool enableRfl;

	private static ONSPAudioSource RoomReflectionGizmoAS;

	public bool EnableSpatialization
	{
		get
		{
			return enableSpatialization;
		}
		set
		{
			enableSpatialization = value;
		}
	}

	public float Gain
	{
		get
		{
			return gain;
		}
		set
		{
			gain = Mathf.Clamp(value, 0f, 24f);
		}
	}

	public bool UseInvSqr
	{
		get
		{
			return useInvSqr;
		}
		set
		{
			useInvSqr = value;
		}
	}

	public float Near
	{
		get
		{
			return near;
		}
		set
		{
			near = Mathf.Clamp(value, 0f, 1000000f);
		}
	}

	public float Far
	{
		get
		{
			return far;
		}
		set
		{
			far = Mathf.Clamp(value, 0f, 1000000f);
		}
	}

	public float VolumetricRadius
	{
		get
		{
			return volumetricRadius;
		}
		set
		{
			volumetricRadius = Mathf.Clamp(value, 0f, 1000f);
		}
	}

	public float ReverbSend
	{
		get
		{
			return reverbSend;
		}
		set
		{
			reverbSend = Mathf.Clamp(value, -60f, 20f);
		}
	}

	public bool EnableRfl
	{
		get
		{
			return enableRfl;
		}
		set
		{
			enableRfl = value;
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnBeforeSceneLoadRuntimeMethod()
	{
		OSP_SetGlobalVoiceLimit(ONSPSettings.Instance.voiceLimit);
	}

	[DllImport("AudioPluginOculusSpatializer")]
	private static extern void ONSP_GetGlobalRoomReflectionValues(ref bool reflOn, ref bool reverbOn, ref float width, ref float height, ref float length);

	private void Awake()
	{
		AudioSource source = GetComponent<AudioSource>();
		SetParameters(ref source);
	}

	private void Start()
	{
	}

	private void Update()
	{
		AudioSource source = GetComponent<AudioSource>();
		if (!Application.isPlaying || AudioListener.pause || !source.isPlaying || !source.isActiveAndEnabled)
		{
			source.spatialize = false;
		}
		else
		{
			SetParameters(ref source);
		}
	}

	public void SetParameters(ref AudioSource source)
	{
		source.spatialize = enableSpatialization;
		source.SetSpatializerFloat(0, gain);
		if (useInvSqr)
		{
			source.SetSpatializerFloat(1, 1f);
		}
		else
		{
			source.SetSpatializerFloat(1, 0f);
		}
		source.SetSpatializerFloat(2, near);
		source.SetSpatializerFloat(3, far);
		source.SetSpatializerFloat(4, volumetricRadius);
		if (enableRfl)
		{
			source.SetSpatializerFloat(5, 0f);
		}
		else
		{
			source.SetSpatializerFloat(5, 1f);
		}
		source.SetSpatializerFloat(10, reverbSend);
	}

	private void OnDrawGizmos()
	{
		if (RoomReflectionGizmoAS == null)
		{
			RoomReflectionGizmoAS = this;
		}
		Color color = default(Color);
		color.r = 1f;
		color.g = 0.5f;
		color.b = 0f;
		color.a = 1f;
		Gizmos.color = color;
		Gizmos.DrawWireSphere(base.transform.position, Near);
		color.a = 0.1f;
		Gizmos.color = color;
		Gizmos.DrawSphere(base.transform.position, Near);
		color.r = 1f;
		color.g = 0f;
		color.b = 0f;
		color.a = 1f;
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, Far);
		color.a = 0.1f;
		Gizmos.color = color;
		Gizmos.DrawSphere(base.transform.position, Far);
		color.r = 1f;
		color.g = 0f;
		color.b = 1f;
		color.a = 1f;
		Gizmos.color = color;
		Gizmos.DrawWireSphere(base.transform.position, VolumetricRadius);
		color.a = 0.1f;
		Gizmos.color = color;
		Gizmos.DrawSphere(base.transform.position, VolumetricRadius);
		if (RoomReflectionGizmoAS == this)
		{
			bool reflOn = false;
			bool reverbOn = false;
			float width = 1f;
			float height = 1f;
			float length = 1f;
			ONSP_GetGlobalRoomReflectionValues(ref reflOn, ref reverbOn, ref width, ref height, ref length);
			if (Camera.main != null && reflOn)
			{
				color = (Gizmos.color = ((!reverbOn) ? Color.cyan : Color.white));
				Gizmos.DrawWireCube(Camera.main.transform.position, new Vector3(width, height, length));
				color.a = 0.1f;
				Gizmos.color = color;
				Gizmos.DrawCube(Camera.main.transform.position, new Vector3(width, height, length));
			}
		}
	}

	private void OnDestroy()
	{
		if (RoomReflectionGizmoAS == this)
		{
			RoomReflectionGizmoAS = null;
		}
	}

	[DllImport("AudioPluginOculusSpatializer")]
	private static extern int OSP_SetGlobalVoiceLimit(int VoiceLimit);
}
