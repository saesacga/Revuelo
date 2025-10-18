using System;
using Unity.Netcode;
using UnityEngine;

public class BaseAtkCardV1 : BaseCard
{
    private enum PositiveEffectV1 { Steal, Discard }
    private enum NegativeEffectV1 { NothingHappens, UDiscard, Gift  }
    //private enum Target { AnyPlayer, RightPlayer, LeftPlayer }
    
    private readonly NetworkVariable<PositiveEffectV1> _positiveEffect = new NetworkVariable<PositiveEffectV1>();
    private readonly NetworkVariable<NegativeEffectV1> _negativeEffect = new NetworkVariable<NegativeEffectV1>();
    //private readonly NetworkVariable<Target> _target = new NetworkVariable<Target>();
    
    private readonly NetworkVariable<int> _positiveQuantity = new NetworkVariable<int>();
    private readonly NetworkVariable<int> _negativeQuantity = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (IsServer) SetCardDataServerRpc();
    }
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SetCardDataServerRpc()
    {
        _positiveEffect.Value = CardColor switch
        { 
            CardType_Color.CardColor.Green => PositiveEffectV1.Discard, 
            CardType_Color.CardColor.Orange => (PositiveEffectV1)UnityEngine.Random.Range(0, Enum.GetValues(typeof(PositiveEffectV1)).Length), 
            CardType_Color.CardColor.Red => PositiveEffectV1.Steal, 
            _ => throw new Exception("Invalid card color")
        }; 
        _negativeEffect.Value = CardColor switch 
        { 
            CardType_Color.CardColor.Green => NegativeEffectV1.UDiscard, 
            CardType_Color.CardColor.Orange => (NegativeEffectV1)UnityEngine.Random.Range(1, 2), 
            CardType_Color.CardColor.Red => NegativeEffectV1.Gift, 
            _ => throw new Exception("Invalid card color") 
        };
        
        _positiveQuantity.Value = (CardColor, _positiveEffect.Value) switch
        {
            (CardType_Color.CardColor.Green, PositiveEffectV1.Discard) => UnityEngine.Random.Range(1, 3),
            (CardType_Color.CardColor.Orange, PositiveEffectV1.Discard) => UnityEngine.Random.Range(2, 4),
            (CardType_Color.CardColor.Orange, PositiveEffectV1.Steal) => UnityEngine.Random.Range(1, 3),
            (CardType_Color.CardColor.Red, PositiveEffectV1.Steal) => UnityEngine.Random.Range(3, 5),

            _ => throw new Exception("Invalid card color")
        };
        
        _negativeQuantity.Value = _positiveQuantity.Value - 1;
        if (_negativeQuantity.Value < 1) _negativeEffect.Value = NegativeEffectV1.NothingHappens;
        
        //Set the number to get on the dice
        CardNumberToGet.Value = CardColor switch 
        { 
            CardType_Color.CardColor.Green => UnityEngine.Random.Range(7, 10), 
            CardType_Color.CardColor.Orange => UnityEngine.Random.Range(9, 12), 
            CardType_Color.CardColor.Red => UnityEngine.Random.Range(13, 15), 
            _ => throw new Exception("Invalid card color")
        };
        
        DisplayCardDataRpc();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void DisplayCardDataRpc()
    {
        CardData.PositiveEffectText.text = _positiveEffect.Value switch
        {
            PositiveEffectV1.Discard=> $"Choose another player to discard {_positiveQuantity.Value} {(_positiveQuantity.Value == 1 ? "card" : "cards")}",
            PositiveEffectV1.Steal => $"Steal {_positiveQuantity.Value} {(_positiveQuantity.Value == 1 ? "card" : "cards")} from another player",
            _ => throw new Exception("Invalid card effect")
        };

        CardData.NegativeEffectText.text = _negativeEffect.Value switch
        {
            NegativeEffectV1.NothingHappens => "Nothing happens",
            NegativeEffectV1.UDiscard => $"Discard {_negativeQuantity.Value} {(_negativeQuantity.Value == 1 ? "card" : "cards")}",
            NegativeEffectV1.Gift => $"Give another player {_negativeQuantity.Value} {(_negativeQuantity.Value == 1 ? "card" : "cards")}",
            _ => throw new Exception("Invalid card effect")
        };
        
        CardData.CardNumberUpText.text = $"{CardNumberToGet.Value}";
        CardData.CardNumberDownText.text = $"{CardNumberToGet.Value-1}";
    }

    protected override void PositiveEffect()
    {
        switch (_positiveEffect.Value) 
        { 
            case PositiveEffectV1.Discard:
                NetworkHandler.Instance.EndTurnServerRpc();
                break;
            case PositiveEffectV1.Steal:
                CardHandler.Instance.StealRpc(_positiveQuantity.Value);
                break;
            default: 
                throw new ArgumentOutOfRangeException();
        }
    }
    
    protected override void NegativeEffect() 
    { 
        NetworkHandler.Instance.EndTurnServerRpc();
    }
    
    private float Probability()
    {
        var probability = (20 - CardNumberToGet.Value + 1f) / 20;
        return probability;
    }
}
