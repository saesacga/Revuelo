using Unity.Netcode;

public class BaseDefV1 : BaseCard
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