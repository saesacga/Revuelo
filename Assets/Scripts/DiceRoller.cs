using Unity.Netcode;
using System;
using UnityEngine;

public class DiceRoller : NetworkBehaviour
{
    private Transform[] _faceEmpties;
    private Rigidbody _rb;

    private bool _isChecking;
    
    public static DiceRoller Instance { get; private set; } //Singleton
    
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;
        
        _faceEmpties = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            _faceEmpties[i] = transform.GetChild(i);
        }

        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_rb.IsSleeping() && !_isChecking)
        {
            GetTopFaceServerRpc();
            _isChecking = true;
        }

        if (!_rb.IsSleeping())
        {
            _isChecking = false;
        }
    }
    
    public event Action<int> OnDiceRolled;

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void GetTopFaceServerRpc()
    {
        float bestDot = -1f;
        int faceNumber = -1;

        for (int i = 0; i < _faceEmpties.Length; i++)
        {
            Vector3 dir = _faceEmpties[i].up;
            float dot = Vector3.Dot(dir, Vector3.up);

            if (dot > bestDot)
            {
                bestDot = dot;
                faceNumber = i + 1;
            }
        }
        SentNumberRpc(faceNumber);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void SentNumberRpc(int number)
    {
        //Debug.Log($"Dice rolled {number}");
        OnDiceRolled?.Invoke(number);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RollDiceServerRpc()
    {
        if (!IsServer) return;

        _rb.AddForce(new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(8f, 12f), UnityEngine.Random.Range(-5f, 5f)), ForceMode.Impulse);
        _rb.AddTorque(UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(5f, 10f), ForceMode.Impulse);
    }
}
