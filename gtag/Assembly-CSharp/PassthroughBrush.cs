using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B4 RID: 180
public class PassthroughBrush : MonoBehaviour
{
	// Token: 0x060003FA RID: 1018 RVA: 0x0001A7DC File Offset: 0x000189DC
	private void OnDisable()
	{
		this.brushStatus = PassthroughBrush.BrushState.Idle;
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x0001A7E8 File Offset: 0x000189E8
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.LookRotation(base.transform.position - Camera.main.transform.position);
		if (this.controllerHand != OVRInput.Controller.LTouch && this.controllerHand != OVRInput.Controller.RTouch)
		{
			return;
		}
		Vector3 position = base.transform.position;
		PassthroughBrush.BrushState brushState = this.brushStatus;
		if (brushState != PassthroughBrush.BrushState.Idle)
		{
			if (brushState != PassthroughBrush.BrushState.Inking)
			{
				return;
			}
			this.UpdateLine(position);
			if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, this.controllerHand))
			{
				this.brushStatus = PassthroughBrush.BrushState.Idle;
			}
		}
		else
		{
			if (OVRInput.GetUp(OVRInput.Button.One, this.controllerHand))
			{
				this.UndoInkLine();
			}
			if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, this.controllerHand))
			{
				this.StartLine(position);
				this.brushStatus = PassthroughBrush.BrushState.Inking;
				return;
			}
		}
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x0001A8A8 File Offset: 0x00018AA8
	private void StartLine(Vector3 inkPos)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.lineSegmentPrefab, inkPos, Quaternion.identity);
		this.currentLineSegment = gameObject.GetComponent<LineRenderer>();
		this.currentLineSegment.positionCount = 1;
		this.currentLineSegment.SetPosition(0, inkPos);
		this.strokeWidth = this.currentLineSegment.startWidth;
		this.strokeLength = 0f;
		this.inkPositions.Clear();
		this.inkPositions.Add(inkPos);
		gameObject.transform.parent = this.lineContainer.transform;
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x0001A938 File Offset: 0x00018B38
	private void UpdateLine(Vector3 inkPos)
	{
		float magnitude = (inkPos - this.inkPositions[this.inkPositions.Count - 1]).magnitude;
		if (magnitude >= this.minInkDist)
		{
			this.inkPositions.Add(inkPos);
			this.currentLineSegment.positionCount = this.inkPositions.Count;
			this.currentLineSegment.SetPositions(this.inkPositions.ToArray());
			this.strokeLength += magnitude;
			this.currentLineSegment.material.SetFloat("_LineLength", this.strokeLength / this.strokeWidth);
		}
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x0001A9E0 File Offset: 0x00018BE0
	public void ClearLines()
	{
		for (int i = 0; i < this.lineContainer.transform.childCount; i++)
		{
			Object.Destroy(this.lineContainer.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x0001AA24 File Offset: 0x00018C24
	public void UndoInkLine()
	{
		if (this.lineContainer.transform.childCount >= 1)
		{
			Object.Destroy(this.lineContainer.transform.GetChild(this.lineContainer.transform.childCount - 1).gameObject);
		}
	}

	// Token: 0x040004A7 RID: 1191
	public OVRInput.Controller controllerHand;

	// Token: 0x040004A8 RID: 1192
	public GameObject lineSegmentPrefab;

	// Token: 0x040004A9 RID: 1193
	public GameObject lineContainer;

	// Token: 0x040004AA RID: 1194
	public bool forceActive = true;

	// Token: 0x040004AB RID: 1195
	private LineRenderer currentLineSegment;

	// Token: 0x040004AC RID: 1196
	private List<Vector3> inkPositions = new List<Vector3>();

	// Token: 0x040004AD RID: 1197
	private float minInkDist = 0.01f;

	// Token: 0x040004AE RID: 1198
	private float strokeWidth = 0.1f;

	// Token: 0x040004AF RID: 1199
	private float strokeLength;

	// Token: 0x040004B0 RID: 1200
	private PassthroughBrush.BrushState brushStatus;

	// Token: 0x020003D4 RID: 980
	public enum BrushState
	{
		// Token: 0x04001C1A RID: 7194
		Idle,
		// Token: 0x04001C1B RID: 7195
		Inking
	}
}
