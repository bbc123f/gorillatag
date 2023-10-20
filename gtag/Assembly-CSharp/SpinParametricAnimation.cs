using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000071 RID: 113
public class SpinParametricAnimation : MonoBehaviour
{
	// Token: 0x0600023A RID: 570 RVA: 0x0000F2C7 File Offset: 0x0000D4C7
	protected void OnEnable()
	{
		this.axis = this.axis.normalized;
	}

	// Token: 0x0600023B RID: 571 RVA: 0x0000F2DC File Offset: 0x0000D4DC
	protected void LateUpdate()
	{
		Transform transform = base.transform;
		this._animationProgress = (this._animationProgress + Time.deltaTime * this.revolutionsPerSecond) % 1f;
		float num = this.timeCurve.Evaluate(this._animationProgress) * 360f;
		float angle = num - this._oldAngle;
		this._oldAngle = num;
		if (this.WorldSpaceRotation)
		{
			transform.rotation = Quaternion.AngleAxis(angle, this.axis) * transform.rotation;
			return;
		}
		transform.localRotation = Quaternion.AngleAxis(angle, this.axis) * transform.localRotation;
	}

	// Token: 0x040002E6 RID: 742
	[Tooltip("Axis to rotate around.")]
	public Vector3 axis = Vector3.up;

	// Token: 0x040002E7 RID: 743
	[Tooltip("Whether rotation is in World Space or Local Space")]
	public bool WorldSpaceRotation = true;

	// Token: 0x040002E8 RID: 744
	[FormerlySerializedAs("speed")]
	[Tooltip("Speed of rotation.")]
	public float revolutionsPerSecond = 0.25f;

	// Token: 0x040002E9 RID: 745
	[Tooltip("Affects the progress of the animation over time.")]
	public AnimationCurve timeCurve;

	// Token: 0x040002EA RID: 746
	private float _animationProgress;

	// Token: 0x040002EB RID: 747
	private float _oldAngle;
}
