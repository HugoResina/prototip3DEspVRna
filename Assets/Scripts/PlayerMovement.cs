using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(PlayerInputs))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Camera _playerCamera;

    [Header("Movement Settings")]
    public float moveSpeed = 3.0f;
    public float sprintSpeed = 6.0f;

    [Header("Look Settings")]
    public float lookSensitivityH = 0.1f;
    public float lookSensitivityV = 0.1f;
    public float looklimitV = 80.0f;

    [Header("Gravity Settings")]
    public float gravity = 9.81f;

    private PlayerInputs _playerInputs;
    private PlayerInteraction _playerInteraction;

    private Vector3 _currentMovement = Vector3.zero;
    private Vector2 _cameraRotation = Vector2.zero;

    private void OnEnable() => UIManager.Instance.OnPauseGame += DisableInputs;
    private void OnDisable() => UIManager.Instance.OnPauseGame -= DisableInputs;

    private void Awake()
    {
        _playerInputs = GetComponent<PlayerInputs>();
        _playerInteraction = GetComponent<PlayerInteraction>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        UIManager.Instance.SetCursorState(true, false);
    }

    private void Update()
    {
       if(!_playerInteraction.isInteracting)
            Movement();
    }

    private void LateUpdate()
    {
        if(!_playerInteraction.isInteracting)
            Look();
    }

    private void Movement()
    {
        float speed = _playerInputs.SprintInput ? sprintSpeed : moveSpeed;
        float HorizontalSpeed = _playerInputs.MoveInput.x * speed;
        float VerticalSpeed = _playerInputs.MoveInput.y * speed;

        Vector3 HorizontalMovement = new Vector3(HorizontalSpeed, 0, VerticalSpeed);
        HorizontalMovement = transform.rotation * HorizontalMovement;

        Fall();

        _currentMovement.x = HorizontalMovement.x;
        _currentMovement.z = HorizontalMovement.z;

        _characterController.Move(_currentMovement * Time.deltaTime);
    }

    private void Look()
    {
        _cameraRotation.x = _playerInputs.LookInput.x * lookSensitivityH;
        _cameraRotation.y -= _playerInputs.LookInput.y * lookSensitivityV;
        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y, -looklimitV, looklimitV);

        transform.Rotate(0, _cameraRotation.x, 0);
        _playerCamera.transform.localRotation = Quaternion.Euler(_cameraRotation.y, 0, 0);
    }

    private void Fall()
    {
        if (_characterController.isGrounded)
        {
            _currentMovement.y = -0.5f;
        }
        else
        {
            _currentMovement.y -= gravity * Time.deltaTime;
        }
    }

    private void DisableInputs(bool disable)
    {
        _playerInputs.enabled = !disable;
    }
}