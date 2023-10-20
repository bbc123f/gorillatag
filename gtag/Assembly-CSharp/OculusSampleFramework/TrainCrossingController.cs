using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002EE RID: 750
	public class TrainCrossingController : MonoBehaviour
	{
		// Token: 0x06001451 RID: 5201 RVA: 0x00073016 File Offset: 0x00071216
		private void Awake()
		{
			this._lightsSide1Mat = this._lightSide1Renderer.material;
			this._lightsSide2Mat = this._lightSide2Renderer.material;
		}

		// Token: 0x06001452 RID: 5202 RVA: 0x0007303A File Offset: 0x0007123A
		private void OnDestroy()
		{
			if (this._lightsSide1Mat != null)
			{
				Object.Destroy(this._lightsSide1Mat);
			}
			if (this._lightsSide2Mat != null)
			{
				Object.Destroy(this._lightsSide2Mat);
			}
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x0007306E File Offset: 0x0007126E
		public void CrossingButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this.ActivateTrainCrossing();
			}
			this._toolInteractingWithMe = ((obj.NewInteractableState > InteractableState.Default) ? obj.Tool : null);
		}

		// Token: 0x06001454 RID: 5204 RVA: 0x0007309C File Offset: 0x0007129C
		private void Update()
		{
			if (this._toolInteractingWithMe == null)
			{
				this._selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;
				return;
			}
			this._selectionCylinder.CurrSelectionState = ((this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown || this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay) ? SelectionCylinder.SelectionState.Highlighted : SelectionCylinder.SelectionState.Selected);
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x000730F0 File Offset: 0x000712F0
		private void ActivateTrainCrossing()
		{
			int num = this._crossingSounds.Length - 1;
			AudioClip audioClip = this._crossingSounds[(int)(Random.value * (float)num)];
			this._audioSource.clip = audioClip;
			this._audioSource.timeSamples = 0;
			this._audioSource.Play();
			if (this._xingAnimationCr != null)
			{
				base.StopCoroutine(this._xingAnimationCr);
			}
			this._xingAnimationCr = base.StartCoroutine(this.AnimateCrossing(audioClip.length * 0.75f));
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x0007316E File Offset: 0x0007136E
		private IEnumerator AnimateCrossing(float animationLength)
		{
			this.ToggleLightObjects(true);
			float animationEndTime = Time.time + animationLength;
			float lightBlinkDuration = animationLength * 0.1f;
			float lightBlinkStartTime = Time.time;
			float lightBlinkEndTime = Time.time + lightBlinkDuration;
			Material lightToBlinkOn = this._lightsSide1Mat;
			Material lightToBlinkOff = this._lightsSide2Mat;
			Color onColor = new Color(1f, 1f, 1f, 1f);
			Color offColor = new Color(1f, 1f, 1f, 0f);
			while (Time.time < animationEndTime)
			{
				float t = (Time.time - lightBlinkStartTime) / lightBlinkDuration;
				lightToBlinkOn.SetColor(this._colorId, Color.Lerp(offColor, onColor, t));
				lightToBlinkOff.SetColor(this._colorId, Color.Lerp(onColor, offColor, t));
				if (Time.time > lightBlinkEndTime)
				{
					Material material = lightToBlinkOn;
					lightToBlinkOn = lightToBlinkOff;
					lightToBlinkOff = material;
					lightBlinkStartTime = Time.time;
					lightBlinkEndTime = Time.time + lightBlinkDuration;
				}
				yield return null;
			}
			this.ToggleLightObjects(false);
			yield break;
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x00073184 File Offset: 0x00071384
		private void AffectMaterials(Material[] materials, Color newColor)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				materials[i].SetColor(this._colorId, newColor);
			}
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x000731B0 File Offset: 0x000713B0
		private void ToggleLightObjects(bool enableState)
		{
			this._lightSide1Renderer.gameObject.SetActive(enableState);
			this._lightSide2Renderer.gameObject.SetActive(enableState);
		}

		// Token: 0x040016FD RID: 5885
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x040016FE RID: 5886
		[SerializeField]
		private AudioClip[] _crossingSounds;

		// Token: 0x040016FF RID: 5887
		[SerializeField]
		private MeshRenderer _lightSide1Renderer;

		// Token: 0x04001700 RID: 5888
		[SerializeField]
		private MeshRenderer _lightSide2Renderer;

		// Token: 0x04001701 RID: 5889
		[SerializeField]
		private SelectionCylinder _selectionCylinder;

		// Token: 0x04001702 RID: 5890
		private Material _lightsSide1Mat;

		// Token: 0x04001703 RID: 5891
		private Material _lightsSide2Mat;

		// Token: 0x04001704 RID: 5892
		private int _colorId = Shader.PropertyToID("_Color");

		// Token: 0x04001705 RID: 5893
		private Coroutine _xingAnimationCr;

		// Token: 0x04001706 RID: 5894
		private InteractableTool _toolInteractingWithMe;
	}
}
