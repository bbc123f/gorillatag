using UnityEngine;

public class WaterSplashEffect : MonoBehaviour
{
	private static int lastPlayedBigSplashAudioClipIndex = -1;

	private static int lastPlayedSmallSplashEntryAudioClipIndex = -1;

	private static int lastPlayedSmallSplashExitAudioClipIndex = -1;

	public ParticleSystem[] bigSplashParticleSystems;

	public ParticleSystem[] smallSplashParticleSystems;

	public float lifeTime = 1f;

	private float startTime = -1f;

	public AudioSource audioSource;

	public AudioClip[] bigSplashAudioClips;

	public AudioClip[] smallSplashEntryAudioClips;

	public AudioClip[] smallSplashExitAudioClips;

	private void OnEnable()
	{
		startTime = Time.time;
	}

	public void Destroy()
	{
		DeactivateParticleSystems(bigSplashParticleSystems);
		DeactivateParticleSystems(smallSplashParticleSystems);
		ObjectPools.instance.Destroy(base.gameObject);
	}

	public void PlayEffect(bool isBigSplash, bool isEntry)
	{
		if (isBigSplash)
		{
			DeactivateParticleSystems(smallSplashParticleSystems);
			PlayParticleEffects(bigSplashParticleSystems);
			PlayRandomAudioClipWithoutRepeats(bigSplashAudioClips, ref lastPlayedBigSplashAudioClipIndex);
		}
		else if (isEntry)
		{
			DeactivateParticleSystems(bigSplashParticleSystems);
			PlayParticleEffects(smallSplashParticleSystems);
			PlayRandomAudioClipWithoutRepeats(smallSplashEntryAudioClips, ref lastPlayedSmallSplashEntryAudioClipIndex);
		}
		else
		{
			DeactivateParticleSystems(bigSplashParticleSystems);
			PlayParticleEffects(smallSplashParticleSystems);
			PlayRandomAudioClipWithoutRepeats(smallSplashExitAudioClips, ref lastPlayedSmallSplashExitAudioClipIndex);
		}
	}

	private void Update()
	{
		if ((Time.time - startTime) / lifeTime >= 1f)
		{
			Destroy();
		}
	}

	private void DeactivateParticleSystems(ParticleSystem[] particleSystems)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].gameObject.SetActive(value: false);
			}
		}
	}

	private void PlayParticleEffects(ParticleSystem[] particleSystems)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].gameObject.SetActive(value: true);
				particleSystems[i].Play();
			}
		}
	}

	private void PlayRandomAudioClipWithoutRepeats(AudioClip[] audioClips, ref int lastPlayedAudioClipIndex)
	{
		if (!(audioSource != null) || audioClips == null || audioClips.Length == 0)
		{
			return;
		}
		int num = 0;
		if (audioClips.Length > 1)
		{
			int num2 = Random.Range(0, audioClips.Length);
			if (num2 == lastPlayedAudioClipIndex)
			{
				num2 = ((Random.Range(0f, 1f) > 0.5f) ? ((num2 + 1) % audioClips.Length) : (num2 - 1));
				if (num2 < 0)
				{
					num2 = audioClips.Length - 1;
				}
			}
			num = num2;
		}
		lastPlayedAudioClipIndex = num;
		audioSource.clip = audioClips[num];
		audioSource.Play();
	}
}
