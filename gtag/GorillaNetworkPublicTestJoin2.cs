using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

public class GorillaNetworkPublicTestJoin2 : GorillaTriggerBox
{
	public void Awake()
	{
		this.count = 0;
	}

	public void LateUpdate()
	{
		try
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
			{
				if (((!Player.Instance.GetComponent<Rigidbody>().useGravity && !Player.Instance.isClimbing) || Player.Instance.GetComponent<Rigidbody>().isKinematic) && !this.waiting && !GorillaNot.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					base.StartCoroutine(this.GracePeriod());
				}
				if ((Player.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || Player.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f) && !this.waiting && !GorillaNot.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					base.StartCoroutine(this.GracePeriod());
				}
				float magnitude = (Player.Instance.transform.position - this.lastPosition).magnitude;
			}
			if (PhotonNetwork.InRoom && GorillaTagger.Instance.otherPlayer != null && GorillaGameManager.instance != null)
			{
				this.tempRig = GorillaGameManager.StaticFindRigForPlayer(GorillaTagger.Instance.otherPlayer);
				if (this.tempRig != null && GorillaTagger.Instance.offlineVRRig != null && (this.tempRig.transform.position - GorillaTagger.Instance.offlineVRRig.transform.position).magnitude > 8f)
				{
					this.count++;
					if (this.count >= 3 && !this.waiting && !GorillaNot.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
					{
						base.StartCoroutine(this.GracePeriod());
					}
				}
			}
			else
			{
				this.count = 0;
			}
			this.lastPosition = Player.Instance.transform.position;
		}
		catch
		{
		}
	}

	private IEnumerator GracePeriod()
	{
		this.waiting = true;
		yield return new WaitForSeconds(30f);
		try
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
			{
				if ((!Player.Instance.GetComponent<Rigidbody>().useGravity && !Player.Instance.isClimbing) || Player.Instance.GetComponent<Rigidbody>().isKinematic)
				{
					GorillaNot.instance.SendReport("gorvity bisdabled", PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
				if (Player.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || Player.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f)
				{
					GorillaNot.instance.SendReport(string.Concat(new string[]
					{
						"jimp 2mcuh.",
						Player.Instance.jumpMultiplier.ToString(),
						".",
						Player.Instance.maxJumpSpeed.ToString(),
						"."
					}), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
				if (GorillaTagger.Instance.sphereCastRadius > 0.04f)
				{
					GorillaNot.instance.SendReport("wack rad. " + GorillaTagger.Instance.sphereCastRadius.ToString(), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
			}
			if (PhotonNetwork.InRoom && GorillaTagger.Instance.otherPlayer != null && GorillaGameManager.instance != null)
			{
				this.tempRig = GorillaGameManager.StaticFindRigForPlayer(GorillaTagger.Instance.otherPlayer);
				if (this.tempRig != null && GorillaTagger.Instance.offlineVRRig != null && (this.tempRig.transform.position - GorillaTagger.Instance.offlineVRRig.transform.position).magnitude > 8f)
				{
					this.count++;
					if (this.count >= 3)
					{
						GorillaNot.instance.SendReport("tee hee", PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
					}
				}
			}
			else
			{
				this.count = 0;
			}
			this.waiting = false;
			yield break;
		}
		catch
		{
			yield break;
		}
		yield break;
	}

	public GorillaNetworkPublicTestJoin2()
	{
	}

	public GameObject[] makeSureThisIsDisabled;

	public GameObject[] makeSureThisIsEnabled;

	public string gameModeName;

	public PhotonNetworkController photonNetworkController;

	public string componentTypeToAdd;

	public GameObject componentTarget;

	public GorillaLevelScreen[] joinScreens;

	public GorillaLevelScreen[] leaveScreens;

	private Transform tosPition;

	private Transform othsTosPosition;

	private PhotonView fotVew;

	private int count;

	private bool waiting;

	private Vector3 lastPosition;

	private VRRig tempRig;

	[CompilerGenerated]
	private sealed class <GracePeriod>d__17 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <GracePeriod>d__17(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			GorillaNetworkPublicTestJoin2 gorillaNetworkPublicTestJoin = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				gorillaNetworkPublicTestJoin.waiting = true;
				this.<>2__current = new WaitForSeconds(30f);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			try
			{
				if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
				{
					if ((!Player.Instance.GetComponent<Rigidbody>().useGravity && !Player.Instance.isClimbing) || Player.Instance.GetComponent<Rigidbody>().isKinematic)
					{
						GorillaNot.instance.SendReport("gorvity bisdabled", PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
					}
					if (Player.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || Player.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f)
					{
						GorillaNot.instance.SendReport(string.Concat(new string[]
						{
							"jimp 2mcuh.",
							Player.Instance.jumpMultiplier.ToString(),
							".",
							Player.Instance.maxJumpSpeed.ToString(),
							"."
						}), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
					}
					if (GorillaTagger.Instance.sphereCastRadius > 0.04f)
					{
						GorillaNot.instance.SendReport("wack rad. " + GorillaTagger.Instance.sphereCastRadius.ToString(), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
					}
				}
				if (PhotonNetwork.InRoom && GorillaTagger.Instance.otherPlayer != null && GorillaGameManager.instance != null)
				{
					gorillaNetworkPublicTestJoin.tempRig = GorillaGameManager.StaticFindRigForPlayer(GorillaTagger.Instance.otherPlayer);
					if (gorillaNetworkPublicTestJoin.tempRig != null && GorillaTagger.Instance.offlineVRRig != null && (gorillaNetworkPublicTestJoin.tempRig.transform.position - GorillaTagger.Instance.offlineVRRig.transform.position).magnitude > 8f)
					{
						gorillaNetworkPublicTestJoin.count++;
						if (gorillaNetworkPublicTestJoin.count >= 3)
						{
							GorillaNot.instance.SendReport("tee hee", PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
						}
					}
				}
				else
				{
					gorillaNetworkPublicTestJoin.count = 0;
				}
				gorillaNetworkPublicTestJoin.waiting = false;
			}
			catch
			{
			}
			return false;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public GorillaNetworkPublicTestJoin2 <>4__this;
	}
}
