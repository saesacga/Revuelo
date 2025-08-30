using Unity.Netcode;
using UnityEngine;

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
            //Debug.Log($"[SERVER] Cliente conectado: {clientId}");
        }
    }
    private void OnClientDisconnected(ulong clientId)
    {
        if (!IsServer) return;

        if (ClientIds.Contains(clientId))
        {
            ClientIds.Remove(clientId);
            //Debug.Log($"[SERVER] Cliente desconectado: {clientId}");
        }
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
