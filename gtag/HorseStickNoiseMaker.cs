using UnityEngine;

public class HorseStickNoiseMaker : MonoBehaviour
{
	[Tooltip("Meters the object should traverse between playing a provided audio clip.")]
	public float metersPerClip = 4f;

	[Tooltip("Number of seconds that must elapse before playing another audio clip.")]
	public float minSecBetweenClips = 1.5f;

	public SoundBankPlayer soundBankPlayer;

	[Tooltip("Transform assigned in Gorilla Player Networked Prefab to the Gorilla Player Networked parent to keep track of distance traveled.")]
	public Transform gorillaPlayerXform;

	private Vector3 oldPos;

	private float timeSincePlay;

	private float distElapsed;

	protected void OnEnable()
	{
		if (gorillaPlayerXform == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		oldPos = gorillaPlayerXform.position;
		distElapsed = 0f;
		timeSincePlay = 0f;
	}

	protected void LateUpdate()
	{
		Vector3 position = gorillaPlayerXform.position;
		distElapsed += (position - oldPos).magnitude;
		timeSincePlay += Time.deltaTime;
		oldPos = position;
		if (distElapsed >= metersPerClip && timeSincePlay >= minSecBetweenClips)
		{
			soundBankPlayer.Play();
			distElapsed = 0f;
			timeSincePlay = 0f;
		}
	}
}
