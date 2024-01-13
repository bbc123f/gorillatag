using UnityEngine;

namespace OVR;

public class AmbienceEmitter : MonoBehaviour
{
	public SoundFXRef[] ambientSounds = new SoundFXRef[0];

	public bool autoActivate = true;

	[Tooltip("Automatically play the sound randomly again when checked.  Should be OFF for looping sounds")]
	public bool autoRetrigger = true;

	[MinMax(2f, 4f, 0.1f, 10f)]
	public Vector2 randomRetriggerDelaySecs = new Vector2(2f, 4f);

	[Tooltip("If defined, the sounds will randomly play from these transform positions, otherwise the sound will play from this transform")]
	public Transform[] playPositions = new Transform[0];

	private bool activated;

	private int playingIdx = -1;

	private float nextPlayTime;

	private float fadeTime = 0.25f;

	private int lastPosIdx = -1;

	private void Awake()
	{
		if (autoActivate)
		{
			activated = true;
			nextPlayTime = Time.time + Random.Range(randomRetriggerDelaySecs.x, randomRetriggerDelaySecs.y);
		}
		Transform[] array = playPositions;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == null)
			{
				Debug.LogWarning("[AmbienceEmitter] Invalid play positions in " + base.name);
				playPositions = new Transform[0];
				break;
			}
		}
	}

	private void Update()
	{
		if (activated && (playingIdx == -1 || autoRetrigger) && Time.time >= nextPlayTime)
		{
			Play();
			if (!autoRetrigger)
			{
				activated = false;
			}
		}
	}

	public void OnTriggerEnter(Collider col)
	{
		activated = !activated;
	}

	public void Play()
	{
		Transform transform = base.transform;
		if (playPositions.Length != 0)
		{
			int num = Random.Range(0, playPositions.Length);
			while (playPositions.Length > 1 && num == lastPosIdx)
			{
				num = Random.Range(0, playPositions.Length);
			}
			transform = playPositions[num];
			lastPosIdx = num;
		}
		playingIdx = ambientSounds[Random.Range(0, ambientSounds.Length)].PlaySoundAt(transform.position);
		if (playingIdx != -1)
		{
			AudioManager.FadeInSound(playingIdx, fadeTime);
			nextPlayTime = Time.time + Random.Range(randomRetriggerDelaySecs.x, randomRetriggerDelaySecs.y);
		}
	}

	public void EnableEmitter(bool enable)
	{
		activated = enable;
		if (enable)
		{
			Play();
		}
		else if (playingIdx != -1)
		{
			AudioManager.FadeOutSound(playingIdx, fadeTime);
		}
	}
}
