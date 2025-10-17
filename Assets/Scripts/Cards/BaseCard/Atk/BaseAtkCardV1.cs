using System;
using Unity.Netcode;
using UnityEngine;

public class BaseAtkCardV1 : BaseCard
{
    private enum PositiveAtkV1 { Steal, Discard }
    private enum NegativeAtkV1 { NothingHappens, UDiscard, Gift, GetSteal,  }
    private enum Target { AnyPlayer, RightPlayer, LeftPlayer }
    
    private readonly NetworkVariable<PositiveAtkV1> _positiveAtk = new NetworkVariable<PositiveAtkV1>();
    private readonly NetworkVariable<NegativeAtkV1> _negativeAtk = new NetworkVariable<NegativeAtkV1>();
    private readonly NetworkVariable<Target> _target = new NetworkVariable<Target>();
    
    private readonly NetworkVariable<int> _positiveQuantity = new NetworkVariable<int>();
    private readonly NetworkVariable<int> _negativeQuantity = new NetworkVariable<int>();
    
    public override void OnNetworkSpawn()
    {
        Debug.Log("Attack Spawned 2.0");
        
        base.OnNetworkSpawn();
        SetCardDataServerRpc();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SetCardDataServerRpc()
    {
        _positiveAtk.Value = CardColor switch
        { 
            CardType_Color.CardColor.Green => PositiveAtkV1.Discard, 
            CardType_Color.CardColor.Orange => (PositiveAtkV1)UnityEngine.Random.Range(0, Enum.GetValues(typeof(PositiveAtkV1)).Length), 
            CardType_Color.CardColor.Red => PositiveAtkV1.Steal, 
            _ => throw new Exception("Invalid card color")
        }; 
        _negativeAtk.Value = CardColor switch 
        { 
            CardType_Color.CardColor.Green => (NegativeAtkV1)UnityEngine.Random.Range(0, 1), 
            CardType_Color.CardColor.Orange => (NegativeAtkV1)UnityEngine.Random.Range(1, 2), 
            CardType_Color.CardColor.Red => (NegativeAtkV1)UnityEngine.Random.Range(2, 3), 
            _ => throw new Exception("Invalid card color") 
        };
        
        //Set effect and target text
        _positiveQuantity.Value = UnityEngine.Random.Range(2, 5); 
        _negativeQuantity.Value = UnityEngine.Random.Range(1, 3); 
        
        //Set the number to get on the dice
        CardNumberToGet.Value = UnityEngine.Random.Range(2, 20);
        
        DisplayCardDataRpc();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void DisplayCardDataRpc()
    {
        CardData.PositiveEffectText.text = ($"{_positiveAtk} {_positiveQuantity} cards from {_target}");
        CardData.NegativeEffectText.text = $"{_negativeAtk} {_negativeQuantity} card to {_target}";
        
        CardData.CardNumberUpText.text = $"{CardNumberToGet.Value}";
        CardData.CardNumberDownText.text = $"{CardNumberToGet.Value-1}";
    }

    protected override void PositiveEffect()
    {
        switch (_positiveAtk.Value) 
        { 
            case PositiveAtkV1.Discard:
                NetworkHandler.Instance.EndTurnServerRpc();
                break;
            case PositiveAtkV1.Steal:
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
}
