using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000BE RID: 190
[RequireComponent(typeof(OVRSceneAnchor))]
[DefaultExecutionOrder(30)]
public class FurnitureSpawner : MonoBehaviour
{
	// Token: 0x0600042A RID: 1066 RVA: 0x0001B49B File Offset: 0x0001969B
	private void Start()
	{
		this._sceneAnchor = base.GetComponent<OVRSceneAnchor>();
		this._classification = base.GetComponent<OVRSemanticClassification>();
		this.AddRoomLight();
		this.SpawnSpawnable();
	}

	// Token: 0x0600042B RID: 1067 RVA: 0x0001B4C4 File Offset: 0x000196C4
	private void SpawnSpawnable()
	{
		Spawnable spawnable;
		if (!this.FindValidSpawnable(out spawnable))
		{
			return;
		}
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		Vector3 localScale = base.transform.localScale;
		OVRScenePlane component = this._sceneAnchor.GetComponent<OVRScenePlane>();
		OVRSceneVolume component2 = this._sceneAnchor.GetComponent<OVRSceneVolume>();
		Vector3 newSize = component2 ? component2.Dimensions : Vector3.one;
		if (this._classification && component)
		{
			newSize = component.Dimensions;
			newSize.z = 1f;
			if (this._classification.Contains("DESK") || this._classification.Contains("COUCH"))
			{
				this.GetVolumeFromTopPlane(base.transform, component.Dimensions, base.transform.position.y, out position, out rotation, out localScale);
				newSize = localScale;
				position.y += localScale.y / 2f;
			}
			if (this._classification.Contains("WALL_FACE") || this._classification.Contains("CEILING") || this._classification.Contains("FLOOR"))
			{
				newSize.z = 0.01f;
			}
		}
		GameObject gameObject = new GameObject("Root");
		gameObject.transform.parent = base.transform;
		gameObject.transform.SetPositionAndRotation(position, rotation);
		new SimpleResizer().CreateResizedObject(newSize, gameObject, spawnable.ResizablePrefab);
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x0001B654 File Offset: 0x00019854
	private bool FindValidSpawnable(out Spawnable currentSpawnable)
	{
		currentSpawnable = null;
		if (!this._classification)
		{
			return false;
		}
		if (!Object.FindObjectOfType<OVRSceneManager>())
		{
			return false;
		}
		foreach (Spawnable spawnable in this.SpawnablePrefabs)
		{
			if (this._classification.Contains(spawnable.ClassificationLabel))
			{
				currentSpawnable = spawnable;
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x0001B6E0 File Offset: 0x000198E0
	private void AddRoomLight()
	{
		if (!this.RoomLightPrefab)
		{
			return;
		}
		if (this._classification && this._classification.Contains("CEILING") && !FurnitureSpawner._roomLightRef)
		{
			FurnitureSpawner._roomLightRef = Object.Instantiate<GameObject>(this.RoomLightPrefab, this._sceneAnchor.transform);
		}
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x0001B744 File Offset: 0x00019944
	private void GetVolumeFromTopPlane(Transform plane, Vector2 dimensions, float height, out Vector3 position, out Quaternion rotation, out Vector3 localScale)
	{
		float num = height / 2f;
		position = plane.position - Vector3.up * num;
		rotation = Quaternion.LookRotation(-plane.up, Vector3.up);
		localScale = new Vector3(dimensions.x, num * 2f, dimensions.y);
	}

	// Token: 0x040004DE RID: 1246
	[Tooltip("Add a point at ceiling.")]
	public GameObject RoomLightPrefab;

	// Token: 0x040004DF RID: 1247
	public List<Spawnable> SpawnablePrefabs;

	// Token: 0x040004E0 RID: 1248
	private OVRSceneAnchor _sceneAnchor;

	// Token: 0x040004E1 RID: 1249
	private OVRSemanticClassification _classification;

	// Token: 0x040004E2 RID: 1250
	private static GameObject _roomLightRef;

	// Token: 0x040004E3 RID: 1251
	private int _frameCounter;
}
