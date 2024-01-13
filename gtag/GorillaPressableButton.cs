using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GorillaPressableButton : MonoBehaviour
{
	public Material pressedMaterial;

	public Material unpressedMaterial;

	public MeshRenderer buttonRenderer;

	public bool isOn;

	public float debounceTime = 0.25f;

	public float touchTime;

	public bool testPress;

	public bool testHandLeft;

	[TextArea]
	public string offText;

	[TextArea]
	public string onText;

	public Text myText;

	[Space]
	public UnityEvent onPressButton;

	public virtual void Start()
	{
	}

	private void OnEnable()
	{
		if (Application.isEditor)
		{
			StartCoroutine(TestPressCheck());
		}
	}

	private void OnDisable()
	{
		if (Application.isEditor)
		{
			StopAllCoroutines();
		}
	}

	private IEnumerator TestPressCheck()
	{
		while (true)
		{
			if (testPress)
			{
				testPress = false;
				ButtonActivation();
				ButtonActivationWithHand(testHandLeft);
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled || !(touchTime + debounceTime < Time.time) || collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		touchTime = Time.time;
		GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
		onPressButton.Invoke();
		ButtonActivation();
		ButtonActivationWithHand(component.isLeftHand);
		if (!(component == null))
		{
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, component.isLeftHand, 0.05f);
			GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
			{
				GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.Others, 67, component.isLeftHand, 0.05f);
			}
		}
	}

	public virtual void UpdateColor()
	{
		if (isOn)
		{
			buttonRenderer.material = pressedMaterial;
			if (myText != null)
			{
				myText.text = onText;
			}
		}
		else
		{
			buttonRenderer.material = unpressedMaterial;
			if (myText != null)
			{
				myText.text = offText;
			}
		}
	}

	public virtual void ButtonActivation()
	{
	}

	public virtual void ButtonActivationWithHand(bool isLeftHand)
	{
	}
}
