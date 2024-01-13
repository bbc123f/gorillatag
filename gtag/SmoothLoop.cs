using System.Collections;
using UnityEngine;

public class SmoothLoop : MonoBehaviour
{
	public AudioSource source;

	public float delay;

	public bool randomStart;

	private void Start()
	{
		if (delay != 0f && !randomStart)
		{
			source.Stop();
			StartCoroutine(DelayedStart());
		}
		else if (randomStart)
		{
			if (source.isActiveAndEnabled)
			{
				source.Play();
			}
			source.time = Random.Range(0f, source.clip.length);
		}
	}

	private void Update()
	{
		if (source.time > source.clip.length * 0.95f)
		{
			source.time = 0.1f;
		}
	}

	private void OnEnable()
	{
		if (randomStart)
		{
			if (source.isActiveAndEnabled)
			{
				source.Play();
			}
			source.time = Random.Range(0f, source.clip.length);
		}
	}

	public IEnumerator DelayedStart()
	{
		yield return new WaitForSeconds(delay);
		source.Play();
	}
}
