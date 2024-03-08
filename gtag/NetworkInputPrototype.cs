using System;
using System.Runtime.InteropServices;
using Fusion;

[NetworkInputWeaved(4)]
[StructLayout(LayoutKind.Explicit, Size = 16)]
public struct NetworkInputPrototype : INetworkInput
{
	public bool IsUp(int button)
	{
		return !this.Buttons.IsSet(button);
	}

	public bool IsDown(int button)
	{
		return this.Buttons.IsSet(button);
	}

	public const int BUTTON_USE = 0;

	public const int BUTTON_FIRE = 1;

	public const int BUTTON_FIRE_ALT = 2;

	public const int BUTTON_FORWARD = 3;

	public const int BUTTON_BACKWARD = 4;

	public const int BUTTON_LEFT = 5;

	public const int BUTTON_RIGHT = 6;

	public const int BUTTON_JUMP = 7;

	public const int BUTTON_CROUCH = 8;

	public const int BUTTON_WALK = 9;

	public const int BUTTON_ACTION1 = 10;

	public const int BUTTON_ACTION2 = 11;

	public const int BUTTON_ACTION3 = 12;

	public const int BUTTON_ACTION4 = 14;

	public const int BUTTON_RELOAD = 15;

	[FieldOffset(0)]
	public NetworkButtons Buttons;

	[FieldOffset(4)]
	public byte Weapon;

	[FieldOffset(8)]
	public Angle Yaw;

	[FieldOffset(12)]
	public Angle Pitch;
}
