using System;
using System.Collections;
using UnityEngine;

public class SmoothLoop : MonoBehaviour
{
	private void Start()
	{
		if (this.delay != 0f && !this.randomStart)
		{
			this.source.Stop();
			base.StartCoroutine(this.DelayedStart());
			return;
		}
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.Play();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	private void Update()
	{
		if (this.source.time > this.source.clip.length * 0.95f)
		{
			this.source.time = 0.1f;
		}
	}

	private void OnEnable()
	{
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.Play();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	public IEnumerator DelayedStart()
	{
		yield return new WaitForSeconds(this.delay);
		this.source.Play();
		yield break;
	}

	public AudioSource source;

	public float delay;

	public bool randomStart;
}
