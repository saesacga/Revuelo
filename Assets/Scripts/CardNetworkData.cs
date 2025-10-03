using Unity.Netcode;
using UnityEngine;

public class CardNetworkData : NetworkBehaviour
{
    private BaseCard _visualBaseCardRef;

    public CardType_Color.CardColor CardColorValueRef { get; set; }
    public CardType_Color.CardType CardTypeValueRef { get; set; }
    [SerializeField, ReadOnly] private NetworkVariable<CardType_Color.CardType> _cardType = new NetworkVariable<CardType_Color.CardType>();
    [SerializeField, ReadOnly] private NetworkVariable<CardType_Color.CardColor> _cardColor = new NetworkVariable<CardType_Color.CardColor>();

    public override void OnNetworkSpawn()
    {
        SetCardDataRpc();

        int seat = NetworkHandler.Instance.PlayerSeats[GetComponent<NetworkObject>().OwnerClientId]; //Pick a local seat for each card based on an owner
        
        GameObject cardInstance = Instantiate(CardHandler.Instance.BaseCardPrefab); //Create a new card

        switch (_cardType.Value)
        {
            case CardType_Color.CardType.Attack:
                cardInstance.AddComponent<BaseAtkCardV1>();
                break;
            case CardType_Color.CardType.Defense:
                cardInstance.AddComponent<BaseDefV1>();
                break;
            case CardType_Color.CardType.Recruit:
                cardInstance.AddComponent<BaseRecV1>();
                break;
        }

        cardInstance.GetComponent<BaseCard>().Initialize(_cardColor.Value, _cardType.Value, seat, this);
        
        _visualBaseCardRef = cardInstance.GetComponent<BaseCard>();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SetCardDataRpc()
    {
        _cardType.Value = CardTypeValueRef;
        _cardColor.Value = CardColorValueRef;
    }

    [Rpc(SendTo.Server, RequireOwnership = false)] 
    public void ChangeCardHandServerRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        bool senderIsOwner = clientId == OwnerClientId;
        ChangeCardHandLocalRpc(clientId, senderIsOwner);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void ChangeCardHandLocalRpc(ulong clientId, bool senderIsOwner)
    {
        if (_visualBaseCardRef == null) return;
        
        if (senderIsOwner)
        {
            _visualBaseCardRef.transform.SetParent(CardHandler.Instance.DiscardPile);
            return;
        }
        
        //For stealing
        
        int seat = NetworkHandler.Instance.PlayerSeats[clientId];
        
        _visualBaseCardRef.transform.SetParent(CardHandler.Instance.SeatGrids[seat]); 
        int lastIndex = CardHandler.Instance.SeatGrids[seat].childCount - 2;
        _visualBaseCardRef.transform.SetSiblingIndex(lastIndex);

        if (IsServer) GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }

    public NetworkVariable<bool> CardDiscarded = new NetworkVariable<bool>();
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void CardDiscardedServerRpc(bool value)
    {
        CardDiscarded.Value = value;
    }
}
