using UnityEngine;
using Unity.Netcode;

public abstract class BaseCard : MonoBehaviour, IClickable
{ 
    private GameObject CardNetworkDataInstance { get; set; }
    
    protected abstract void OnInitialize();
    protected abstract void PositiveEffect();
    protected abstract void NegativeEffect();
    
    protected CardType_Color.CardColor CardColor;
    
    protected BaseCardData CardData;
    private void OnEnable()
    {
        CardData = GetComponent<BaseCardData>();
    }

    public void Initialize(CardType_Color.CardColor color, CardType_Color.CardType type, int seat, GameObject cardNetworkData)
    {
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

        OnInitialize();
    }

    public bool CardUsed { get; set;}

    public void OnClick()
    {
        NetworkObject netObj = CardNetworkDataInstance.GetComponent<NetworkObject>();

        if (netObj != null && netObj.IsOwner && !CardUsed) //Prevents clients from using other players cards 
        {
            UseCard();
        }
    }

    private void UseCard()
    {
        CardNetworkDataInstance.GetComponent<CardNetworkData>().ChangeCardHandServerRpc();
        NetworkHandler.Instance.EndTurnRpc();
    }
}
