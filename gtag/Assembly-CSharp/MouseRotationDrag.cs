using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class MouseRotationDrag : MonoBehaviour
{
	// Token: 0x0600004D RID: 77 RVA: 0x00004318 File Offset: 0x00002518
	private void Start()
	{
		this.m_currFrameHasFocus = false;
		this.m_prevFrameHasFocus = false;
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00004328 File Offset: 0x00002528
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
		Vector3 vector = mousePosition - prevMousePosition;
		this.m_prevMousePosition = mousePosition;
		if (!prevFrameHasFocus)
		{
			this.m_euler = base.transform.rotation.eulerAngles;
			return;
		}
		if (Input.GetMouseButton(0))
		{
			this.m_euler.x = this.m_euler.x + vector.y;
			this.m_euler.y = this.m_euler.y + vector.x;
			base.transform.rotation = Quaternion.Euler(this.m_euler);
		}
	}

	// Token: 0x04000074 RID: 116
	private bool m_currFrameHasFocus;

	// Token: 0x04000075 RID: 117
	private bool m_prevFrameHasFocus;

	// Token: 0x04000076 RID: 118
	private Vector3 m_prevMousePosition;

	// Token: 0x04000077 RID: 119
	private Vector3 m_euler;
}
