using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class CardType_Color : NetworkBehaviour
{
    public enum CardType { Attack, Recruit, Defense }
    public enum CardColor { Green, Orange, Red }

    private CardType _cardType;
    [SerializeField] [field: ReadOnly] private CardColor _cardColor;
    
    [SerializeField] [field: ReadOnly] private Image _reverseType;

    private void Start() //Check it, start doesn't work the same with NGO
    {
        ChangeCardType();
    }

    public void DeckPressed()
    {
        SpawnCardServerRpc();
        ChangeCardType();
    }
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SpawnCardServerRpc(RpcParams rpcParams = default)
    {
        if (!IsServer) return;

        ulong ownerId = rpcParams.Receive.SenderClientId;

        GameObject cardNetworkDataInstance = Instantiate(CardHandler.Instance.CardNetworkDataPrefab);
       
        cardNetworkDataInstance.GetComponent<CardNetworkData>().CardTypeValueRef = _cardType;
        cardNetworkDataInstance.GetComponent<CardNetworkData>().CardColorValueRef = _cardColor;

        cardNetworkDataInstance.GetComponent<NetworkObject>().SpawnWithOwnership(ownerId);    
    }

    private void ChangeCardType()
    {
        CardType cardTypeServerRef = GetRandomType();
        ChangeCardTypeRpc(cardTypeServerRef);
    }

    private CardType GetRandomType()
    {
        CardType[] cardtypes = (CardType[])Enum.GetValues(typeof(CardType));
        int randomIndex = UnityEngine.Random.Range(0, cardtypes.Length);
        _cardType = cardtypes[randomIndex];
        return _cardType;
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
            _ => throw new Exception("Unvalid card type")
        };
    }
}
