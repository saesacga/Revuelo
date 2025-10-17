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
    
    private CardEffects _cardEffects;

    private readonly NetworkVariable<bool> _cardDiscarded = new NetworkVariable<bool>();
    public bool CardDiscardedValue => _cardDiscarded.Value;
    public static bool UsedCard { get; set; }
    
    protected abstract void OnInitialize();
    protected abstract void CardEffectRpc();

    #endregion
    
    public override void OnNetworkSpawn()
    {
        SetCardDataRpc();
        
        _cardEffects = GetComponent<CardEffects>() ?? gameObject.AddComponent<CardEffects>();
        _cardEffects.ChangeCardHandLocalRpc(OwnerClientId);
        
        OnInitialize();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)] 
    private void SetCardDataRpc()
    {
        _cardType.Value = CardTypeValueRef;
        _cardColor.Value = CardColorValueRef;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsOwner || !CardType_Color.CardPicked || _cardDiscarded.Value || UsedCard || !NetworkHandler.Instance.IsMyTurn) 
            return;
        
        UseCardRpc();
        
        CardEffectRpc();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void UseCardRpc()
    {
        if (IsServer) _cardDiscarded.Value = true; 
        
        transform.SetParent(CardHandler.Instance.DiscardPile); 
        UsedCard = true;
    }
}
