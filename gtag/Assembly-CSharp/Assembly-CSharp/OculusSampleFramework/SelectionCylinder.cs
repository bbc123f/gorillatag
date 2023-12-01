using System;
using UnityEngine;

namespace OculusSampleFramework
{
	public class SelectionCylinder : MonoBehaviour
	{
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

		private void AffectSelectionColor(Color[] newColors)
		{
			int num = newColors.Length;
			for (int i = 0; i < num; i++)
			{
				this._selectionMaterials[i].SetColor(SelectionCylinder._colorId, newColors[i]);
			}
		}

		[SerializeField]
		private MeshRenderer _selectionMeshRenderer;

		private static int _colorId = Shader.PropertyToID("_Color");

		private Material[] _selectionMaterials;

		private Color[] _defaultSelectionColors;

		private Color[] _highlightColors;

		private SelectionCylinder.SelectionState _currSelectionState;

		public enum SelectionState
		{
			Off,
			Selected,
			Highlighted
		}
	}
}
