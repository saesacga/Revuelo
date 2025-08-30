using Unity.Netcode;
using UnityEngine;

public class CardNetworkData : NetworkBehaviour
{
    private GameObject _visualCardPrefabRef;

    public CardType_Color.CardColor CardColorValueRef { get; set; }
    public CardType_Color.CardType CardTypeValueRef { get; set; }
    [SerializeField, ReadOnly] private NetworkVariable<CardType_Color.CardType> _cardType = new NetworkVariable<CardType_Color.CardType>();
    [SerializeField, ReadOnly] private NetworkVariable<CardType_Color.CardColor> _cardColor = new NetworkVariable<CardType_Color.CardColor>();

    public override void OnNetworkSpawn()
    {
        SetCardDataRpc();

        NetworkList<ulong> players = NetworkHandler.Instance.ClientIds;
        ulong myId = NetworkManager.Singleton.LocalClientId;

        int myIndex = players.IndexOf(myId);
        int ownerIndex = players.IndexOf(GetComponent<NetworkObject>().OwnerClientId);
        int count = players.Count;

        int seatIndex = (ownerIndex - myIndex + count) % count;

        var newCard = _cardType.Value switch //Check which card type to use
        {
            CardType_Color.CardType.Attack => CardHandler.Instance.BaseAtkPrefabs[0],
            CardType_Color.CardType.Defense => CardHandler.Instance.BaseDefPrefabs[0],
            CardType_Color.CardType.Recruit => CardHandler.Instance.BaseRecPrefabs[0],
            _ => throw new System.Exception("Unvalid cardprefab")
        };

        GameObject cardInstance = Instantiate(newCard); //Create new card
        cardInstance.transform.SetParent(CardHandler.Instance.SeatGrids[seatIndex]); //Set new card parent to local player grid
        int lastIndex = CardHandler.Instance.SeatGrids[seatIndex].childCount - 2;
        cardInstance.transform.SetSiblingIndex(lastIndex); //Change the position in hierarchy so cards instantiate in the middle of the hand

        cardInstance.GetComponent<BaseCard>().Initialize(_cardColor.Value);

        cardInstance.GetComponent<BaseCard>().CardNetworkDataInstance = gameObject;

        _visualCardPrefabRef = cardInstance;
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SetCardDataRpc()
    {
        _cardType.Value = CardTypeValueRef;
        _cardColor.Value = CardColorValueRef;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UseNetworkCardRpc()
    {
        if (_visualCardPrefabRef == null) return;
        _visualCardPrefabRef.transform.SetParent(CardHandler.Instance.DiscardPile);
    }
}
