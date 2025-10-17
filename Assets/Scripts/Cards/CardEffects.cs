using Unity.Netcode;
using UnityEngine.EventSystems;

public class CardEffects : NetworkBehaviour, IPointerClickHandler
{
    #region Setup

    private CardNetwork _cardNetwork;
    private void OnEnable()
    {
        _cardNetwork = GetComponent<CardNetwork>();
    }

    #endregion
    
    [Rpc(SendTo.ClientsAndHost)]
    public void ChangeCardHandLocalRpc(ulong clientId)
    {
        int seat = NetworkHandler.Instance.PlayerSeats[clientId]; //Pick a local seat for each card based on an owner

        transform.SetParent(CardHandler.Instance.SeatGrids[seat]); //Set new card parent to local player grid
        int lastIndex = CardHandler.Instance.SeatGrids[seat].childCount - 2;
        transform.SetSiblingIndex(lastIndex); //Change the position in hierarchy so cards instantiate in the middle of the hand

        if (IsServer) GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        #region Steal

        var clickerId = NetworkManager.Singleton.LocalClientId;
        var stealerId = CardHandler.Instance.StealerClientId;
        var quantity = CardHandler.Instance.StealQuantity;

        if (quantity <= 0 || clickerId != stealerId || clickerId == OwnerClientId || _cardNetwork.CardDiscardedValue) return;
        
        ChangeCardHandLocalRpc(stealerId);
        CardHandler.Instance.ReduceStealQuantityServerRpc();

        #endregion
    }
}
