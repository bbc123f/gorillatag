using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class WaterRippleEffect : MonoBehaviour
{
	[SerializeField]
	private float ripplePlaybackSpeed = 1f;

	[SerializeField]
	private float fadeOutDelay = 0.5f;

	[SerializeField]
	private float fadeOutTime = 1f;

	private string ripplePlaybackSpeedName = "RipplePlaybackSpeed";

	private int ripplePlaybackSpeedHash;

	private float rippleStartTime = -1f;

	private Animator animator;

	private SpriteRenderer renderer;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		renderer = GetComponent<SpriteRenderer>();
		ripplePlaybackSpeedHash = Animator.StringToHash(ripplePlaybackSpeedName);
	}

	private void OnEnable()
	{
		Play();
	}

	public void Destroy()
	{
		ObjectPools.instance.Destroy(base.gameObject);
	}

	public void Play()
	{
		rippleStartTime = Time.time;
		animator.SetFloat(ripplePlaybackSpeedHash, ripplePlaybackSpeed);
		Color color = renderer.color;
		color.a = 1f;
		renderer.color = color;
	}

	private void Update()
	{
		float num = Mathf.Clamp01((Time.time - rippleStartTime - fadeOutDelay) / fadeOutTime);
		Color color = renderer.color;
		color.a = 1f - num;
		renderer.color = color;
		if (num >= 1f - Mathf.Epsilon)
		{
			Destroy();
		}
	}
}
