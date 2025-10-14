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
    
    private Vector3 _endPosition;

    private void Awake()
    {
        _visualTransform = transform.GetChild(0);
        _colliderTransform = transform.GetChild(1);
        _cardNetwork = GetComponent<CardNetwork>();
    }

    private float _rotationTime = 0.1f;
    private float _moveTime = 0.2f;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsOwner || _cardNetwork.CardDiscarded)
            return;
        
        _endPosition = GetComponent<FlexalonCurveAnimator>().EndPosition;
        
        var scale = new Vector3(1.1f, 1.1f, 1.1f);
        
        HoverToggle(new Vector3(_endPosition.x, _endPosition.y + 0.1f, _endPosition.z + 0.05f), scale);
        
        _parent = transform.parent;
        _index = transform.GetSiblingIndex();
        
        if (_index > 1) //Check if there's a card in the left
        {
            CardAnimation left = _parent.GetChild(_index - 1).GetComponent<CardAnimation>();
            left.HoverToggle(new Vector3(left.transform.position.x, _endPosition.y + 0.05f, _endPosition.z + 0.03f), scale);
        }

        if (_index < _parent.childCount - 2) //Check if there's a card in the right
        {
            CardAnimation right = _parent.GetChild(_index + 1).GetComponent<CardAnimation>();
            right.HoverToggle(new Vector3(right.transform.position.x, _endPosition.y + 0.05f, _endPosition.z + 0.03f), scale);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsOwner || _cardNetwork.CardDiscarded)
            return;
        
        HoverToggle(_endPosition, Vector3.one);
        
        _parent = transform.parent;
        _index = transform.GetSiblingIndex();
        
        if (_index > 1) //Check if there's a card in the left
        {
            CardAnimation left = _parent.GetChild(_index - 1).GetComponent<CardAnimation>();
            left.HoverToggle(left.GetComponent<FlexalonCurveAnimator>().EndPosition, Vector3.one);
        }

        if (_index < _parent.childCount - 2) //Check if there's a card in the right
        {
            CardAnimation right = _parent.GetChild(_index + 1).GetComponent<CardAnimation>();
            right.HoverToggle(right.GetComponent<FlexalonCurveAnimator>().EndPosition, Vector3.one);
        }
    }

    private void HoverToggle(Vector3 endPosition, Vector3 scale)
    {
        _colliderTransform.position = endPosition;
        _colliderTransform.localScale = scale;
        
        _sequenceTween?.Kill();
        _sequenceTween = DOTween.Sequence();
        
        _sequenceTween.Append(_visualTransform.DOMove(endPosition, _moveTime));
        _sequenceTween.Join(
            DOTween.Sequence()
                .Append(_visualTransform.DOLocalRotate(new Vector3(0, 60, 0), _rotationTime))
                .Append(_visualTransform.DOLocalRotate(Vector3.zero, _rotationTime))
        );
        _sequenceTween.Join(_visualTransform.DOScale(scale, _moveTime));
        
        _sequenceTween.SetEase(Ease.InOutQuad);
        _sequenceTween.Play();
    }
}
