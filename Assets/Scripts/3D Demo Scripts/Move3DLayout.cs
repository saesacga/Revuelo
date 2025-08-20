using UnityEngine;
using UnityEngine.InputSystem;
using Flexalon;
public class Move3DLayout : MonoBehaviour
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
        Debug.Log("Clicked on the screen");

        Vector2 screenPosition = InputHandler.Instance.Point.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Card"))
            {
                hit.collider.transform.SetParent(_newParent, true);

                GetComponent<FlexalonObject>()?.ForceUpdate();
            }
            else
            {
                Debug.LogWarning("Clicked object is not a card.");
            }
        }
    }
}
