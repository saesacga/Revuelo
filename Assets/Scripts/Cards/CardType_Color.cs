using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class CardType_Color : NetworkBehaviour, IPointerClickHandler
{ 
    public enum CardType { Attack, Recruit, Defense } 
    public enum CardColor { Green, Orange, Red }

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
        if (CardPicked) return;
        for (var i = 0; i < 6; i++) DeckPressed();
    }

    private void DeckPressed()
    {
        SpawnCardServerRpc();
        GetRandomTypeRpc();
        CardPicked = true;
    }

    [Rpc(SendTo.Server, RequireOwnership = false)] private void SpawnCardServerRpc(RpcParams rpcParams = default)
    {
        if (!IsServer) return;

        ulong ownerId = rpcParams.Receive.SenderClientId;
        
        GameObject cardNetworkInstance = Instantiate(CardHandler.Instance.CardPrefabs[UnityEngine.Random.Range(0, CardHandler.Instance.CardPrefabs.Length)]);
        
        cardNetworkInstance.GetComponent<CardNetwork>().CardTypeValueRef = Type.Value; 
        cardNetworkInstance.GetComponent<CardNetwork>().CardColorValueRef = LocalColor;
        
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
