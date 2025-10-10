using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public abstract class CardNetwork : NetworkBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{ 
    public CardType_Color.CardColor CardColorValueRef { get; set; }
    public CardType_Color.CardType CardTypeValueRef { get; set; }
    
    private readonly NetworkVariable<CardType_Color.CardType> _cardType = new NetworkVariable<CardType_Color.CardType>();
    private readonly NetworkVariable<CardType_Color.CardColor> _cardColor = new NetworkVariable<CardType_Color.CardColor>();
    
    protected CardType_Color.CardType CardType => _cardType.Value;
    public CardType_Color.CardColor CardColor => _cardColor.Value;
    
    private Sequence _sequenceTween;
    private Transform _visualTransform;
        
    protected abstract void OnInitialize();
    
    public override void OnNetworkSpawn()
    {
        SetCardDataRpc();
        int seat = NetworkHandler.Instance.PlayerSeats[GetComponent<NetworkObject>().OwnerClientId]; //Pick a local seat for each card based on an owner
        
        transform.SetParent(CardHandler.Instance.SeatGrids[seat]); //Set new card parent to local player grid
        int lastIndex = CardHandler.Instance.SeatGrids[seat].childCount - 2;
        transform.SetSiblingIndex(lastIndex); //Change the position in hierarchy so cards instantiate in the middle of the hand
        
        OnInitialize();
        
        _visualTransform = transform.GetChild(0);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)] private void SetCardDataRpc()
    {
        _cardType.Value = CardTypeValueRef;
        _cardColor.Value = CardColorValueRef;
    }

    private readonly NetworkVariable<bool> _cardDiscarded = new NetworkVariable<bool>();
    [Rpc(SendTo.Server, RequireOwnership = false)] protected void CardDiscardedServerRpc(bool value)
    {
        _cardDiscarded.Value = value;
    }

    public static bool UsedCard { get; set; }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsOwner && CardType_Color.CardPicked && !_cardDiscarded.Value && !UsedCard) 
            UseCardRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    protected virtual void UseCardRpc()
    {
        transform.SetParent(CardHandler.Instance.DiscardPile);
        UsedCard = true;
    }
    
    private float _rotationTime = 0.1f;
    private float _moveTime = 0.2f;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsOwner || !CardType_Color.CardPicked || _cardDiscarded.Value || UsedCard)
            return;
        
        _sequenceTween?.Kill();
        
        _sequenceTween = DOTween.Sequence();
        
        _sequenceTween.Append(_visualTransform.DOLocalMove(new Vector3(0, 0.1f, 0.1f), _moveTime));
        
        _sequenceTween.Join(
            DOTween.Sequence()
                .Append(_visualTransform.DOLocalRotate(new Vector3(0, 60, 0), _rotationTime))
                .Append(_visualTransform.DOLocalRotate(Vector3.zero, _rotationTime))
            );
        _sequenceTween.SetEase(Ease.InOutQuad);
        _sequenceTween.Play();
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsOwner || !CardType_Color.CardPicked || _cardDiscarded.Value || UsedCard)
            return;
        
        _sequenceTween?.Kill();
        
        _sequenceTween = DOTween.Sequence();

        _sequenceTween.Append(_visualTransform.DOLocalMove(Vector3.zero, _moveTime));
        _sequenceTween.Join(
            DOTween.Sequence()
                .Append(_visualTransform.DOLocalRotate(new Vector3(0, 60, 0), _rotationTime))
                .Append(_visualTransform.DOLocalRotate(Vector3.zero, _rotationTime))
        );
        _sequenceTween.SetEase(Ease.InOutQuad);
        _sequenceTween.Play();
    }
}
