using UnityEngine;
using UnityEngine.Audio;

public class MicInput : MonoBehaviour
{
	[Tooltip("Assign to 'MicInput.mixer'.")]
	public AudioMixerGroup mixerGroup;

	private const int kSampleCount = 256;

	private const int kFrequency = 44100;

	private const float kRemapFromMin = 0f;

	private const float kRemapFromMax = 0.001f;

	private static MicInput instance;

	private static AudioSource audioSource;

	private bool initialized;

	public static float Loudness { get; private set; }

	public static string DeviceName { get; private set; }

	protected void Awake()
	{
		if (instance != null && instance != this)
		{
			Debug.LogError("MicInput2: There can only be one instance of this class.");
			Object.Destroy(this);
			return;
		}
		instance = this;
		audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.outputAudioMixerGroup = mixerGroup;
		audioSource.loop = true;
		audioSource.volume = 1f;
	}

	protected void OnEnable()
	{
		StartRecording();
	}

	protected void OnDisable()
	{
		StopRecording();
	}

	protected void OnApplicationFocus(bool focus)
	{
		if (!focus)
		{
			StopRecording();
		}
		else if (!initialized)
		{
			StartRecording();
		}
	}

	protected void LateUpdate()
	{
		if (Microphone.IsRecording(DeviceName))
		{
			float[] array = new float[256];
			audioSource.GetOutputData(array, 0);
			float num = 0f;
			for (int i = 0; i <= array.Length - 1; i++)
			{
				num = Mathf.Abs(num + array[i]);
			}
			float value = num / 256f;
			Loudness = Mathf.InverseLerp(0f, 0.001f, value);
		}
	}

	private void StartRecording()
	{
		if (Microphone.devices.Length == 0)
		{
			DeviceName = "No microphone found.";
			return;
		}
		DeviceName = Microphone.devices[0];
		audioSource.clip = Microphone.Start(DeviceName, loop: true, 1, 44100);
		audioSource.Play();
		initialized = true;
	}

	private void StopRecording()
	{
		initialized = false;
		audioSource.Stop();
		Microphone.End(DeviceName);
		Loudness = 0f;
	}
}
