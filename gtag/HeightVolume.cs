using UnityEngine;

public class HeightVolume : MonoBehaviour
{
	public Transform heightTop;

	public Transform heightBottom;

	public AudioSource audioSource;

	public float baseVolume;

	public float minVolume;

	public Transform targetTransform;

	public bool invertHeightVol;

	private void Awake()
	{
		if (targetTransform == null)
		{
			targetTransform = Camera.main.transform;
		}
	}

	private void Update()
	{
		if (audioSource.gameObject.activeSelf)
		{
			if (targetTransform.position.y > heightTop.position.y)
			{
				audioSource.volume = ((!invertHeightVol) ? baseVolume : minVolume);
			}
			else if (targetTransform.position.y < heightBottom.position.y)
			{
				audioSource.volume = ((!invertHeightVol) ? minVolume : baseVolume);
			}
			else
			{
				audioSource.volume = ((!invertHeightVol) ? ((targetTransform.position.y - heightBottom.position.y) / (heightTop.position.y - heightBottom.position.y) * (baseVolume - minVolume) + minVolume) : ((heightTop.position.y - targetTransform.position.y) / (heightTop.position.y - heightBottom.position.y) * (baseVolume - minVolume) + minVolume));
			}
		}
	}
}
