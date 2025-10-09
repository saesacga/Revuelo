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

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        if (!_clientIds.Contains(clientId))
        {
            _clientIds.Add(clientId);
        }
    }
    private void OnClientDisconnected(ulong clientId)
    {
        if (!IsServer) return;

        if (_clientIds.Contains(clientId))
        {
            _clientIds.Remove(clientId);
        }
    }

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
        Debug.Log("Seats assigned!");
    }

    private int _currentTurnIndex;
    public bool MyTurn { get; private set; } = true;

    [Rpc(SendTo.ClientsAndHost)]
    public void EndTurnRpc()
    {
        if (_currentTurnIndex < _clientIds.Count - 1) { _currentTurnIndex++; }
        else { _currentTurnIndex = 0; }

        int myIndex = _clientIds.IndexOf(NetworkManager.Singleton.LocalClientId);
        
        CardType_Color.CardPicked = false;
        BaseCard.UsedCard = false;

        MyTurn = myIndex == _currentTurnIndex;
    }
}
