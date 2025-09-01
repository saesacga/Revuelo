using UnityEngine;

public class CardHandler : MonoBehaviour
{
    #region Singleton
    public static CardHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;
    }
    #endregion

    [field: SerializeField] [field: ReadOnly] public GameObject CardNetworkDataPrefab { get; private set; }

    [SerializeField] public Transform[] SeatGrids;
    [field: SerializeField] [field: ReadOnly] public Transform DiscardPile { get; private set; }

    [field: SerializeField] [field: ReadOnly] public Sprite[] TypeImages { get; private set; }

    [field: SerializeField] public GameObject[] BaseAtkPrefabs { get; private set; }
    [field: SerializeField] public GameObject[] BaseDefPrefabs { get; private set; }
    [field: SerializeField] public GameObject[] BaseRecPrefabs { get; private set; }
}
