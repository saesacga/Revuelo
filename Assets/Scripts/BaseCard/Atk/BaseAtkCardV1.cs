using UnityEngine;
using UnityEngine.UI;

public class BaseAtkCardV1 : BaseCard
{
    private enum AtkV1
    {
        Steal,
        Discard
    }

    protected override void OnInitialize()
    {
        
    }

    protected override void PositiveEffect()
    {
        throw new System.NotImplementedException();
    }

    protected override void NegativeEffect()
    {
        throw new System.NotImplementedException();
    }
}
