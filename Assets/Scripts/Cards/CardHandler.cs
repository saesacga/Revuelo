using Unity.Netcode;
using UnityEngine;
using Sirenix.OdinInspector;

public class CardHandler : NetworkBehaviour 
{ 
    #region Singleton 
    public static CardHandler Instance { get; private set; }
    
    private void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        
        Instance = this; 
    } 
    #endregion

    #region Global References

    [field: SerializeField, DisableIf("@DiscardPile != null"), SceneObjectsOnly, FoldoutGroup("Global References")] 
    public Transform DiscardPile { get; private set; }
    
    [DisableIf("@SeatGrids != null && SeatGrids.Length == 6"), SceneObjectsOnly, FoldoutGroup("Global References")]
    public Transform[] SeatGrids;
    
    [field: SerializeField, DisableIf("@TypeImages != null && TypeImages.Length == 6"), AssetsOnly, FoldoutGroup("Global References")] 
    public Sprite[] TypeImages { get; private set; }
    
    [field: SerializeField, AssetsOnly, FoldoutGroup("Global References")]
    public GameObject[] CardPrefabs { get; private set; }
    
    #endregion

    #region Steal

    private readonly NetworkVariable<int> _stealQuantity = new NetworkVariable<int>();
    public int StealQuantity => _stealQuantity.Value;
    
    private readonly NetworkVariable<ulong> _stealerClientId = new NetworkVariable<ulong>();
    public ulong StealerClientId => _stealerClientId.Value;

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void StealServerRpc(int quantity, RpcParams rpcParams = default)
    {
        _stealQuantity.Value = quantity;
        _stealerClientId.Value = rpcParams.Receive.SenderClientId;
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void ReduceStealQuantityServerRpc()
    {
        _stealQuantity.Value--;
        if (_stealQuantity.Value == 0)
        {
            NetworkHandler.Instance.EndTurnServerRpc();
        }
    }

    #endregion
}
