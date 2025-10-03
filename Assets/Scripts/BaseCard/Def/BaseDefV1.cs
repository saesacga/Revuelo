using UnityEngine;
using UnityEngine.UI;

public class BaseDefV1 : BaseCard
{
    protected override void OnInitialize()
    {
        
    }

    protected override void PositiveEffect()
    {
        NetworkHandler.Instance.EndTurnRpc();
    }

    protected override void NegativeEffect()
    {
        NetworkHandler.Instance.EndTurnRpc();
    }
}
