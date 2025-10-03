using System;
using UnityEngine;

public class BaseAtkCardV1 : BaseCard
{
    private int _numberToGet;
    
    private enum PositiveAtkV1 { Steal, Discard }
    private enum NegativeAtkV1 { NothingHappens, UDiscard, Gift, GetSteal,  }
    private enum Target { AnyPlayer, RightPlayer, LeftPlayer }
    
    private PositiveAtkV1 _positiveAtk;
    private NegativeAtkV1 _negativeAtk;
    private Target _target;

    protected override void OnInitialize()
    {
        _positiveAtk = CardColor switch
        {
            CardType_Color.CardColor.Green => PositiveAtkV1.Discard,
            CardType_Color.CardColor.Orange => (PositiveAtkV1)UnityEngine.Random.Range(0, Enum.GetValues(typeof(PositiveAtkV1)).Length),
            CardType_Color.CardColor.Red => PositiveAtkV1.Steal,
            _ => throw new Exception("Invalid card color")
        };
        _negativeAtk = CardColor switch
        {
            CardType_Color.CardColor.Green => (NegativeAtkV1)UnityEngine.Random.Range(0, 1),
            CardType_Color.CardColor.Orange => (NegativeAtkV1)UnityEngine.Random.Range(1, 2),
            CardType_Color.CardColor.Red => (NegativeAtkV1)UnityEngine.Random.Range(2, 3),
            _ => throw new Exception("Invalid card color")
        };
        
        //Set effect and target text
        PositiveQuantity = UnityEngine.Random.Range(2, 5);
        NegativeQuantity = UnityEngine.Random.Range(1, 3);
        CardData.PositiveEffectText.text = ($"{_positiveAtk} {PositiveQuantity} cards from {_target}");
        CardData.NegativeEffectText.text = $"{_negativeAtk} {NegativeQuantity} card to {_target}";
        //Set the number to get on the dice
        CardNumber = UnityEngine.Random.Range(2, 20);
        CardData.CardNumberUpText.text = $"{CardNumber}";
        CardData.CardNumberDownText.text = $"{CardNumber-1}";
        
        Debug.Log($"The number to get is: {CardNumber} or more");
    }
    
    protected override void PositiveEffect()
    {
        switch (_positiveAtk)
        {
            case PositiveAtkV1.Discard:
                NetworkHandler.Instance.EndTurnRpc();
                break;
            case PositiveAtkV1.Steal:
                Stealing = true;
                StealQuantity = PositiveQuantity;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void NegativeEffect()
    {
        NetworkHandler.Instance.EndTurnRpc();
    }
}
