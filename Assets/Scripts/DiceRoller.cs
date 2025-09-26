using Unity.Netcode;
using UnityEngine;

public class DiceRoller : NetworkBehaviour, IClickable
{
    private Transform[] _faceEmpties;
    private Rigidbody _rb;

    private bool _isChecking;

    private void Awake()
    {
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

        if (_rb.IsSleeping() == false)
        {
            _isChecking = false;
        }
    }

    public NetworkVariable<int> DiceNumber = new NetworkVariable<int>();

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

        DiceNumber.Value = faceNumber;
        Debug.Log($"Dice rolled {DiceNumber.Value}");
    }

    public void OnClick()
    {
        RollDiceServerRpc();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void RollDiceServerRpc()
    {
        if (!IsServer) return;

        _rb.AddForce(new Vector3(Random.Range(-5f, 5f), Random.Range(8f, 12f), Random.Range(-5f, 5f)), ForceMode.Impulse);
        _rb.AddTorque(Random.insideUnitSphere * Random.Range(5f, 10f), ForceMode.Impulse);
    }
}
