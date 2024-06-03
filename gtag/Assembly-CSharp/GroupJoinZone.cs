using System;

[Flags]
public enum GroupJoinZone
{
	Basement = 1,
	Beach = 2,
	Cave = 4,
	Canyon = 8,
	City = 16,
	Clouds = 32,
	Forest = 64,
	Mountain = 128,
	Rotating = 256,
	_LastPrimaryRoom = 4096,
	TreeRoom = 8192,
	MountainTunnel = 16384,
	BasementTunnel = 32768,
	RotatingTunnel = 65536,
	BeachTunnel = 131072,
	CloudsElevator = 262144
}
