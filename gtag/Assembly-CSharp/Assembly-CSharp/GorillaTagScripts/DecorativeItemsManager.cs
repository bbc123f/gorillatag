using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	public class DecorativeItemsManager : MonoBehaviourPun, IPunObservable
	{
		public static DecorativeItemsManager Instance
		{
			get
			{
				return DecorativeItemsManager._instance;
			}
		}

		private void Awake()
		{
			if (DecorativeItemsManager._instance != null && DecorativeItemsManager._instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				DecorativeItemsManager._instance = this;
			}
			this.currentIndex = -1;
			this.shouldRunUpdate = true;
			foreach (DecorativeItem decorativeItem in this.decorativeItemsContainer.GetComponentsInChildren<DecorativeItem>(false))
			{
				if (decorativeItem)
				{
					this.itemsList.Add(decorativeItem);
					DecorativeItem decorativeItem2 = decorativeItem;
					decorativeItem2.respawnItem = (UnityAction<DecorativeItem>)Delegate.Combine(decorativeItem2.respawnItem, new UnityAction<DecorativeItem>(this.OnRequestToRespawn));
				}
			}
			foreach (AttachPoint attachPoint in this.respawnableHooksContainer.GetComponentsInChildren<AttachPoint>(false))
			{
				if (attachPoint)
				{
					this.respawnableHooks.Add(attachPoint);
				}
			}
			this.allHooks.AddRange(this.respawnableHooks);
			foreach (GameObject gameObject in this.nonRespawnableHooksContainer)
			{
				foreach (AttachPoint attachPoint2 in gameObject.GetComponentsInChildren<AttachPoint>(false))
				{
					if (attachPoint2)
					{
						this.allHooks.Add(attachPoint2);
					}
				}
			}
		}

		private void OnDestroy()
		{
			foreach (DecorativeItem decorativeItem in this.itemsList)
			{
				decorativeItem.respawnItem = (UnityAction<DecorativeItem>)Delegate.Remove(decorativeItem.respawnItem, new UnityAction<DecorativeItem>(this.OnRequestToRespawn));
			}
			this.itemsList.Clear();
			this.respawnableHooks.Clear();
			if (DecorativeItemsManager._instance == this)
			{
				DecorativeItemsManager._instance = null;
			}
		}

		private void Update()
		{
			if (!PhotonNetwork.InRoom)
			{
				return;
			}
			if (base.photonView.IsMine)
			{
				if (!this.shouldRunUpdate)
				{
					return;
				}
				this.SpawnItem(this.UpdateListPerFrame());
			}
		}

		private void SpawnItem(int index)
		{
			if (!PhotonNetwork.InRoom)
			{
				return;
			}
			if (index < 0 || index >= this.itemsList.Count)
			{
				return;
			}
			if (this.respawnableHooks == null)
			{
				return;
			}
			if (this.itemsList == null)
			{
				return;
			}
			if (this.itemsList.Count > this.respawnableHooks.Count)
			{
				Debug.LogError("Trying to snap more decorative items than allowed! Some items will be left un-hooked!");
				return;
			}
			Transform transform = this.RandomSpawn();
			if (transform == null)
			{
				return;
			}
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			this.itemsList[index].Respawn(position, rotation);
			base.photonView.RPC("RespawnItemRPC", RpcTarget.Others, new object[]
			{
				index,
				position,
				rotation
			});
		}

		[PunRPC]
		private void RespawnItemRPC(int index, Vector3 _transformPos, Quaternion _transformRot, PhotonMessageInfo info)
		{
			if (index < 0 || index > this.itemsList.Count - 1 || !_transformPos.IsValid() || !_transformRot.IsValid() || info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "RespawnItemRPC");
			this.itemsList[index].Respawn(_transformPos, _transformRot);
		}

		private Transform RandomSpawn()
		{
			this.lastIndex = this.currentIndex;
			int index = Random.Range(0, this.respawnableHooks.Count);
			if (!this.respawnableHooks[index].IsHooked())
			{
				this.currentIndex = index;
			}
			else
			{
				this.currentIndex = -1;
			}
			if (this.currentIndex != this.lastIndex && this.currentIndex > -1)
			{
				return this.respawnableHooks[this.currentIndex].attachPoint;
			}
			this.currentIndex = -1;
			return null;
		}

		private int UpdateListPerFrame()
		{
			this.arrayIndex++;
			if (this.arrayIndex >= this.itemsList.Count || this.arrayIndex < 0)
			{
				this.shouldRunUpdate = false;
				return -1;
			}
			return this.arrayIndex;
		}

		private void OnRequestToRespawn(DecorativeItem item)
		{
			if (base.photonView.IsMine)
			{
				if (item == null)
				{
					return;
				}
				int index = this.itemsList.IndexOf(item);
				this.SpawnItem(index);
			}
		}

		public AttachPoint getCurrentAttachPointByPosition(Vector3 _attachPoint)
		{
			foreach (AttachPoint attachPoint in this.allHooks)
			{
				if (attachPoint.attachPoint.position == _attachPoint)
				{
					return attachPoint;
				}
			}
			return null;
		}

		void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			if (stream.IsWriting)
			{
				stream.SendNext(this.currentIndex);
				return;
			}
			this.currentIndex = (int)stream.ReceiveNext();
		}

		public GameObject decorativeItemsContainer;

		public GameObject respawnableHooksContainer;

		public List<GameObject> nonRespawnableHooksContainer = new List<GameObject>();

		private readonly List<DecorativeItem> itemsList = new List<DecorativeItem>();

		private readonly List<AttachPoint> respawnableHooks = new List<AttachPoint>();

		private readonly List<AttachPoint> allHooks = new List<AttachPoint>();

		private int lastIndex;

		private int currentIndex;

		private int arrayIndex = -1;

		private bool shouldRunUpdate;

		[OnEnterPlay_SetNull]
		private static DecorativeItemsManager _instance;
	}
}
