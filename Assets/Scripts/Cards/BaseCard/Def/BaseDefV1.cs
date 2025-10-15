using Unity.Netcode;

public class BaseDefV1 : NetworkBehaviour, IPlayable
{
    public void PositiveEffect()
    {
        NetworkHandler.Instance.EndTurnServerRpc();
    }

    public void NegativeEffect()
    {
        NetworkHandler.Instance.EndTurnServerRpc();
    }
}