using UnityEngine;

public class WingsWearable : MonoBehaviour
{
	[Tooltip("This animator must have a parameter called 'FlapSpeed'")]
	public Animator animator;

	[Tooltip("X axis is move speed, Y axis is flap speed")]
	public AnimationCurve flapSpeedCurve;

	private Transform xform;

	private Vector3 oldPos;

	private readonly int flapSpeedParamID = Animator.StringToHash("FlapSpeed");

	private void Awake()
	{
		xform = animator.transform;
	}

	private void OnEnable()
	{
		oldPos = xform.localPosition;
	}

	private void Update()
	{
		Vector3 position = xform.position;
		float f = (position - oldPos).magnitude / Time.deltaTime;
		float value = flapSpeedCurve.Evaluate(Mathf.Abs(f));
		animator.SetFloat(flapSpeedParamID, value);
		oldPos = position;
	}
}
