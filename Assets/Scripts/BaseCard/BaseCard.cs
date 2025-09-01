using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public abstract class BaseCard : MonoBehaviour, IClickable
{
    private int _numberToGet;

    public GameObject CardNetworkDataInstance { get; set; }

    [SerializeField, ReadOnly] private Image _frame;
    [SerializeField, ReadOnly] private Image _reverseBG;

    protected abstract void OnInitialize();
    protected abstract void PositiveEffect();
    protected abstract void NegativeEffect();

    public void Initialize(CardType_Color.CardColor color, int seat, GameObject cardNetworkData)
    {
        var cardColor = color switch
        {
            CardType_Color.CardColor.Green => Color.green,
            CardType_Color.CardColor.Orange => Color.yellow,
            CardType_Color.CardColor.Red => Color.red,
            _ => throw new System.Exception("Unvalid card color")
        };
        _frame.color = cardColor;
        _reverseBG.color = cardColor;

        transform.SetParent(CardHandler.Instance.SeatGrids[seat]); //Set new card parent to local player grid
        int lastIndex = CardHandler.Instance.SeatGrids[seat].childCount - 2;
        transform.SetSiblingIndex(lastIndex); //Change the position in hierarchy so cards instantiate in the middle of the hand

        CardNetworkDataInstance = cardNetworkData;

        OnInitialize();
    }

    public void OnClick()
    {
        NetworkObject netObj = CardNetworkDataInstance.GetComponent<NetworkObject>();

        if (netObj != null && netObj.IsOwner) //Prevents clients from using other players cards 
        {
            UseCard();
        }
    }

    public void UseCard()
    {
        CardNetworkDataInstance.GetComponent<CardNetworkData>().UseNetworkCardRpc();
        NetworkHandler.Instance.EndTurnRpc();
    }
}
