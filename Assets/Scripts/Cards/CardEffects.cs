using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardEffects : NetworkBehaviour, IPointerClickHandler
{
    private static int _stealQuantity;
    private static ulong _stealerClientId;
    private CardNetwork _cardNetwork;

    private void OnEnable()
    {
        _cardNetwork = GetComponent<CardNetwork>();
    }
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void StealRpc(int quantity, RpcParams rpcParams = default)
    {
        _stealQuantity = quantity;
        _stealerClientId = rpcParams.Receive.SenderClientId;
    }
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void StealCardServerRpc(RpcParams rpcParams = default)
    {
        if (_stealQuantity <= 0) return;
        
        var clientId = rpcParams.Receive.SenderClientId;
        var senderIsOwner = clientId == OwnerClientId; //You're clicking on your own card
        var senderIsStealer = clientId == _stealerClientId; //You're the thief
        
        if (!senderIsStealer || senderIsOwner) return;
        
        _cardNetwork.ChangeCardHandLocalRpc(clientId);
        
        _stealQuantity--;
        if (_stealQuantity <= 0) NetworkHandler.Instance.EndTurnServerRpc();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_cardNetwork.CardDiscardedValue) StealCardServerRpc();
    }
}
