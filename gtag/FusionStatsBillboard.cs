using System;
using Fusion;
using UnityEngine;

[ScriptHelp(BackColor = EditorHeaderBackColor.Olive)]
[ExecuteAlways]
public class FusionStatsBillboard : Fusion.Behaviour
{
	private void Awake()
	{
		this._fusionStats = base.GetComponent<FusionStats>();
	}

	private void OnEnable()
	{
		this.UpdateLookAt();
	}

	private void OnDisable()
	{
		base.transform.localRotation = default(Quaternion);
	}

	private Camera MainCamera
	{
		get
		{
			float time = Time.time;
			if (time == FusionStatsBillboard._lastCameraFindTime)
			{
				return FusionStatsBillboard._currentCam;
			}
			FusionStatsBillboard._lastCameraFindTime = time;
			return FusionStatsBillboard._currentCam = Camera.main;
		}
		set
		{
			FusionStatsBillboard._currentCam = value;
		}
	}

	private void LateUpdate()
	{
		this.UpdateLookAt();
	}

	public void UpdateLookAt()
	{
		if (this._fusionStats && this._fusionStats.CanvasType == FusionStats.StatCanvasTypes.Overlay)
		{
			return;
		}
		Camera camera = (this.Camera ? this.Camera : this.MainCamera);
		if (camera && base.enabled)
		{
			base.transform.rotation = camera.transform.rotation;
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void ResetStatics()
	{
		FusionStatsBillboard._currentCam = null;
		FusionStatsBillboard._lastCameraFindTime = 0f;
	}

	public FusionStatsBillboard()
	{
	}

	[InlineHelp]
	public Camera Camera;

	private static float _lastCameraFindTime;

	private static Camera _currentCam;

	private FusionStats _fusionStats;
}
