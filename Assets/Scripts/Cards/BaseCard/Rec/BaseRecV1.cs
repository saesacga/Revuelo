
using Unity.Netcode;

public class BaseRecV1 : NetworkBehaviour, IPlayable 
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
