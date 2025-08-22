using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Deck : NetworkBehaviour
{
    #region Properties and Fields

    [SerializeField] private Transform[] _seatGrids;

    [field: SerializeField] public GameObject[] BaseAtkPrefabs { get; private set; }
    [field: SerializeField] public GameObject[] BaseDefPrefabs { get; private set; }
    [field: SerializeField] public GameObject[] BaseRecPrefabs { get; private set; }

    [field: SerializeField] public Sprite[] TypeImages { get; private set; }

    private GameObject _newCard;

    #endregion

    private void OnEnable()
    {
        CardType_Color.OnDeckButtonPressed += SpawnCardServerRpc;
    }
    private void OnDisable()
    {
        CardType_Color.OnDeckButtonPressed -= SpawnCardServerRpc;
    }
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SpawnCardServerRpc(CardType_Color.CardColor color, CardType_Color.CardType type, RpcParams rpcParams)
    {
        if (!IsServer) return;

        ulong ownerId = rpcParams.Receive.SenderClientId;  
      
        SpawnCardClientRpc(ownerId, type, color);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnCardClientRpc(ulong ownerId, CardType_Color.CardType type, CardType_Color.CardColor color)
    {
        NetworkList<ulong> players = NetworkHandler.Instance.ClientIds;
        ulong myId = NetworkManager.Singleton.LocalClientId;

        int myIndex = players.IndexOf(myId);
        int ownerIndex = players.IndexOf(ownerId);
        int count = players.Count;

        int seatIndex = (ownerIndex - myIndex + count) % count;

        _newCard = type switch //Check which card type to use
        {
            CardType_Color.CardType.Attack => BaseAtkPrefabs[0],
            CardType_Color.CardType.Defense => BaseDefPrefabs[0],
            CardType_Color.CardType.Recruit => BaseRecPrefabs[0],
            _ => throw new System.Exception("Unvalid cardprefab")
        };
        
        GameObject cardInstance = Instantiate(_newCard, transform.GetChild(0)); //Create new card
        cardInstance.transform.SetParent(_seatGrids[seatIndex]); //Set new card parent to local player grid
        int lastIndex = _seatGrids[seatIndex].childCount - 2;
        cardInstance.transform.SetSiblingIndex(lastIndex); //Change the position in hierarchy so cards instantiate in the middle of the hand
        cardInstance.GetComponent<BaseCard>().Initialize(color);
    }
}
