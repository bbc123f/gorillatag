using System;
using System.Text;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

public class DebugHudStats : MonoBehaviour
{
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (this.firstAwake == 0f)
		{
			this.firstAwake = Time.time;
		}
		if (this.updateTimer < this.delayUpdateRate)
		{
			this.updateTimer += Time.deltaTime;
			return;
		}
		this.builder.Clear();
		int num = Mathf.RoundToInt(1f / Time.smoothDeltaTime);
		num = Mathf.Min(num, 90);
		if (num < 89)
		{
			if (this.lastCheckInvalid)
			{
				this.text.color = Color.red;
			}
			this.lastCheckInvalid = true;
		}
		else
		{
			this.lastCheckInvalid = false;
			this.text.color = Color.white;
		}
		this.builder.Append(num);
		this.builder.AppendLine(" fps");
		float magnitude = Player.Instance.currentVelocity.magnitude;
		this.builder.Append(Mathf.RoundToInt(magnitude));
		this.builder.AppendLine(" m/s");
		if (GorillaComputer.instance != null)
		{
			this.builder.AppendLine(GorillaComputer.instance.GetServerTime().ToString());
		}
		else
		{
			this.builder.AppendLine("Server Time Unavailable");
		}
		this.text.text = this.builder.ToString();
		this.updateTimer = 0f;
	}

	[SerializeField]
	private Text text;

	[SerializeField]
	private float delayUpdateRate = 0.25f;

	private float updateTimer;

	private bool lastCheckInvalid;

	public float sessionAnytrackingLost;

	public float last30SecondsTrackingLost;

	private float firstAwake;

	private bool wasTrackingLost;

	private bool leftHandTracked;

	private bool rightHandTracked;

	private StringBuilder builder;
}
