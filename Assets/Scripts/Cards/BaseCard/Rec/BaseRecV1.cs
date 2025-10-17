
using Unity.Netcode;

public class BaseRecV1 : BaseCard
{
    protected override void PositiveEffect()
    {
        NetworkHandler.Instance.EndTurnServerRpc();
    }

    protected override void NegativeEffect()
    {
        NetworkHandler.Instance.EndTurnServerRpc();
    }
}
