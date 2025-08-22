using UnityEngine;
using UnityEngine.UI;

public abstract class BaseCard : MonoBehaviour
{
    private int _numberToGet;

    [SerializeField] private Image _frame;
    [SerializeField] private Image _reverseBG;

    protected abstract void OnInitialize();
    protected abstract void PositiveEffect();
    protected abstract void NegativeEffect();

    public void Initialize(CardType_Color.CardColor color)
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

        OnInitialize();
    }
}
