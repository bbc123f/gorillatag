using System.Text;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.UI;

public class DebugHudStats : MonoBehaviour
{
	[SerializeField]
	private Text text;

	[SerializeField]
	private float delayUpdateRate = 0.25f;

	private float updateTimer;

	private bool lastCheckInvalid;

	private StringBuilder builder;

	private void Awake()
	{
		base.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (updateTimer < delayUpdateRate)
		{
			updateTimer += Time.deltaTime;
			return;
		}
		builder.Clear();
		int a = Mathf.RoundToInt(1f / Time.smoothDeltaTime);
		a = Mathf.Min(a, 90);
		if (a < 89)
		{
			if (lastCheckInvalid)
			{
				text.color = Color.red;
			}
			lastCheckInvalid = true;
		}
		else
		{
			lastCheckInvalid = false;
			text.color = Color.white;
		}
		builder.Append(a);
		builder.AppendLine(" fps");
		float magnitude = Player.Instance.currentVelocity.magnitude;
		builder.Append(Mathf.RoundToInt(magnitude));
		builder.Append(" m/s");
		text.text = builder.ToString();
		updateTimer = 0f;
	}
}
