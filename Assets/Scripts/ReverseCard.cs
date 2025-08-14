using UnityEngine;
using UnityEngine.UI;

public class ReverseCard : MonoBehaviour
{
    [SerializeField] private Image _cardTypeImage;
    public void Initialize(CardType_Color.CardColor color, Sprite reverseSprite)
    {
        GetComponentInChildren<Image>().color = color switch
        {
            CardType_Color.CardColor.Green => Color.green,
            CardType_Color.CardColor.Orange => Color.yellow,
            CardType_Color.CardColor.Red => Color.red,
            _ => throw new System.Exception("Unvalid card color")
        };

        _cardTypeImage.sprite = reverseSprite;
    }
}
