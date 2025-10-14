using System;
using Unity.VisualScripting;
using UnityEngine;

public class BaseCard : CardNetwork
{
    private BaseCardData _cardData;
    public int CardNumberToGet { get; set; }
    
    protected override void OnInitialize()
    {
        _cardData = GetComponent<BaseCardData>();
        
        _cardData.CardTypeFront.sprite = CardType switch 
        {
            CardType_Color.CardType.Attack => CardHandler.Instance.TypeImages[3],
            CardType_Color.CardType.Defense => CardHandler.Instance.TypeImages[4], 
            CardType_Color.CardType.Recruit => CardHandler.Instance.TypeImages[5],
            _ => throw new System.Exception("Invalid card type") 
        };
        _cardData.CardTypeBack.sprite = CardType switch 
        { 
            CardType_Color.CardType.Attack => CardHandler.Instance.TypeImages[0], 
            CardType_Color.CardType.Defense => CardHandler.Instance.TypeImages[1], 
            CardType_Color.CardType.Recruit => CardHandler.Instance.TypeImages[2], 
            _ => throw new System.Exception("Invalid card type") 
        };
        
        var cardColor = CardColor switch
        { 
            CardType_Color.CardColor.Green => Color.green,
            CardType_Color.CardColor.Orange => Color.yellow,
            CardType_Color.CardColor.Red => Color.red,
            _ => throw new System.Exception("Invalid card color") 
        };
        
        _cardData.Frame.color = cardColor; 
        _cardData.ReverseBg.color = cardColor;

        switch (CardType)
        { 
            case CardType_Color.CardType.Attack:
                transform.AddComponent<BaseAtkCardV1>();
                break; 
            case CardType_Color.CardType.Defense:
                transform.AddComponent<BaseDefV1>(); 
                break;
            case CardType_Color.CardType.Recruit:
                transform.AddComponent<BaseRecV1>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    protected override void UseCardRpc()
    {
        base.UseCardRpc();
        
        if (!IsOwner) return;
        
        DiceRoller.Instance.OnDiceRolled += CardEffect; 
        DiceRoller.Instance.RollDiceServerRpc();
    }

    private void CardEffect(int diceValue)
    {
        if (diceValue >= CardNumberToGet)
        {
            GetComponent<IPlayable>().PositiveEffect();
        }
        else
        {
            GetComponent<IPlayable>().NegativeEffect();
        }
        DiceRoller.Instance.OnDiceRolled -= CardEffect;
    } 
}
