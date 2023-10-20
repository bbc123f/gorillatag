using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020001CD RID: 461
public abstract class LerpComponent : MonoBehaviour
{
	// Token: 0x17000086 RID: 134
	// (get) Token: 0x06000BCC RID: 3020 RVA: 0x000496E4 File Offset: 0x000478E4
	// (set) Token: 0x06000BCD RID: 3021 RVA: 0x000496EC File Offset: 0x000478EC
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

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x06000BCE RID: 3022 RVA: 0x00049727 File Offset: 0x00047927
	// (set) Token: 0x06000BCF RID: 3023 RVA: 0x0004972F File Offset: 0x0004792F
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

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x06000BD0 RID: 3024 RVA: 0x00049747 File Offset: 0x00047947
	protected virtual bool CanRender
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000BD1 RID: 3025
	protected abstract void OnLerp(float t);

	// Token: 0x06000BD2 RID: 3026 RVA: 0x0004974A File Offset: 0x0004794A
	protected void RenderLerp()
	{
		this.OnLerp(this._lerp);
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x00049758 File Offset: 0x00047958
	protected virtual int GetState()
	{
		return new ValueTuple<float, int>(this._lerp, 779562875).GetHashCode();
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x00049783 File Offset: 0x00047983
	protected virtual void Validate()
	{
		if (this._lerpLength < 0f)
		{
			this._lerpLength = 0f;
		}
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x0004979D File Offset: 0x0004799D
	[Conditional("UNITY_EDITOR")]
	private void OnDrawGizmosSelected()
	{
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x0004979F File Offset: 0x0004799F
	[Conditional("UNITY_EDITOR")]
	private void TryEditorRender(bool playModeCheck = true)
	{
	}

	// Token: 0x06000BD7 RID: 3031 RVA: 0x000497A1 File Offset: 0x000479A1
	[Conditional("UNITY_EDITOR")]
	private void LerpToOne()
	{
	}

	// Token: 0x06000BD8 RID: 3032 RVA: 0x000497A3 File Offset: 0x000479A3
	[Conditional("UNITY_EDITOR")]
	private void LerpToZero()
	{
	}

	// Token: 0x06000BD9 RID: 3033 RVA: 0x000497A5 File Offset: 0x000479A5
	[Conditional("UNITY_EDITOR")]
	private void StartPreview(float lerpFrom, float lerpTo)
	{
	}

	// Token: 0x04000F56 RID: 3926
	[SerializeField]
	[Range(0f, 1f)]
	protected float _lerp;

	// Token: 0x04000F57 RID: 3927
	[SerializeField]
	protected float _lerpLength = 1f;

	// Token: 0x04000F58 RID: 3928
	[Space]
	[SerializeField]
	protected LerpChangedEvent _onLerpChanged;

	// Token: 0x04000F59 RID: 3929
	[SerializeField]
	protected bool _previewInEditor = true;

	// Token: 0x04000F5A RID: 3930
	[NonSerialized]
	private bool _previewing;

	// Token: 0x04000F5B RID: 3931
	[NonSerialized]
	private bool _cancelPreview;

	// Token: 0x04000F5C RID: 3932
	[NonSerialized]
	private bool _rendering;

	// Token: 0x04000F5D RID: 3933
	[NonSerialized]
	private int _lastState;

	// Token: 0x04000F5E RID: 3934
	[NonSerialized]
	private float _prevLerpFrom;

	// Token: 0x04000F5F RID: 3935
	[NonSerialized]
	private float _prevLerpTo;
}
