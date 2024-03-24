using System;
using BuildSafe;
using UnityEngine.SceneManagement;

public class NMeshTarget : SceneBakeTask
{
	public override int callbackOrder
	{
		get
		{
			return 1;
		}
		set
		{
		}
	}

	public override void OnSceneBake(Scene scene, SceneBakeMode mode)
	{
		NMesh.Add(base.gameObject);
	}

	public NMeshTarget()
	{
	}
}
