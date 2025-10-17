using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class CardType_Color : NetworkBehaviour, IPointerClickHandler
{ 
    public enum CardType { Attack, Recruit, Defense } 
    public enum CardColor { Green, Orange, Red, Any }

    [field: SerializeField] [field: ReadOnly] public CardColor LocalColor { get; private set; }
    [HideInInspector] public NetworkVariable<CardType> Type = new NetworkVariable<CardType>();

    [SerializeField] [field: ReadOnly] private Image _reverseType;
    
    public static bool CardPicked { get; set; }
    
    public override void OnNetworkSpawn() 
    { 
        GetRandomTypeRpc(); 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CardPicked || !NetworkHandler.Instance.IsMyTurn) return;
        for (var i = 0; i < 5; i++) DeckPressed();
    }

    private void DeckPressed()
    {
        SpawnCardServerRpc();
        GetRandomTypeRpc();
        CentralZone.Instance.ChangeCentralZone(CentralZone.CentralZoneState.DiscardPile);
        CardPicked = true;
    }

    [Rpc(SendTo.Server, RequireOwnership = false)] private void SpawnCardServerRpc(RpcParams rpcParams = default)
    {
        if (!IsServer) return;

        ulong ownerId = rpcParams.Receive.SenderClientId;
        
        var sourceArray = Type.Value switch
        {
            CardType.Attack => CardHandler.Instance.AttackCardPrefabs,
            CardType.Recruit => CardHandler.Instance.RecruitCardPrefabs,
            CardType.Defense => CardHandler.Instance.DefenseCardPrefabs,
            _ => throw new Exception("Invalid card type") 
        };

        // Filtrar por color
        var filtered = sourceArray.Where(c =>
            {
                var card = c.GetComponent<CardNetwork>();
                return card != null && (card.ValueCardColor == LocalColor || card.ValueCardColor == CardColor.Any);
            }).ToArray();
        
        var cardNetworkInstance = Instantiate(filtered[UnityEngine.Random.Range(0, filtered.Length)]).GetComponent<CardNetwork>();
        
        if (cardNetworkInstance.ValueCardColor == CardColor.Any) cardNetworkInstance.ValueCardColor = LocalColor;
        
        cardNetworkInstance.GetComponent<NetworkObject>().SpawnWithOwnership(ownerId);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)] private void GetRandomTypeRpc()
    {
        Type.Value = (CardType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(CardType)).Length);
        
        ChangeCardTypeRpc(Type.Value); 
    }

    //Reroll card type in the deck 
    [Rpc(SendTo.ClientsAndHost)] 
    private void ChangeCardTypeRpc(CardType type)
    { 
        _reverseType.sprite = type switch 
        { 
            CardType.Attack => CardHandler.Instance.TypeImages[0], 
            CardType.Defense => CardHandler.Instance.TypeImages[1],
            CardType.Recruit => CardHandler.Instance.TypeImages[2],
            _ => throw new Exception("Invalid card type")
        };
    }
}
