using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] private float speed = 100f; // Базовая скорость движения объекта
    [SerializeField] private float hSpeed = 100f; // Горизонтальная скорость движения объекта
    [SerializeField] private float runSpeedMultiplier = 2f; // Множитель скорости при беге (Настройте по необходимости)
    [SerializeField] private float crouchSpeedMultiplier = 0.5f; // Множитель скорости при приседании (Настройте по необходимости)
    [SerializeField] private float jumpForce = 5f; // Сила прыжка объекта
    [SerializeField] private float crouchHeight = 0.5f; // Высота объекта при приседании (Настройте по необходимости)
    private float originalHeight; // Исходная высота объекта
    private Rigidbody _rb; // Компонент Rigidbody объекта
    [SerializeField] private Camera mainCamera; // Основная камера
    [SerializeField] private Camera firstPersonCamera; // Камера от первого лица
    [SerializeField] private float smoothness = 5f; // Плавность движения камеры
    [SerializeField] private float thirdPersonCameraHeight = 3f; // Высота камеры от третьего лица
    [SerializeField] private float firstPersonCameraHeight = 1.1f; // Высота камеры от первого лица
    [SerializeField] private float crouchCameraHeight = 0.5f; // Высота камеры при приседании (Настройте по необходимости)
    [SerializeField] private float cameraSwitchSpeed = 5f; // Скорость переключения камеры

    private bool isFirstPersonCameraActive = false; // Флаг активности камеры от первого лица
    private bool isCameraSwitching = false; // Флаг переключения камеры
    private bool isGrounded = true; // Флаг, указывающий, находится ли объект на земле
    private bool isRunning = false; // Флаг, указывающий, что объект бежит
    private bool isCrouching = false; // Флаг, указывающий, что объект приседает


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        originalHeight = _rb.transform.localScale.y;
    }

    private void FixedUpdate()
    {
        // Определение горизонтальной и вертикальной скорости в зависимости от текущего состояния
        float horizontalSpeed = Input.GetAxis("Horizontal") * GetEffectiveSpeed() * Time.fixedDeltaTime;
        float verticalSpeed = Input.GetAxis("Vertical") * GetEffectiveSpeed() * Time.fixedDeltaTime;

        // Применение скорости к Rigidbody
        _rb.velocity = transform.TransformDirection(new Vector3(horizontalSpeed, _rb.velocity.y, verticalSpeed));

        // Обновление положения камеры
        MoveCamera();

        // Переключение камеры при нажатии C
        if (Input.GetKeyDown(KeyCode.C) && !isCameraSwitching)
        {
            SwitchCamera();
        }

        // Проверка на бег
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Проверка на приседание при нажатии левого или правого Ctrl
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            ToggleCrouch();
        }

        // Проверка на прыжок при нажатии пробела
        if (Input.GetButtonDown("Jump"))
        {
            TryJump();
        }
    }

    // Определение эффективной скорости в зависимости от текущего состояния
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

    // Обновление положения камеры в зависимости от текущего режима
    void MoveCamera()
    {
        if (isFirstPersonCameraActive)
        {
            // Обработка вращения камеры относительно игрока
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            transform.Rotate(Vector3.up * mouseX);

            float newRotationX = firstPersonCamera.transform.eulerAngles.x - mouseY;
            firstPersonCamera.transform.rotation = Quaternion.Euler(newRotationX, transform.eulerAngles.y, 0f);

            // Обновление положения и высоты камеры в режиме первого лица
            Vector3 newPosition = transform.position + transform.up * GetCameraHeight();
            firstPersonCamera.transform.position = Vector3.Lerp(firstPersonCamera.transform.position, newPosition, Time.deltaTime * smoothness);
        }
        else
        {
            if (mainCamera != null)
            {
                // Обновление положения камеры в режиме третьего лица
                Vector3 targetPosition = transform.position - transform.forward * 5f + Vector3.up * GetCameraHeight();
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * smoothness * cameraSwitchSpeed);
                mainCamera.transform.LookAt(transform.position);
            }
        }
    }

    // Определение высоты камеры в зависимости от текущего состояния
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

    // Переключение между камерами
    void SwitchCamera()
    {
        isFirstPersonCameraActive = !isFirstPersonCameraActive;

        mainCamera.enabled = !isFirstPersonCameraActive;
        firstPersonCamera.enabled = isFirstPersonCameraActive;

        isCameraSwitching = true;

        Invoke("ResetCameraSwitchFlag", 1.0f);
    }

    // Сброс флага переключения камеры
    void ResetCameraSwitchFlag()
    {
        isCameraSwitching = false;
    }

    // Включение/выключение приседания
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

    // Попытка выполнить прыжок, если находится на земле
    void TryJump()
    {
        if (isGrounded)
        {
            Jump();
        }
    }

    // Выполнение прыжка
    void Jump()
    {
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;

        // Предотвращаем нежелательные повторные прыжки в воздухе
        Invoke("ResetGroundedFlag", 0.2f);
    }

    // Сброс флага нахождения на земле
    void ResetGroundedFlag()
    {
        isGrounded = true;
    }

    // Обработка столкновения с землей
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
