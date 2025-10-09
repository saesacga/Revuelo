using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class BaseCardData : MonoBehaviour
{
    #region Base Card Data
    
    [FoldoutGroup("Base Card Data"), DisableIf("@CardTypeFront != null")]
    public Image CardTypeFront;
    
    [FoldoutGroup("Base Card Data"), DisableIf("@CardTypeBack != null")]
    public Image CardTypeBack;
    
    [FoldoutGroup("Base Card Data"), DisableIf("@Frame != null")]
    public Image Frame; 
    
    [FoldoutGroup("Base Card Data"), DisableIf("@ReverseBg != null")]
    public Image ReverseBg; 
    
    [FoldoutGroup("Base Card Data"), DisableIf("@PositiveEffectText != null")]
    public TextMeshProUGUI PositiveEffectText; 
    
    [FoldoutGroup("Base Card Data"), DisableIf("@NegativeEffectText != null")]
    public TextMeshProUGUI NegativeEffectText;
    
    [FoldoutGroup("Base Card Data"), DisableIf("@CardNumberUpText != null")]
    public TextMeshProUGUI  CardNumberUpText;
    
    [FoldoutGroup("Base Card Data"), DisableIf("@CardNumberDownText != null")]
    public TextMeshProUGUI  CardNumberDownText;

    #endregion
}
