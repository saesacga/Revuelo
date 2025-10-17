using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CardNetwork : NetworkBehaviour, IPointerClickHandler
{
    #region Members
    
    [EnumToggleButtons]
    public CardType_Color.CardColor ValueCardColor;
    
    private readonly NetworkVariable<CardType_Color.CardColor> _cardColor = new NetworkVariable<CardType_Color.CardColor>();
    protected CardType_Color.CardColor CardColor => _cardColor.Value;
    
    private CardEffects _cardEffects;

    private readonly NetworkVariable<bool> _cardDiscarded = new NetworkVariable<bool>();
    public bool CardDiscardedValue => _cardDiscarded.Value;
    public static bool UsedCard { get; set; }
    
    protected abstract void OnInitialize();
    protected abstract void CardEffect();

    #endregion
    
    public override void OnNetworkSpawn()
    {
        Debug.Log("Spawned");
        
        SetCardDataRpc();
        
        _cardEffects = GetComponent<CardEffects>() ?? gameObject.AddComponent<CardEffects>();
        _cardEffects.ChangeCardHandLocalRpc(OwnerClientId);
        
        OnInitialize();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)] 
    private void SetCardDataRpc()
    {
        _cardColor.Value = ValueCardColor;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsOwner || !CardType_Color.CardPicked || _cardDiscarded.Value || UsedCard || !NetworkHandler.Instance.IsMyTurn) 
            return;
        
        UseCardRpc();
        
        CardEffect();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void UseCardRpc()
    {
        if (IsServer) _cardDiscarded.Value = true; 
        
        transform.SetParent(CardHandler.Instance.DiscardPile); 
        UsedCard = true;
    }
}
