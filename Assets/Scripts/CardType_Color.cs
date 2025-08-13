using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.Netcode;

[RequireComponent(typeof(Button))]
public class CardType_Color : NetworkBehaviour
{
    public enum CardType { Attack, Recruit, Defense }
    public enum CardColor { Green, Orange, Red }

    [SerializeField] private CardColor _cardColor;
    private CardType _cardType;
    public static event Action<CardColor, CardType> OnDeckButtonPressed;
    private Button _button;
    private Deck _deck;
    private Image _reverseType;

    private void Awake()
    {
        _deck = GetComponentInParent<Deck>();

        _reverseType = transform.GetComponentsInChildren<Image>().FirstOrDefault(img => img.gameObject != this.gameObject);

        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => { OnDeckButtonPressed?.Invoke(_cardColor, _cardType); ChangeCardType(); });
    }

    private void Start()
    {
        ChangeCardType();
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
            CardType.Attack => _deck.TypeImages[0],
            CardType.Defense => _deck.TypeImages[1],
            CardType.Recruit => _deck.TypeImages[2],
            _ => throw new Exception("Unvalid card type")
        };
    }
}
