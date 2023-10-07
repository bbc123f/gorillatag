using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020001CC RID: 460
public abstract class LerpComponent : MonoBehaviour
{
	// Token: 0x17000084 RID: 132
	// (get) Token: 0x06000BC6 RID: 3014 RVA: 0x0004947C File Offset: 0x0004767C
	// (set) Token: 0x06000BC7 RID: 3015 RVA: 0x00049484 File Offset: 0x00047684
	public float Lerp
	{
		get
		{
			return this._lerp;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (!Mathf.Approximately(this._lerp, num))
			{
				LerpChangedEvent onLerpChanged = this._onLerpChanged;
				if (onLerpChanged != null)
				{
					onLerpChanged.Invoke(num);
				}
			}
			this._lerp = num;
		}
	}

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x06000BC8 RID: 3016 RVA: 0x000494BF File Offset: 0x000476BF
	// (set) Token: 0x06000BC9 RID: 3017 RVA: 0x000494C7 File Offset: 0x000476C7
	public float LerpTime
	{
		get
		{
			return this._lerpLength;
		}
		set
		{
			this._lerpLength = ((value < 0f) ? 0f : value);
		}
	}

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x06000BCA RID: 3018 RVA: 0x000494DF File Offset: 0x000476DF
	protected virtual bool CanRender
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000BCB RID: 3019
	protected abstract void OnLerp(float t);

	// Token: 0x06000BCC RID: 3020 RVA: 0x000494E2 File Offset: 0x000476E2
	protected void RenderLerp()
	{
		this.OnLerp(this._lerp);
	}

	// Token: 0x06000BCD RID: 3021 RVA: 0x000494F0 File Offset: 0x000476F0
	protected virtual int GetState()
	{
		return new ValueTuple<float, int>(this._lerp, 779562875).GetHashCode();
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x0004951B File Offset: 0x0004771B
	protected virtual void Validate()
	{
		if (this._lerpLength < 0f)
		{
			this._lerpLength = 0f;
		}
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x00049535 File Offset: 0x00047735
	[Conditional("UNITY_EDITOR")]
	private void OnDrawGizmosSelected()
	{
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x00049537 File Offset: 0x00047737
	[Conditional("UNITY_EDITOR")]
	private void TryEditorRender(bool playModeCheck = true)
	{
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x00049539 File Offset: 0x00047739
	[Conditional("UNITY_EDITOR")]
	private void LerpToOne()
	{
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x0004953B File Offset: 0x0004773B
	[Conditional("UNITY_EDITOR")]
	private void LerpToZero()
	{
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x0004953D File Offset: 0x0004773D
	[Conditional("UNITY_EDITOR")]
	private void StartPreview(float lerpFrom, float lerpTo)
	{
	}

	// Token: 0x04000F52 RID: 3922
	[SerializeField]
	[Range(0f, 1f)]
	protected float _lerp;

	// Token: 0x04000F53 RID: 3923
	[SerializeField]
	protected float _lerpLength = 1f;

	// Token: 0x04000F54 RID: 3924
	[Space]
	[SerializeField]
	protected LerpChangedEvent _onLerpChanged;

	// Token: 0x04000F55 RID: 3925
	[SerializeField]
	protected bool _previewInEditor = true;

	// Token: 0x04000F56 RID: 3926
	[NonSerialized]
	private bool _previewing;

	// Token: 0x04000F57 RID: 3927
	[NonSerialized]
	private bool _cancelPreview;

	// Token: 0x04000F58 RID: 3928
	[NonSerialized]
	private bool _rendering;

	// Token: 0x04000F59 RID: 3929
	[NonSerialized]
	private int _lastState;

	// Token: 0x04000F5A RID: 3930
	[NonSerialized]
	private float _prevLerpFrom;

	// Token: 0x04000F5B RID: 3931
	[NonSerialized]
	private float _prevLerpTo;
}
