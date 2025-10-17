using System;
using Unity.Netcode;
using UnityEngine;

public class BaseAtkCardV1 : NetworkBehaviour, IPlayable
{
    private int _negativeQuantity;
    private int _positiveQuantity; 
    
    private enum PositiveAtkV1 { Steal, Discard }
    private enum NegativeAtkV1 { NothingHappens, UDiscard, Gift, GetSteal,  }
    private enum Target { AnyPlayer, RightPlayer, LeftPlayer }
    private PositiveAtkV1 _positiveAtk;
    private NegativeAtkV1 _negativeAtk;
    private Target _target;
    
    private CardEffects _cardEffects;
    
    private void OnEnable()
    {
        var baseCard = GetComponent<BaseCard>();
        var cardData = GetComponent<BaseCardData>();
        
        _positiveAtk = baseCard.CardColor switch
        { 
            CardType_Color.CardColor.Green => PositiveAtkV1.Discard, 
            CardType_Color.CardColor.Orange => (PositiveAtkV1)UnityEngine.Random.Range(0, Enum.GetValues(typeof(PositiveAtkV1)).Length), 
            CardType_Color.CardColor.Red => PositiveAtkV1.Steal, 
            _ => throw new Exception("Invalid card color")
        }; 
        _negativeAtk = baseCard.CardColor switch 
        { 
            CardType_Color.CardColor.Green => (NegativeAtkV1)UnityEngine.Random.Range(0, 1), 
            CardType_Color.CardColor.Orange => (NegativeAtkV1)UnityEngine.Random.Range(1, 2), 
            CardType_Color.CardColor.Red => (NegativeAtkV1)UnityEngine.Random.Range(2, 3), 
            _ => throw new Exception("Invalid card color") 
        };
        
        //Set effect and target text
        _positiveQuantity = UnityEngine.Random.Range(2, 5); 
        _negativeQuantity = UnityEngine.Random.Range(1, 3); 
        cardData.PositiveEffectText.text = ($"{_positiveAtk} {_positiveQuantity} cards from {_target}"); 
        cardData.NegativeEffectText.text = $"{_negativeAtk} {_negativeQuantity} card to {_target}"; 
        
        //Set the number to get on the dice
        baseCard.CardNumberToGet = UnityEngine.Random.Range(2, 20);
        cardData.CardNumberUpText.text = $"{baseCard.CardNumberToGet}";
        cardData.CardNumberDownText.text = $"{baseCard.CardNumberToGet-1}";
    } 
    
    public void PositiveEffect()
    {
        switch (_positiveAtk) 
        { 
            case PositiveAtkV1.Discard:
                NetworkHandler.Instance.EndTurnServerRpc();
                break;
            case PositiveAtkV1.Steal:
                CardHandler.Instance.StealServerRpc(_positiveQuantity);
                break;
            default: 
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void NegativeEffect() 
    { 
        NetworkHandler.Instance.EndTurnServerRpc();
    }
}
