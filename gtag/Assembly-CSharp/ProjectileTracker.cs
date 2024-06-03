using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

internal static class ProjectileTracker
{
	static ProjectileTracker()
	{
		RoomSystem.LeftRoomEvent = (Action)Delegate.Combine(RoomSystem.LeftRoomEvent, new Action(ProjectileTracker.ClearProjectiles));
	}

	private static void ClearProjectiles()
	{
		List<ProjectileTracker.ProjectileInfo> list = null;
		if (ProjectileTracker.playerProjectiles.ContainsKey(PhotonNetwork.LocalPlayer))
		{
			list = ProjectileTracker.playerProjectiles[PhotonNetwork.LocalPlayer];
		}
		ProjectileTracker.playerProjectiles.Clear();
		if (list != null)
		{
			ProjectileTracker.playerProjectiles[PhotonNetwork.LocalPlayer] = list;
		}
		ProjectileTracker.localPlayerProjectileCounter = 0;
	}

	internal static int IncrementLocalPlayerProjectileCount()
	{
		ProjectileTracker.localPlayerProjectileCounter++;
		return ProjectileTracker.localPlayerProjectileCounter;
	}

	private static int localPlayerProjectileCounter = 0;

	public static int maxProjectilesToKeepTrackOfPerPlayer = 50;

	public static Dictionary<Player, List<ProjectileTracker.ProjectileInfo>> playerProjectiles = new Dictionary<Player, List<ProjectileTracker.ProjectileInfo>>();

	public struct ProjectileInfo
	{
		public ProjectileInfo(double newTime, Vector3 newVel, Vector3 origin, int newCount, float newScale)
		{
			this.timeLaunched = newTime;
			this.shotVelocity = newVel;
			this.launchOrigin = origin;
			this.projectileCount = newCount;
			this.scale = newScale;
		}

		public double timeLaunched;

		public Vector3 shotVelocity;

		public Vector3 launchOrigin;

		public int projectileCount;

		public float scale;
	}
}
