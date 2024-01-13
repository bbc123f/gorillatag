using Photon.Realtime;

public interface IRequestableOwnershipGuardCallbacks
{
	void OnOwnershipTransferred(Player toPlayer, Player fromPlayer);

	bool OnOwnershipRequest(Player fromPlayer);

	void OnMyOwnerLeft();

	bool OnMasterClientAssistedTakeoverRequest(Player fromPlayer, Player toPlayer);

	void OnMyCreatorLeft();
}
