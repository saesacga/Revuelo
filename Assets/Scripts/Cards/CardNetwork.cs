using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CardNetwork : NetworkBehaviour, IPointerClickHandler
{
    #region Members

    public CardType_Color.CardColor CardColorValueRef { get; set; }
    public CardType_Color.CardType CardTypeValueRef { get; set; }
    
    private readonly NetworkVariable<CardType_Color.CardType> _cardType = new NetworkVariable<CardType_Color.CardType>();
    private readonly NetworkVariable<CardType_Color.CardColor> _cardColor = new NetworkVariable<CardType_Color.CardColor>();
    protected CardType_Color.CardType CardType => _cardType.Value;
    public CardType_Color.CardColor CardColor => _cardColor.Value;
    
    protected readonly NetworkVariable<bool> CardDiscarded = new NetworkVariable<bool>();
    public bool CardDiscardedValue => CardDiscarded.Value;
    public static bool UsedCard { get; set; }
    
    protected abstract void OnInitialize();
    protected abstract void UseCardRpc();

    #endregion
    
    public override void OnNetworkSpawn()
    {
        SetCardDataRpc();
        
        ChangeCardHandLocalRpc(OwnerClientId);
        
        OnInitialize();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)] 
    private void SetCardDataRpc()
    {
        _cardType.Value = CardTypeValueRef;
        _cardColor.Value = CardColorValueRef;
    }
    
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
        if (IsOwner && CardType_Color.CardPicked && !CardDiscarded.Value && !UsedCard && NetworkHandler.Instance.IsMyTurn) 
            UseCardRpc();
    }
}
