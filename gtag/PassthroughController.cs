using System;
using UnityEngine;

public class PassthroughController : MonoBehaviour
{
	private void Start()
	{
		GameObject gameObject = GameObject.Find("OVRCameraRig");
		if (gameObject == null)
		{
			Debug.LogError("Scene does not contain an OVRCameraRig");
			return;
		}
		this.passthroughLayer = gameObject.GetComponent<OVRPassthroughLayer>();
		if (this.passthroughLayer == null)
		{
			Debug.LogError("OVRCameraRig does not contain an OVRPassthroughLayer component");
		}
	}

	private void Update()
	{
		Color color = Color.HSVToRGB(Time.time * 0.1f % 1f, 1f, 1f);
		this.passthroughLayer.edgeColor = color;
		float num = Mathf.Sin(Time.time);
		this.passthroughLayer.SetColorMapControls(num, 0f, 0f, null, OVRPassthroughLayer.ColorMapEditorType.GrayscaleToColor);
		base.transform.position = Camera.main.transform.position;
		base.transform.rotation = Quaternion.LookRotation(new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized);
	}

	private OVRPassthroughLayer passthroughLayer;
}
