using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] private float speed = 100f;
    [SerializeField] private float runSpeedMultiplier = 3f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float crouchHeight = 0.5f;
    private float originalHeight;
    private Rigidbody _rb;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float smoothness = 5f;
    [SerializeField] private float thirdPersonCameraHeight = 3f;
    [SerializeField] private float crouchCameraHeight = 0.5f;

    private bool isFirstPersonCameraActive = false;
    private bool isCameraSwitching = false;
    private bool isGrounded = true;
    private bool isRunning = false;
    private bool isCrouching = false;
    private bool canJump = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        originalHeight = _rb.transform.localScale.y;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isCameraSwitching)
        {
            SwitchCamera();
        }

        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            ToggleCrouch();
        }

        if (Input.GetButtonDown("Jump"))
        {
            TryJump();
        }
    }

    private void FixedUpdate()
    {
        float horizontalSpeed = Input.GetAxis("Horizontal") * GetEffectiveSpeed() * Time.fixedDeltaTime;
        float verticalSpeed = Input.GetAxis("Vertical") * GetEffectiveSpeed() * Time.fixedDeltaTime;
        _rb.velocity = transform.TransformDirection(new Vector3(horizontalSpeed, _rb.velocity.y, verticalSpeed));

        MoveCamera();
    }

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

    void MoveCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (isFirstPersonCameraActive)
        {
            transform.Rotate(Vector3.up * mouseX);

            float newRotationX = mainCamera.transform.eulerAngles.x - mouseY;
            mainCamera.transform.rotation = Quaternion.Euler(newRotationX, transform.eulerAngles.y, 0f);

            Vector3 newPosition = transform.position + transform.up * GetCameraHeight();
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, newPosition, Time.deltaTime * smoothness);
        }
        else
        {
            Vector3 targetPosition = transform.position - transform.forward * 5f + Vector3.up * GetCameraHeight();
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * smoothness);

            mainCamera.transform.LookAt(transform.position + transform.forward * 5f);
        }
    }

    float GetCameraHeight()
    {
        if (isCrouching)
        {
            return crouchCameraHeight;
        }
        else if (isFirstPersonCameraActive)
        {
            return 1f;
        }
        else
        {
            return thirdPersonCameraHeight;
        }
    }

    void SwitchCamera()
    {
        isFirstPersonCameraActive = !isFirstPersonCameraActive;
        isCameraSwitching = true;
        Invoke("ResetCameraSwitchFlag", 1.0f);
    }

    void ResetCameraSwitchFlag()
    {
        isCameraSwitching = false;
    }

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

    void TryJump()
    {
        if (isGrounded && canJump)
        {
            Jump();
        }
    }

    void Jump()
    {
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        canJump = false;
        isGrounded = false;
        Invoke("ResetJumpFlag", 0.2f);
    }

    void ResetJumpFlag()
    {
        canJump = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            canJump = true;
        }
    }
}
