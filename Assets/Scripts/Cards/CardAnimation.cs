using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Flexalon;
using Unity.Netcode;

public class CardAnimation : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Sequence _sequenceTween;
    private Transform _visualTransform;
    private Transform _colliderTransform;
    private Transform _parent;
    private int _index;
    
    private CardNetwork _cardNetwork;
    
    [SerializeField] private float _endPositionY = 0.1f;
    [SerializeField] private float _endPositionZ = 0.2f;
    [SerializeField] private float _scale = 1.1f;
    [SerializeField] private float _siblingsReduction = 0.03f;
    [SerializeField] private float _moveTime = 0.2f;

    private void Awake()
    {
        _visualTransform = transform.GetChild(0);
        _colliderTransform = transform.GetChild(1);
        _cardNetwork = GetComponent<CardNetwork>();
    }

    private CardAnimation _left;
    private CardAnimation _right;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsOwner || _cardNetwork.CardDiscardedValue)
            return;
        
        var endPos = new Vector3(0, _endPositionY, _endPositionZ);
        HoverToggle(endPos, _scale);
        
        _parent = transform.parent;
        _index = transform.GetSiblingIndex();
        
        if (_index > 1) //Check if there's a card in the left
        {
            _left = _parent.GetChild(_index - 1).GetComponent<CardAnimation>();
            _left.HoverToggle(endPos - (Vector3.one * _siblingsReduction), _scale - _siblingsReduction);
        }

        if (_index < _parent.childCount - 2) //Check if there's a card in the right
        {
            _right = _parent.GetChild(_index + 1).GetComponent<CardAnimation>();
            _right.HoverToggle(endPos - (Vector3.one * _siblingsReduction), _scale - _siblingsReduction);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsOwner || _cardNetwork.CardDiscardedValue)
            return;
        
        HoverToggle(Vector3.zero, 1);
        _left?.HoverToggle(Vector3.zero, 1);
        _right?.HoverToggle(Vector3.zero, 1);
    }

    private void HoverToggle(Vector3 endPosition, float scale)
    {
        _colliderTransform.localPosition = endPosition;
        _colliderTransform.localScale = Vector3.one * scale;
        
        _sequenceTween?.Kill();
        _sequenceTween = DOTween.Sequence();
        
        _sequenceTween.Append(_visualTransform.DOLocalMove(endPosition, _moveTime));
        _sequenceTween.Join(_visualTransform.DOScale(Vector3.one * scale, _moveTime));
        
        _sequenceTween.SetEase(Ease.InOutQuad);
        _sequenceTween.Play();
    }
}
