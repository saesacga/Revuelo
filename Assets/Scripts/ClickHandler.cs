using UnityEngine;
using UnityEngine.InputSystem;
using Flexalon;
public class ClickHandler : MonoBehaviour
{
    [SerializeField] private Transform _newParent;

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

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Card"))
            {
                hit.collider.transform.SetParent(_newParent, true);

                GetComponent<FlexalonObject>()?.ForceUpdate();
            }
            else if (hit.collider.CompareTag("Deck"))
            {
                hit.collider.GetComponent<CardType_Color>()?.DeckPressed();
            }
        }
    }
}
