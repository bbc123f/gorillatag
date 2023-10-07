using System;
using UnityEngine;

// Token: 0x02000016 RID: 22
public class MousePositionDrag : MonoBehaviour
{
	// Token: 0x0600004A RID: 74 RVA: 0x0000427E File Offset: 0x0000247E
	private void Start()
	{
		this.m_currFrameHasFocus = false;
		this.m_prevFrameHasFocus = false;
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00004290 File Offset: 0x00002490
	private void Update()
	{
		this.m_currFrameHasFocus = Application.isFocused;
		bool prevFrameHasFocus = this.m_prevFrameHasFocus;
		this.m_prevFrameHasFocus = this.m_currFrameHasFocus;
		if (!prevFrameHasFocus && !this.m_currFrameHasFocus)
		{
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		Vector3 prevMousePosition = this.m_prevMousePosition;
		Vector3 a = mousePosition - prevMousePosition;
		this.m_prevMousePosition = mousePosition;
		if (!prevFrameHasFocus)
		{
			return;
		}
		if (Input.GetMouseButton(0))
		{
			base.transform.position += 0.02f * a;
		}
	}

	// Token: 0x04000071 RID: 113
	private bool m_currFrameHasFocus;

	// Token: 0x04000072 RID: 114
	private bool m_prevFrameHasFocus;

	// Token: 0x04000073 RID: 115
	private Vector3 m_prevMousePosition;
}
