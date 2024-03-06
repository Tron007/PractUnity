using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] private float speed = 100f; // ������� �������� �������� �������
    [SerializeField] private float hSpeed = 100f; // �������������� �������� �������� �������
    [SerializeField] private float runSpeedMultiplier = 2f; // ��������� �������� ��� ���� (��������� �� �������������)
    [SerializeField] private float crouchSpeedMultiplier = 0.5f; // ��������� �������� ��� ���������� (��������� �� �������������)
    [SerializeField] private float jumpForce = 5f; // ���� ������ �������
    [SerializeField] private float crouchHeight = 0.5f; // ������ ������� ��� ���������� (��������� �� �������������)
    private float originalHeight; // �������� ������ �������
    private Rigidbody _rb; // ��������� Rigidbody �������
    [SerializeField] private Camera mainCamera; // �������� ������
    [SerializeField] private Camera firstPersonCamera; // ������ �� ������� ����
    [SerializeField] private float smoothness = 5f; // ��������� �������� ������
    [SerializeField] private float thirdPersonCameraHeight = 3f; // ������ ������ �� �������� ����
    [SerializeField] private float firstPersonCameraHeight = 1.1f; // ������ ������ �� ������� ����
    [SerializeField] private float crouchCameraHeight = 0.5f; // ������ ������ ��� ���������� (��������� �� �������������)
    [SerializeField] private float cameraSwitchSpeed = 5f; // �������� ������������ ������

    private bool isFirstPersonCameraActive = false; // ���� ���������� ������ �� ������� ����
    private bool isCameraSwitching = false; // ���� ������������ ������
    private bool isGrounded = true; // ����, �����������, ��������� �� ������ �� �����
    private bool isRunning = false; // ����, �����������, ��� ������ �����
    private bool isCrouching = false; // ����, �����������, ��� ������ ���������


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        originalHeight = _rb.transform.localScale.y;
    }

    private void FixedUpdate()
    {
        // ����������� �������������� � ������������ �������� � ����������� �� �������� ���������
        float horizontalSpeed = Input.GetAxis("Horizontal") * GetEffectiveSpeed() * Time.fixedDeltaTime;
        float verticalSpeed = Input.GetAxis("Vertical") * GetEffectiveSpeed() * Time.fixedDeltaTime;

        // ���������� �������� � Rigidbody
        _rb.velocity = transform.TransformDirection(new Vector3(horizontalSpeed, _rb.velocity.y, verticalSpeed));

        // ���������� ��������� ������
        MoveCamera();

        // ������������ ������ ��� ������� C
        if (Input.GetKeyDown(KeyCode.C) && !isCameraSwitching)
        {
            SwitchCamera();
        }

        // �������� �� ���
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // �������� �� ���������� ��� ������� ������ ��� ������� Ctrl
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            ToggleCrouch();
        }

        // �������� �� ������ ��� ������� �������
        if (Input.GetButtonDown("Jump"))
        {
            TryJump();
        }
    }

    // ����������� ����������� �������� � ����������� �� �������� ���������
    float GetEffectiveSpeed()
    {
        if (isCrouching)
        {
            return speed * crouchSpeedMultiplier;
        }
        else if (isRunning)
        {
            return speed * runSpeedMultiplier;
        }
        else
        {
            return speed;
        }
    }

    // ���������� ��������� ������ � ����������� �� �������� ������
    void MoveCamera()
    {
        if (isFirstPersonCameraActive)
        {
            // ��������� �������� ������ ������������ ������
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            transform.Rotate(Vector3.up * mouseX);

            float newRotationX = firstPersonCamera.transform.eulerAngles.x - mouseY;
            firstPersonCamera.transform.rotation = Quaternion.Euler(newRotationX, transform.eulerAngles.y, 0f);

            // ���������� ��������� � ������ ������ � ������ ������� ����
            Vector3 newPosition = transform.position + transform.up * GetCameraHeight();
            firstPersonCamera.transform.position = Vector3.Lerp(firstPersonCamera.transform.position, newPosition, Time.deltaTime * smoothness);
        }
        else
        {
            if (mainCamera != null)
            {
                // ���������� ��������� ������ � ������ �������� ����
                Vector3 targetPosition = transform.position - transform.forward * 5f + Vector3.up * GetCameraHeight();
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * smoothness * cameraSwitchSpeed);
                mainCamera.transform.LookAt(transform.position);
            }
        }
    }

    // ����������� ������ ������ � ����������� �� �������� ���������
    float GetCameraHeight()
    {
        if (isCrouching)
        {
            return crouchCameraHeight;
        }
        else if (isFirstPersonCameraActive)
        {
            return firstPersonCameraHeight;
        }
        else
        {
            return thirdPersonCameraHeight;
        }
    }

    // ������������ ����� ��������
    void SwitchCamera()
    {
        isFirstPersonCameraActive = !isFirstPersonCameraActive;

        mainCamera.enabled = !isFirstPersonCameraActive;
        firstPersonCamera.enabled = isFirstPersonCameraActive;

        isCameraSwitching = true;

        Invoke("ResetCameraSwitchFlag", 1.0f);
    }

    // ����� ����� ������������ ������
    void ResetCameraSwitchFlag()
    {
        isCameraSwitching = false;
    }

    // ���������/���������� ����������
    void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        if (isCrouching)
        {
            _rb.transform.localScale = new Vector3(_rb.transform.localScale.x, crouchHeight, _rb.transform.localScale.z);
        }
        else
        {
            _rb.transform.localScale = new Vector3(_rb.transform.localScale.x, originalHeight, _rb.transform.localScale.z);
        }
    }

    // ������� ��������� ������, ���� ��������� �� �����
    void TryJump()
    {
        if (isGrounded)
        {
            Jump();
        }
    }

    // ���������� ������
    void Jump()
    {
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;

        // ������������� ������������� ��������� ������ � �������
        Invoke("ResetGroundedFlag", 0.2f);
    }

    // ����� ����� ���������� �� �����
    void ResetGroundedFlag()
    {
        isGrounded = true;
    }

    // ��������� ������������ � ������
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
