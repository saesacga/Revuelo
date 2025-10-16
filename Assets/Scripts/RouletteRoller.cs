using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;

public class RouletteRoller : NetworkBehaviour
{
    private readonly float _startAngle = -64f;
    private readonly float _angleStep = 18f;
    
    private readonly Dictionary<int, float> _numberToAngle = new Dictionary<int, float>();

    private Transform _visualTransform;
    private readonly float _spinDuration = 2f;
    private bool _spinning;
    
    public event Action<int> OnRouletteSpinned; 
    private NetworkVariable<int> _rouletteNumber = new NetworkVariable<int>();
    
    public static RouletteRoller Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;
        
        GenerateRouletteMap();
        
        _visualTransform = transform.GetChild(0);
    }

    private void GenerateRouletteMap()
    {
        _numberToAngle.Clear();

        var small = 1;
        var large = 20;
        var currentAngle = _startAngle;
        
        for (var i = 0; i < 20; i++)
        {
            var currentNumber = (i % 2 == 0) ? small++ : large--;
            _numberToAngle[currentNumber] = currentAngle;

            currentAngle += _angleStep;
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SpinRouletteServerRpc()
    {
        _rouletteNumber.Value = UnityEngine.Random.Range(1, 21);
        SpinRouletteRpc(_rouletteNumber.Value);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void SpinRouletteRpc(int number)
    {
        float targetAngle = _numberToAngle[number];

        // Add an extra 360 to the target angle to make sure the rotation is always positive
        float finalRotationY = 360f * 3 + targetAngle;
        
        _visualTransform.DOKill();
        
        _visualTransform
            .DOLocalRotate(new Vector3(0, finalRotationY, 0), _spinDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                OnRouletteSpinned?.Invoke(number);
            });
    }
}
