using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents
{
	public GameEvents()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static GameEvents()
	{
	}

	public static UnityEvent<GorillaKeyboardBindings> OnGorrillaKeyboardButtonPressedEvent = new UnityEvent<GorillaKeyboardBindings>();

	internal static UnityEvent<string> ScreenTextChangedEvent = new UnityEvent<string>();

	internal static UnityEvent<Material[]> ScreenTextMaterialsEvent = new UnityEvent<Material[]>();

	internal static UnityEvent<string> FunctionSelectTextChangedEvent = new UnityEvent<string>();

	internal static UnityEvent<Material[]> FunctionTextMaterialsEvent = new UnityEvent<Material[]>();

	internal static UnityEvent<string> ScoreboardTextChangedEvent = new UnityEvent<string>();

	internal static UnityEvent<Material[]> ScoreboardMaterialsEvent = new UnityEvent<Material[]>();
}
