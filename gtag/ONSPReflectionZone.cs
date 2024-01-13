using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ONSPReflectionZone : MonoBehaviour
{
	public AudioMixerSnapshot mixerSnapshot;

	public float fadeTime;

	private static Stack<ReflectionSnapshot> snapshotList = new Stack<ReflectionSnapshot>();

	private static ReflectionSnapshot currentSnapshot = default(ReflectionSnapshot);

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (CheckForAudioListener(other.gameObject))
		{
			PushCurrentMixerShapshot();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (CheckForAudioListener(other.gameObject))
		{
			PopCurrentMixerSnapshot();
		}
	}

	private bool CheckForAudioListener(GameObject gameObject)
	{
		if (gameObject.GetComponentInChildren<AudioListener>() != null)
		{
			return true;
		}
		return false;
	}

	private void PushCurrentMixerShapshot()
	{
		ReflectionSnapshot item = currentSnapshot;
		snapshotList.Push(item);
		SetReflectionValues();
	}

	private void PopCurrentMixerSnapshot()
	{
		ReflectionSnapshot mss = snapshotList.Pop();
		SetReflectionValues(ref mss);
	}

	private void SetReflectionValues()
	{
		if (mixerSnapshot != null)
		{
			Debug.Log("Setting off snapshot " + mixerSnapshot.name);
			mixerSnapshot.TransitionTo(fadeTime);
			currentSnapshot.mixerSnapshot = mixerSnapshot;
			currentSnapshot.fadeTime = fadeTime;
		}
		else
		{
			Debug.Log("Mixer snapshot not set - Please ensure play area has at least one encompassing snapshot.");
		}
	}

	private void SetReflectionValues(ref ReflectionSnapshot mss)
	{
		if (mss.mixerSnapshot != null)
		{
			Debug.Log("Setting off snapshot " + mss.mixerSnapshot.name);
			mss.mixerSnapshot.TransitionTo(mss.fadeTime);
			currentSnapshot.mixerSnapshot = mss.mixerSnapshot;
			currentSnapshot.fadeTime = mss.fadeTime;
		}
		else
		{
			Debug.Log("Mixer snapshot not set - Please ensure play area has at least one encompassing snapshot.");
		}
	}
}
