using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    [SerializeField] private InputActionAsset _inputActionAsset;

    private InputAction _interact;
    private InputAction _point;
    private InputAction _test;
    private InputAction _diceCam;

    public InputAction Interact => _interact;
    public InputAction Point => _point;
    public InputAction Test => _test;
    public InputAction DiceCam => _diceCam;

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
        _point = _inputActionAsset.FindAction("Point");
        _test = _inputActionAsset.FindAction("Test");
        _diceCam = _inputActionAsset.FindAction("DiceCam");
    }
}
