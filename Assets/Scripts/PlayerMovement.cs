using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Camera _playerCamera;

    [Header("Movement Settings")]
    public float moveSpeed = 3.0f;

    [Header("Look Settings")]
    public float lookSensitivityH = 0.1f;
    public float lookSensitivityV = 0.1f;
    public float looklimitV = 80.0f;

    [Header("Gravity Settings")]
    public float gravity = 9.81f;

    private PlayerInputs _playerInputs;
    private Vector3 _currentMovement = Vector3.zero;
    private Vector2 _cameraRotation = Vector2.zero;

    private void Awake()
    {
        _playerInputs = GetComponent<PlayerInputs>();
    }

    private void Update()
    {
        Movement();
    }

    private void LateUpdate()
    {
        Look();
    }

    private void Movement()
    {
        float HorizontalSpeed = _playerInputs.MoveInput.x * moveSpeed;
        float VerticalSpeed = _playerInputs.MoveInput.y * moveSpeed;

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
        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSensitivityV * _playerInputs.LookInput.y, -looklimitV, looklimitV);

        transform.Rotate(0, _cameraRotation.x, 0);
        _playerCamera.transform.localRotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0);
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
}
