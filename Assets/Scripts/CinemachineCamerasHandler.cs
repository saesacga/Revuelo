using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CinemachineCamerasHandler : MonoBehaviour
{
    #region Singleton
    public static CinemachineCamerasHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;
    }
    #endregion

    [SerializeField, ReadOnly] private CinemachineCamera _playerCam;
    [SerializeField, ReadOnly] private CinemachineCamera _diceCam;

    private void Start()
    {
        InputHandler.Instance.DiceCam.performed += SwitchCam;
    }
    private void OnDestroy()
    {
        InputHandler.Instance.DiceCam.performed -= SwitchCam;
    }

    private bool _toggle;
    private void SwitchCam(InputAction.CallbackContext context)
    {
        if (_toggle)
        {
            _playerCam.Priority = 1;
            _diceCam.Priority = 0;
            _toggle = false;
        }
        else
        {
            _playerCam.Priority = 0;
            _diceCam.Priority = 1;
            _toggle = true;
        }
    }
}
