using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class NetworkHandler : NetworkBehaviour
{
    public static NetworkHandler Instance { get; private set; }

    public NetworkList<ulong> ClientIds { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        ClientIds = new NetworkList<ulong>();
    }

    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        InputHandler.Instance.Test.performed += ctx => AssignSeatsRpc(); //Here only for testing, must be removed soon
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

        if (!ClientIds.Contains(clientId))
        {
            ClientIds.Add(clientId);
        }
    }
    private void OnClientDisconnected(ulong clientId)
    {
        if (!IsServer) return;

        if (ClientIds.Contains(clientId))
        {
            ClientIds.Remove(clientId);
        }
    }

    private Dictionary<ulong, int> _playerSeats = new Dictionary<ulong, int>();
    public IReadOnlyDictionary<ulong, int> PlayerSeats => _playerSeats;

    [Rpc(SendTo.ClientsAndHost)]
    private void AssignSeatsRpc()
    {
        int count = ClientIds.Count;
        int localIndex = ClientIds.IndexOf(NetworkManager.Singleton.LocalClientId);

        foreach (ulong element in ClientIds)
        {
            int playerIndex = ClientIds.IndexOf(element);
            int seatIndex = (playerIndex - localIndex + count) % count;

            _playerSeats[element] = seatIndex;
        }

        Debug.Log("Seats Assigned");
    }

    private int _currentTurnIndex;
    private bool _myTurn = true;
    public bool MyTurn => _myTurn;

    [Rpc(SendTo.ClientsAndHost)]
    public void EndTurnRpc()
    {
        if (_currentTurnIndex < ClientIds.Count - 1) { _currentTurnIndex++; }
        else { _currentTurnIndex = 0; }

        int myIndex = ClientIds.IndexOf(NetworkManager.Singleton.LocalClientId);

        _myTurn = myIndex == _currentTurnIndex;
    }
}
