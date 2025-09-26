using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaseCardData : MonoBehaviour
{
    [SerializeField, ReadOnly] public Image CardTypeFront, CardTypeBack;
    [SerializeField, ReadOnly] public Image Frame;
    [SerializeField, ReadOnly] public Image ReverseBg;
    [SerializeField, ReadOnly] public TextMeshProUGUI PositiveEffectText, NegativeEffectText;
    [SerializeField, ReadOnly] public TextMeshProUGUI  CardNumberUpText, CardNumberDownText;
}
