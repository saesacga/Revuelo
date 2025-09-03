using Unity.Netcode;
using UnityEngine;

public class DiceRoller : NetworkBehaviour, IClickable
{
    [SerializeField] private Transform[] _faceEmpties;
    private Rigidbody _rb;

    private bool _isChecking;

    private void Awake()
    {
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

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void GetTopFaceServerRpc()
    {
        Transform bestFace = null;
        float bestDot = -1f;
        int faceNumber = -1;

        for (int i = 0; i < _faceEmpties.Length; i++)
        {
            Vector3 dir = _faceEmpties[i].up;
            float dot = Vector3.Dot(dir, Vector3.up);

            if (dot > bestDot)
            {
                bestDot = dot;
                bestFace = _faceEmpties[i];
                faceNumber = i + 1;
            }
        }

        Debug.Log($"Dice rolled {faceNumber}");
        return;
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
