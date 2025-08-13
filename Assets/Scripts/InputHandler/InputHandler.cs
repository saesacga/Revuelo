using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    [SerializeField] private InputActionAsset _inputActionAsset;

    private InputAction _interact;
    public InputAction Interact => _interact;
    
    private void OnEnable()
    {
        _inputActionAsset.FindActionMap("Player").Enable();
    }
    private void OnDisable()
    {
        _inputActionAsset.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _interact = _inputActionAsset.FindAction("Interact");
    }
}
