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
    
    //[field: SerializeField] [field: ReadOnly] public GameObject CardNetworkDataPrefab { get; private set; }
    
    public Transform[] SeatGrids; 
    [field: SerializeField] [field: ReadOnly] public Transform DiscardPile { get; private set; }
    
    [field: SerializeField] [field: ReadOnly] public Sprite[] TypeImages { get; private set; }
    
    [field: SerializeField, AssetsOnly] public GameObject[] CardPrefabs { get; private set; }
    
        //public BaseCard.BaseCard BaseCard { get; set; }
    
        /*private static int Count { get; set; }
         
         [Rpc(SendTo.Server, RequireOwnership = false)]
        protected void ChangeCardHandServerRpc(RpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;
            bool senderIsOwner = clientId == OwnerClientId;
            ChangeCardHandLocalRpc(clientId, senderIsOwner);
        }
         
    protected static bool Stealing { get; set; }
    protected static int StealQuantity { get; set; }
    private void StealCards()
    {
        if (!Stealing || NetObj.IsOwner || CardNetworkDataInstance.CardDiscarded.Value) return; //Stealing
        CardNetworkDataInstance.ChangeCardHandServerRpc();
        Count++;
        if (StealQuantity > Count) return;
        Stealing = false; Count = 0; NetworkHandler.Instance.EndTurnRpc();
    }*/
    
        //For stealing
        
        /*int seat = NetworkHandler.Instance.PlayerSeats[clientId];

    _visualBaseCardRef.transform.SetParent(CardHandler.Instance.SeatGrids[seat]);
    int lastIndex = CardHandler.Instance.SeatGrids[seat].childCount - 2;
    _visualBaseCardRef.transform.SetSiblingIndex(lastIndex);

    if (IsServer) GetComponent<NetworkObject>().ChangeOwnership(clientId);*/
}
