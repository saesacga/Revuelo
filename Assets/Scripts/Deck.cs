using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Deck : NetworkBehaviour
{
    #region Properties and Fields

    [SerializeField] private Transform _handGrid;

    [SerializeField] private GameObject _reverseCardPrefab;

    [field: SerializeField] public Transform[] EnemiesGrid { get; private set; }
   
    [field: SerializeField] public GameObject[] BaseAtkPrefabs { get; private set; }
    [field: SerializeField] public GameObject[] BaseDefPrefabs { get; private set; }
    [field: SerializeField] public GameObject[] BaseRecPrefabs { get; private set; }

    [field: SerializeField] public Sprite[] TypeImages { get; private set; }

    private GameObject _newCard;

    #endregion
    private void OnEnable()
    {
        CardType_Color.OnDeckButtonPressed += ChangeGameState;
    }
    private void OnDisable()
    {
        CardType_Color.OnDeckButtonPressed -= ChangeGameState;
    }

    public void ChangeGameState(CardType_Color.CardColor color, CardType_Color.CardType type)
    {
        CreateCard(color, type);
        CreateEnemyCardRpc(color, type);
    }

    //Create cards for main player
    private void CreateCard(CardType_Color.CardColor color, CardType_Color.CardType type)
    {
        _newCard = type switch
        {
            CardType_Color.CardType.Attack => BaseAtkPrefabs[0],
            CardType_Color.CardType.Defense => BaseDefPrefabs[0],
            CardType_Color.CardType.Recruit => BaseRecPrefabs[0],
            _ => throw new System.Exception("Unvalid cardprefab")
        };

        GameObject cardInstance = Instantiate(_newCard, _handGrid);
        cardInstance.GetComponent<BaseCard>().Initialize(color);
    }

    //Create reverse cards for other players
    [Rpc(SendTo.ClientsAndHost)]
    private void CreateEnemyCardRpc(CardType_Color.CardColor color, CardType_Color.CardType type)
    {
        GameObject enemyCardInstance = Instantiate(_reverseCardPrefab, EnemiesGrid[0]);

        Sprite reverseSprite = type switch
        { 
            CardType_Color.CardType.Attack => TypeImages[0],
            CardType_Color.CardType.Defense => TypeImages[1],
            CardType_Color.CardType.Recruit => TypeImages[2],
            _ => throw new Exception("Unvalid card type")
        };

        enemyCardInstance.GetComponent<ReverseCard>().Initialize(color, reverseSprite);
    }
}
