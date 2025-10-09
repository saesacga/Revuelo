using UnityEngine;
using UnityEngine.InputSystem;

public class ClickHandler : MonoBehaviour 
{
    #region Singleton
    public static ClickHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;
    }
    #endregion

    private void Start()
    {
        InputHandler.Instance.Interact.performed += OnClick;
    }

    private void OnDestroy()
    {
        InputHandler.Instance.Interact.performed -= OnClick;
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = InputHandler.Instance.Point.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit) && NetworkHandler.Instance.MyTurn)
        {
            if (hit.collider.TryGetComponent<IClickable>(out var clickable))
            {
                clickable.OnClick();
            }
            //if (hit.collider.TryGetComponent(out BaseCard baseCard)) CardHandler.Instance.BaseCard = baseCard;
        }
    }
}
