using System;

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
        
        CardData.PositiveEffectText.text = ($"{_positiveAtk} {UnityEngine.Random.Range(2, 5)} cards from {_target}");
        CardData.NegativeEffectText.text = $"{_negativeAtk} {UnityEngine.Random.Range(2, 5)} card to {_target}";
    }

    protected override void PositiveEffect()
    {
        throw new NotImplementedException();
    }

    protected override void NegativeEffect()
    {
        throw new NotImplementedException();
    }
}
