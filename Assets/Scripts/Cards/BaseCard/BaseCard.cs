using Unity.Netcode;
using UnityEngine;

public abstract class BaseCard : CardNetwork
{
    protected BaseCardData CardData;
    protected readonly NetworkVariable<int> CardNumberToGet = new NetworkVariable<int>();
    
    protected abstract void PositiveEffect();
    protected abstract void NegativeEffect();
    
    protected override void OnInitialize()
    {
        CardData = GetComponent<BaseCardData>();
        
        SetCardVisuals();
    }

    private void SetCardVisuals()
    {
        var cardColor = CardColor switch
        { 
            CardType_Color.CardColor.Green => Color.green,
            CardType_Color.CardColor.Orange => Color.yellow,
            CardType_Color.CardColor.Red => Color.red,
            _ => throw new System.Exception("Invalid card color") 
        };
        
        CardData.Frame.color = cardColor; 
        CardData.ReverseBg.color = cardColor;
    }
    
    protected override void CardEffect()
    {
        RouletteRoller.Instance.OnRouletteSpinned += CardEffect; 
        RouletteRoller.Instance.SpinRouletteServerRpc();
    }

    private void CardEffect(int diceValue)
    {
        if (diceValue >= CardNumberToGet.Value)
        {
            PositiveEffect();
        }
        else
        {
            NegativeEffect();
        }
        RouletteRoller.Instance.OnRouletteSpinned -= CardEffect;
    } 
}
