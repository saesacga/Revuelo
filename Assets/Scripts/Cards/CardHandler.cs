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
    
    public Transform[] SeatGrids; 
    [field: SerializeField] [field: ReadOnly] public Transform DiscardPile { get; private set; }
    
    [field: SerializeField] [field: ReadOnly] public Sprite[] TypeImages { get; private set; }
    
    [field: SerializeField, AssetsOnly] public GameObject[] CardPrefabs { get; private set; }
}
