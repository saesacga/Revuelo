using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BaseCard : CardNetwork
{
    private BaseCardData _cardData;
    public int CardNumberToGet { get; set; }
    
    protected override void OnInitialize()
    {
        SetCardVisuals();
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

    private void SetCardVisuals()
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
    }

    [Rpc(SendTo.ClientsAndHost)]
    protected override void CardEffectRpc()
    {
        if (!IsOwner) return;
        
        RouletteRoller.Instance.OnRouletteSpinned += CardEffect; 
        RouletteRoller.Instance.SpinRouletteServerRpc();
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
        RouletteRoller.Instance.OnRouletteSpinned -= CardEffect;
    } 
}
