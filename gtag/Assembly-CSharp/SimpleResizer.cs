using System;
using UnityEngine;

public class SimpleResizer
{
	public void CreateResizedObject(Vector3 newSize, GameObject parent, SimpleResizable sourcePrefab)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(sourcePrefab.gameObject, Vector3.zero, Quaternion.identity);
		gameObject.name = sourcePrefab.name;
		SimpleResizable component = gameObject.GetComponent<SimpleResizable>();
		component.NewSize = newSize;
		if (component == null)
		{
			Debug.LogError("Resizable component missing.");
			return;
		}
		Mesh sharedMesh = this.ProcessVertices(component, newSize);
		MeshFilter component2 = gameObject.GetComponent<MeshFilter>();
		component2.sharedMesh = sharedMesh;
		component2.sharedMesh.RecalculateBounds();
		gameObject.transform.parent = parent.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		Object.Destroy(component);
	}

	private Mesh ProcessVertices(SimpleResizable resizable, Vector3 newSize)
	{
		Mesh mesh = resizable.Mesh;
		Vector3 size = mesh.bounds.size;
		SimpleResizable.Method resizeMethod = (size.x < newSize.x) ? resizable.ScalingX : SimpleResizable.Method.Scale;
		SimpleResizable.Method resizeMethod2 = (size.y < newSize.y) ? resizable.ScalingY : SimpleResizable.Method.Scale;
		SimpleResizable.Method resizeMethod3 = (size.z < newSize.z) ? resizable.ScalingZ : SimpleResizable.Method.Scale;
		Vector3[] vertices = mesh.vertices;
		float pivot = 1f / resizable.DefaultSize.x * resizable.PivotPosition.x;
		float pivot2 = 1f / resizable.DefaultSize.y * resizable.PivotPosition.y;
		float pivot3 = 1f / resizable.DefaultSize.z * resizable.PivotPosition.z;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector = vertices[i];
			vector.x = this.CalculateNewVertexPosition(resizeMethod, vector.x, size.x, newSize.x, resizable.PaddingX, resizable.PaddingXMax, pivot);
			vector.y = this.CalculateNewVertexPosition(resizeMethod2, vector.y, size.y, newSize.y, resizable.PaddingY, resizable.PaddingYMax, pivot2);
			vector.z = this.CalculateNewVertexPosition(resizeMethod3, vector.z, size.z, newSize.z, resizable.PaddingZ, resizable.PaddingZMax, pivot3);
			vertices[i] = vector;
		}
		Mesh mesh2 = Object.Instantiate<Mesh>(mesh);
		mesh2.vertices = vertices;
		return mesh2;
	}

	private float CalculateNewVertexPosition(SimpleResizable.Method resizeMethod, float currentPosition, float currentSize, float newSize, float padding, float paddingMax, float pivot)
	{
		float num = currentSize / 2f * (newSize / 2f * (1f / (currentSize / 2f))) - currentSize / 2f;
		switch (resizeMethod)
		{
		case SimpleResizable.Method.Adapt:
			if (Mathf.Abs(currentPosition) >= padding)
			{
				currentPosition = num * Mathf.Sign(currentPosition) + currentPosition;
			}
			break;
		case SimpleResizable.Method.AdaptWithAsymmetricalPadding:
			if (currentPosition >= padding)
			{
				currentPosition = num * Mathf.Sign(currentPosition) + currentPosition;
			}
			if (currentPosition <= paddingMax)
			{
				currentPosition = num * Mathf.Sign(currentPosition) + currentPosition;
			}
			break;
		case SimpleResizable.Method.Scale:
			currentPosition = newSize / (currentSize / currentPosition);
			break;
		}
		float num2 = newSize * -pivot;
		currentPosition += num2;
		return currentPosition;
	}
}
