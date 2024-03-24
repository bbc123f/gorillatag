using System;
using UnityEngine;

[CreateAssetMenu(fileName = "IngredientTypeSO", menuName = "ScriptableObjects/Add New Magic Ingredient Type")]
public class MagicIngredientType : ScriptableObject
{
	public MagicIngredientType()
	{
	}

	public Color color;
}
