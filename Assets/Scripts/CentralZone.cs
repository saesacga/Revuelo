using System;
using DG.Tweening;
using UnityEngine;

public class CentralZone : MonoBehaviour
{
    public enum CentralZoneState { Deck, DiscardPile }
    
    public static CentralZone Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    public void ChangeCentralZone(CentralZoneState state)
    {
        var rotation = 0f;
        
        switch (state)
        {
            case CentralZoneState.Deck:
                rotation = 0f;
                break;
            case CentralZoneState.DiscardPile:
                rotation = 180f;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        transform.DOLocalRotate(new Vector3(rotation, 0, 0), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutElastic);
    }
}
