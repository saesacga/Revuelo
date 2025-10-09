
using Unity.Netcode;

public class BaseRecV1 : NetworkBehaviour, IPlayable 
{ 
    public void PositiveEffect()
    {
        NetworkHandler.Instance.EndTurnRpc();
    }

    public void NegativeEffect()
    {
        NetworkHandler.Instance.EndTurnRpc();
    }
}
