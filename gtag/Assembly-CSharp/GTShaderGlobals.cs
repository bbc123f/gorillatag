using System;
using UnityEngine;

[ExecuteInEditMode]
public class GTShaderGlobals : MonoBehaviour
{
	private void OnEnable()
	{
		this._mainCamera = Camera.main.transform;
	}

	private void Update()
	{
		GTShaderGlobals._cameraPosGame = this._mainCamera.position;
		Shader.SetGlobalVector(GTShaderGlobals.PropertyIDs._WorldSpaceCameraPos_GT, GTShaderGlobals._cameraPosGame);
	}

	public GTShaderGlobals()
	{
	}

	[SerializeField]
	private Transform _mainCamera;

	[DebugReadOnly]
	private static Vector3 _cameraPosGame;

	public static class PropertyNames
	{
		public const string _WorldSpaceCameraPos_GT = "_WorldSpaceCameraPos_GT";
	}

	public static class PropertyIDs
	{
		// Note: this type is marked as 'beforefieldinit'.
		static PropertyIDs()
		{
		}

		public static readonly int _WorldSpaceCameraPos_GT = Shader.PropertyToID("_WorldSpaceCameraPos_GT");
	}
}
