using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002EC RID: 748
	public class TrainCrossingController : MonoBehaviour
	{
		// Token: 0x0600144A RID: 5194 RVA: 0x00072B4A File Offset: 0x00070D4A
		private void Awake()
		{
			this._lightsSide1Mat = this._lightSide1Renderer.material;
			this._lightsSide2Mat = this._lightSide2Renderer.material;
		}

		// Token: 0x0600144B RID: 5195 RVA: 0x00072B6E File Offset: 0x00070D6E
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

		// Token: 0x0600144C RID: 5196 RVA: 0x00072BA2 File Offset: 0x00070DA2
		public void CrossingButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this.ActivateTrainCrossing();
			}
			this._toolInteractingWithMe = ((obj.NewInteractableState > InteractableState.Default) ? obj.Tool : null);
		}

		// Token: 0x0600144D RID: 5197 RVA: 0x00072BD0 File Offset: 0x00070DD0
		private void Update()
		{
			if (this._toolInteractingWithMe == null)
			{
				this._selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;
				return;
			}
			this._selectionCylinder.CurrSelectionState = ((this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown || this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay) ? SelectionCylinder.SelectionState.Highlighted : SelectionCylinder.SelectionState.Selected);
		}

		// Token: 0x0600144E RID: 5198 RVA: 0x00072C24 File Offset: 0x00070E24
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

		// Token: 0x0600144F RID: 5199 RVA: 0x00072CA2 File Offset: 0x00070EA2
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

		// Token: 0x06001450 RID: 5200 RVA: 0x00072CB8 File Offset: 0x00070EB8
		private void AffectMaterials(Material[] materials, Color newColor)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				materials[i].SetColor(this._colorId, newColor);
			}
		}

		// Token: 0x06001451 RID: 5201 RVA: 0x00072CE4 File Offset: 0x00070EE4
		private void ToggleLightObjects(bool enableState)
		{
			this._lightSide1Renderer.gameObject.SetActive(enableState);
			this._lightSide2Renderer.gameObject.SetActive(enableState);
		}

		// Token: 0x040016F0 RID: 5872
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x040016F1 RID: 5873
		[SerializeField]
		private AudioClip[] _crossingSounds;

		// Token: 0x040016F2 RID: 5874
		[SerializeField]
		private MeshRenderer _lightSide1Renderer;

		// Token: 0x040016F3 RID: 5875
		[SerializeField]
		private MeshRenderer _lightSide2Renderer;

		// Token: 0x040016F4 RID: 5876
		[SerializeField]
		private SelectionCylinder _selectionCylinder;

		// Token: 0x040016F5 RID: 5877
		private Material _lightsSide1Mat;

		// Token: 0x040016F6 RID: 5878
		private Material _lightsSide2Mat;

		// Token: 0x040016F7 RID: 5879
		private int _colorId = Shader.PropertyToID("_Color");

		// Token: 0x040016F8 RID: 5880
		private Coroutine _xingAnimationCr;

		// Token: 0x040016F9 RID: 5881
		private InteractableTool _toolInteractingWithMe;
	}
}
