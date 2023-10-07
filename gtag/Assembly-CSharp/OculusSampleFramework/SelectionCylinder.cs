using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002E7 RID: 743
	public class SelectionCylinder : MonoBehaviour
	{
		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06001417 RID: 5143 RVA: 0x00071CA1 File Offset: 0x0006FEA1
		// (set) Token: 0x06001418 RID: 5144 RVA: 0x00071CAC File Offset: 0x0006FEAC
		public SelectionCylinder.SelectionState CurrSelectionState
		{
			get
			{
				return this._currSelectionState;
			}
			set
			{
				SelectionCylinder.SelectionState currSelectionState = this._currSelectionState;
				this._currSelectionState = value;
				if (currSelectionState != this._currSelectionState)
				{
					if (this._currSelectionState > SelectionCylinder.SelectionState.Off)
					{
						this._selectionMeshRenderer.enabled = true;
						this.AffectSelectionColor((this._currSelectionState == SelectionCylinder.SelectionState.Selected) ? this._defaultSelectionColors : this._highlightColors);
						return;
					}
					this._selectionMeshRenderer.enabled = false;
				}
			}
		}

		// Token: 0x06001419 RID: 5145 RVA: 0x00071D10 File Offset: 0x0006FF10
		private void Awake()
		{
			this._selectionMaterials = this._selectionMeshRenderer.materials;
			int num = this._selectionMaterials.Length;
			this._defaultSelectionColors = new Color[num];
			this._highlightColors = new Color[num];
			for (int i = 0; i < num; i++)
			{
				this._defaultSelectionColors[i] = this._selectionMaterials[i].GetColor(SelectionCylinder._colorId);
				this._highlightColors[i] = new Color(1f, 1f, 1f, this._defaultSelectionColors[i].a);
			}
			this.CurrSelectionState = SelectionCylinder.SelectionState.Off;
		}

		// Token: 0x0600141A RID: 5146 RVA: 0x00071DB4 File Offset: 0x0006FFB4
		private void OnDestroy()
		{
			if (this._selectionMaterials != null)
			{
				foreach (Material material in this._selectionMaterials)
				{
					if (material != null)
					{
						Object.Destroy(material);
					}
				}
			}
		}

		// Token: 0x0600141B RID: 5147 RVA: 0x00071DF4 File Offset: 0x0006FFF4
		private void AffectSelectionColor(Color[] newColors)
		{
			int num = newColors.Length;
			for (int i = 0; i < num; i++)
			{
				this._selectionMaterials[i].SetColor(SelectionCylinder._colorId, newColors[i]);
			}
		}

		// Token: 0x040016BF RID: 5823
		[SerializeField]
		private MeshRenderer _selectionMeshRenderer;

		// Token: 0x040016C0 RID: 5824
		private static int _colorId = Shader.PropertyToID("_Color");

		// Token: 0x040016C1 RID: 5825
		private Material[] _selectionMaterials;

		// Token: 0x040016C2 RID: 5826
		private Color[] _defaultSelectionColors;

		// Token: 0x040016C3 RID: 5827
		private Color[] _highlightColors;

		// Token: 0x040016C4 RID: 5828
		private SelectionCylinder.SelectionState _currSelectionState;

		// Token: 0x020004ED RID: 1261
		public enum SelectionState
		{
			// Token: 0x04002081 RID: 8321
			Off,
			// Token: 0x04002082 RID: 8322
			Selected,
			// Token: 0x04002083 RID: 8323
			Highlighted
		}
	}
}
