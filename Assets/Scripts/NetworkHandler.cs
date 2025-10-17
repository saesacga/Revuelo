using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Acts as Game General Manager, sets general systems such as turns, seats, etc.
/// </summary>
public class NetworkHandler : NetworkBehaviour
{
    public static NetworkHandler Instance { get; private set; }

    private NetworkList<ulong> _clientIds;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _clientIds = new NetworkList<ulong>();
    }

    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        InputHandler.Instance.Test.performed += ctx => AssignSeatsRpc(); //Here, only for testing, must be removed soon
    }
    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    public override void OnNetworkSpawn()
    {
        _playerInTurn.OnValueChanged += OnTurnChanged;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;
        
        if (!_clientIds.Contains(clientId))
        {
            _clientIds.Add(clientId);
        }
        
        if (_clientIds.Count == 1) //Set the first player as the first to play
            _playerInTurn.Value = clientId;
    }
    private void OnClientDisconnected(ulong clientId)
    {
        if (!IsServer) return;

        int index = _clientIds.IndexOf(clientId);
        if (index != -1)
        {
            _clientIds.RemoveAt(index);

            if (_playerInTurn.Value == clientId && _clientIds.Count > 0)
            {
                _currentTurnIndex %= _clientIds.Count;
                _playerInTurn.Value = _clientIds[_currentTurnIndex];
            }
        }
    }
    
    #region Turn Handler
    
    private int _currentTurnIndex;
    private readonly NetworkVariable<ulong> _playerInTurn = new NetworkVariable<ulong>();
    public bool IsMyTurn => _playerInTurn.Value == NetworkManager.Singleton.LocalClientId;

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void EndTurnServerRpc()
    {
        if (!IsServer || _clientIds.Count == 0) return;

        _currentTurnIndex = (_currentTurnIndex + 1) % _clientIds.Count;
        _playerInTurn.Value = _clientIds[_currentTurnIndex];
    }
    private void OnTurnChanged(ulong oldValue, ulong newValue)
    {
        CardType_Color.CardPicked = false;
        CardNetwork.UsedCard = false;
        
        if (IsMyTurn) CentralZone.Instance.ChangeCentralZone(CentralZone.CentralZoneState.Deck);
    }
    
    #endregion

    #region Assign Seats

    private Dictionary<ulong, int> _playerSeats = new Dictionary<ulong, int>();
    public IReadOnlyDictionary<ulong, int> PlayerSeats => _playerSeats;
    
    [Rpc(SendTo.ClientsAndHost)]
    private void AssignSeatsRpc()
    {
        int count = _clientIds.Count;
        int localIndex = _clientIds.IndexOf(NetworkManager.Singleton.LocalClientId);

        foreach (ulong playerId in _clientIds)
        {
            int playerIndex = _clientIds.IndexOf(playerId);
            int seatIndex = (playerIndex - localIndex + count) % count;

            _playerSeats[playerId] = seatIndex;
        }

        if (!IsMyTurn) CentralZone.Instance.ChangeCentralZone(CentralZone.CentralZoneState.DiscardPile);
    }

    #endregion
}
