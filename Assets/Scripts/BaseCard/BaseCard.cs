using UnityEngine;
using Unity.Netcode;

public abstract class BaseCard : MonoBehaviour, IClickable
{ 
    protected CardNetworkData CardNetworkDataInstance { get; private set; }
    protected NetworkObject NetObj { get; private set; }
    
    protected BaseCardData CardData;
    
    protected CardType_Color.CardColor CardColor;
    protected int CardNumber;
    protected int PositiveQuantity;
    protected int NegativeQuantity;

    protected abstract void OnInitialize();
    protected abstract void PositiveEffect();
    protected abstract void NegativeEffect();
    
    public void Initialize(CardType_Color.CardColor color, CardType_Color.CardType type, int seat, CardNetworkData cardNetworkData)
    {
        CardData = GetComponent<BaseCardData>();
        
        CardColor = color;
        
        CardData.CardTypeFront.sprite = type switch
        {
            CardType_Color.CardType.Attack => CardHandler.Instance.TypeImages[3],
            CardType_Color.CardType.Defense => CardHandler.Instance.TypeImages[4],
            CardType_Color.CardType.Recruit => CardHandler.Instance.TypeImages[5],
            _ => throw new System.Exception("Invalid card type")
        };
        CardData.CardTypeBack.sprite = type switch
        {
            CardType_Color.CardType.Attack => CardHandler.Instance.TypeImages[0],
            CardType_Color.CardType.Defense => CardHandler.Instance.TypeImages[1],
            CardType_Color.CardType.Recruit => CardHandler.Instance.TypeImages[2],
            _ => throw new System.Exception("Invalid card type")
        };
        
        var cardColor = color switch
        {
            CardType_Color.CardColor.Green => Color.green,
            CardType_Color.CardColor.Orange => Color.yellow,
            CardType_Color.CardColor.Red => Color.red,
            _ => throw new System.Exception("Invalid card color")
        };
        
        CardData.Frame.color = cardColor;
        CardData.ReverseBg.color = cardColor;

        transform.SetParent(CardHandler.Instance.SeatGrids[seat]); //Set new card parent to local player grid
        int lastIndex = CardHandler.Instance.SeatGrids[seat].childCount - 2;
        transform.SetSiblingIndex(lastIndex); //Change the position in hierarchy so cards instantiate in the middle of the hand
        
        CardNetworkDataInstance = cardNetworkData;
        NetObj = CardNetworkDataInstance.GetComponent<NetworkObject>();
        
        OnInitialize();
    }
    
    public static bool UsedCard { get; set; }

    public virtual void OnClick()
    {
        if (NetObj == null) return;

        if (NetObj.IsOwner && CardType_Color.CardPicked && !CardNetworkDataInstance.CardDiscarded.Value && !UsedCard) //Prevents clients from using other players cards 
        {
            UseCard();
        }
        
        StealCards();
    }

    private void UseCard()
    {
        DiceRoller.Instance.OnDiceRolled += CardEffect;
        CardNetworkDataInstance.ChangeCardHandServerRpc();
        DiceRoller.Instance.RollDiceServerRpc();
        
        CardNetworkDataInstance.CardDiscardedServerRpc(true);
        UsedCard = true;
    }

    private static int Count { get; set; }
    protected static bool Stealing { get; set; }
    protected static int StealQuantity { get; set; }
    private void StealCards()
    {
        if (!Stealing || NetObj.IsOwner || CardNetworkDataInstance.CardDiscarded.Value) return; //Stealing
        CardNetworkDataInstance.ChangeCardHandServerRpc();
        Count++;
        if (StealQuantity > Count) return;
        Stealing = false; Count = 0; NetworkHandler.Instance.EndTurnRpc();
    }
    
    private void CardEffect(int diceValue)
    {
        if (diceValue >= CardNumber)
        {
            PositiveEffect();
        }
        else
        {
            NegativeEffect();
        }
        DiceRoller.Instance.OnDiceRolled -= CardEffect;
    }
}
