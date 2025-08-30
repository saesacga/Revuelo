using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class CardType_Color : NetworkBehaviour, IClickable
{
    public enum CardType { Attack, Recruit, Defense }
    public enum CardColor { Green, Orange, Red }

    [field: SerializeField] [field: ReadOnly] public CardColor LocalColor { get; private set; }
    [HideInInspector] public NetworkVariable<CardType> Type = new NetworkVariable<CardType>();

    [SerializeField] [field: ReadOnly] private Image _reverseType;

    public override void OnNetworkSpawn()
    {
        GetRandomTypeRpc();
    }

    public void OnClick()
    {
        DeckPressed();
    }

    public void DeckPressed()
    {
        SpawnCardServerRpc();
        GetRandomTypeRpc();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SpawnCardServerRpc(RpcParams rpcParams = default)
    {
        if (!IsServer) return;

        ulong ownerId = rpcParams.Receive.SenderClientId;

        GameObject cardNetworkDataInstance = Instantiate(CardHandler.Instance.CardNetworkDataPrefab);

        cardNetworkDataInstance.GetComponent<CardNetworkData>().CardTypeValueRef = Type.Value;
        cardNetworkDataInstance.GetComponent<CardNetworkData>().CardColorValueRef = LocalColor;

        cardNetworkDataInstance.GetComponent<NetworkObject>().SpawnWithOwnership(ownerId);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void GetRandomTypeRpc()
    {
        CardType[] cardtypes = (CardType[])Enum.GetValues(typeof(CardType));
        int randomIndex = UnityEngine.Random.Range(0, cardtypes.Length);
        Type.Value = cardtypes[randomIndex];
    
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
            _ => throw new Exception("Unvalid card type")
        };
    }
}
