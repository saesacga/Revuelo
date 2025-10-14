using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public abstract class CardNetwork : NetworkBehaviour, IPointerClickHandler
{ 
    public CardType_Color.CardColor CardColorValueRef { get; set; }
    public CardType_Color.CardType CardTypeValueRef { get; set; }
    
    private readonly NetworkVariable<CardType_Color.CardType> _cardType = new NetworkVariable<CardType_Color.CardType>();
    private readonly NetworkVariable<CardType_Color.CardColor> _cardColor = new NetworkVariable<CardType_Color.CardColor>();
    
    protected CardType_Color.CardType CardType => _cardType.Value;
    public CardType_Color.CardColor CardColor => _cardColor.Value;
    
    protected abstract void OnInitialize();
    
    public override void OnNetworkSpawn()
    {
        SetCardDataRpc();
        int seat = NetworkHandler.Instance.PlayerSeats[GetComponent<NetworkObject>().OwnerClientId]; //Pick a local seat for each card based on an owner
        
        transform.SetParent(CardHandler.Instance.SeatGrids[seat]); //Set new card parent to local player grid
        int lastIndex = CardHandler.Instance.SeatGrids[seat].childCount - 2;
        transform.SetSiblingIndex(lastIndex); //Change the position in hierarchy so cards instantiate in the middle of the hand
        
        OnInitialize();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)] private void SetCardDataRpc()
    {
        _cardType.Value = CardTypeValueRef;
        _cardColor.Value = CardColorValueRef;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsOwner && CardType_Color.CardPicked && !_cardDiscarded.Value && !UsedCard) 
            UseCardRpc();
    }

    private readonly NetworkVariable<bool> _cardDiscarded = new NetworkVariable<bool>();
    public bool CardDiscarded => _cardDiscarded.Value;
    public static bool UsedCard { get; set; }
    
    [Rpc(SendTo.ClientsAndHost)]
    protected virtual void UseCardRpc()
    {
        if(IsServer) _cardDiscarded.Value = true;
        
        transform.SetParent(CardHandler.Instance.DiscardPile);
        UsedCard = true;
    }
}
