﻿using System;
using UnityEngine;

public class SimpleResizer
{
	public void CreateResizedObject(Vector3 newSize, GameObject parent, SimpleResizable sourcePrefab)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(sourcePrefab.gameObject, Vector3.zero, Quaternion.identity);
		gameObject.name = sourcePrefab.name;
		SimpleResizable component = gameObject.GetComponent<SimpleResizable>();
		component.SetNewSize(newSize);
		if (component == null)
		{
			Debug.LogError("Resizable component missing.");
			return;
		}
		Mesh mesh = SimpleResizer.ProcessVertices(component, newSize, false);
		MeshFilter component2 = gameObject.GetComponent<MeshFilter>();
		component2.sharedMesh = mesh;
		component2.sharedMesh.RecalculateBounds();
		gameObject.transform.parent = parent.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		Object.Destroy(component);
	}

	internal static Mesh ProcessVertices(SimpleResizable resizable, Vector3 newSize, bool pivot = false)
	{
		Mesh originalMesh = resizable.OriginalMesh;
		Vector3 defaultSize = resizable.DefaultSize;
		SimpleResizable.Method method = ((defaultSize.x < newSize.x) ? resizable.ScalingX : SimpleResizable.Method.Scale);
		SimpleResizable.Method method2 = ((defaultSize.y < newSize.y) ? resizable.ScalingY : SimpleResizable.Method.Scale);
		SimpleResizable.Method method3 = ((defaultSize.z < newSize.z) ? resizable.ScalingZ : SimpleResizable.Method.Scale);
		Vector3[] vertices = originalMesh.vertices;
		Vector3 vector = resizable.transform.InverseTransformPoint(resizable.PivotPosition);
		float num = 1f / resizable.DefaultSize.x * vector.x;
		float num2 = 1f / resizable.DefaultSize.y * vector.y;
		float num3 = 1f / resizable.DefaultSize.z * vector.z;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector2 = vertices[i];
			vector2.x = SimpleResizer.CalculateNewVertexPosition(method, vector2.x, defaultSize.x, newSize.x, resizable.PaddingX, resizable.PaddingXMax, num);
			vector2.y = SimpleResizer.CalculateNewVertexPosition(method2, vector2.y, defaultSize.y, newSize.y, resizable.PaddingY, resizable.PaddingYMax, num2);
			vector2.z = SimpleResizer.CalculateNewVertexPosition(method3, vector2.z, defaultSize.z, newSize.z, resizable.PaddingZ, resizable.PaddingZMax, num3);
			if (pivot)
			{
				vector2 += vector;
			}
			vertices[i] = vector2;
		}
		Mesh mesh = Object.Instantiate<Mesh>(originalMesh);
		mesh.vertices = vertices;
		return mesh;
	}

	private static float CalculateNewVertexPosition(SimpleResizable.Method resizeMethod, float currentPosition, float currentSize, float newSize, float padding, float paddingMax, float pivot)
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

	public SimpleResizer()
	{
	}
}
