using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] private float speed = 100f; // Базовая скорость движения объекта
    [SerializeField] private float runSpeedMultiplier = 3f; // Множитель скорости при беге (Настройте по необходимости)
    [SerializeField] private float crouchSpeedMultiplier = 0.5f; // Множитель скорости при приседании (Настройте по необходимости)
    [SerializeField] private float jumpForce = 5f; // Сила прыжка объекта
    [SerializeField] private float crouchHeight = 0.5f; // Высота объекта при приседании (Настройте по необходимости)
    private float originalHeight; // Исходная высота объекта
    private Rigidbody _rb; // Компонент Rigidbody объекта
    [SerializeField] private Camera mainCamera; // Основная камера
    [SerializeField] private float smoothness = 5f; // Плавность движения камеры
    [SerializeField] private float thirdPersonCameraHeight = 3f; // Высота камеры от третьего лица
    [SerializeField] private float crouchCameraHeight = 0.5f; // Высота камеры при приседании (Настройте по необходимости)

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

    // Получение эффективной скорости в зависимости от состояния приседания и бега
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

    // Обработка движения камеры в зависимости от активного режима
    void MoveCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (isFirstPersonCameraActive)
        {
            // Поворот объекта по горизонтали
            transform.Rotate(Vector3.up * mouseX);

            // Поворот камеры от первого лица по вертикали
            float newRotationX = mainCamera.transform.eulerAngles.x - mouseY;
            mainCamera.transform.rotation = Quaternion.Euler(newRotationX, transform.eulerAngles.y, 0f);

            // Позиционирование камеры от первого лица
            Vector3 newPosition = transform.position + transform.up * GetCameraHeight();
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, newPosition, Time.deltaTime * smoothness);
        }
        else
        {
            // Позиционирование камеры от третьего лица
            Vector3 targetPosition = transform.position - transform.forward * 5f + Vector3.up * GetCameraHeight();
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * smoothness);

            // Наведение камеры на объект
            mainCamera.transform.LookAt(transform.position + transform.forward * 5f);
        }
    }

    // Получение высоты камеры в зависимости от состояния приседания и режима камеры
    float GetCameraHeight()
    {
        if (isCrouching)
        {
            return crouchCameraHeight;
        }
        else if (isFirstPersonCameraActive)
        {
            return 1f; // Нулевая высота для камеры от первого лица
        }
        else
        {
            return thirdPersonCameraHeight;
        }
    }

    // Переключение между режимами камеры
    void SwitchCamera()
    {
        isFirstPersonCameraActive = !isFirstPersonCameraActive;

        // Установка флага переключения камеры и задержка сброса флага
        isCameraSwitching = true;
        Invoke("ResetCameraSwitchFlag", 1.0f);
    }

    // Сброс флага переключения камеры
    void ResetCameraSwitchFlag()
    {
        isCameraSwitching = false;
    }

    // Переключение состояния приседания
    void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        // Изменение размера объекта при приседании или возвращение к исходному размеру
        if (isCrouching)
        {
            _rb.transform.localScale = new Vector3(_rb.transform.localScale.x, crouchHeight, _rb.transform.localScale.z);
        }
        else
        {
            _rb.transform.localScale = new Vector3(_rb.transform.localScale.x, originalHeight, _rb.transform.localScale.z);
        }
    }

    // Попытка выполнить прыжок (если находится на земле)
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

        // Задержка перед восстановлением флага "на земле"
        Invoke("ResetGroundedFlag", 0.2f);
    }

    // Сброс флага "на земле"
    void ResetGroundedFlag()
    {
        isGrounded = true;
    }

    // Обработка столкновения с объектами
    private void OnCollisionEnter(Collision collision)
    {
        // Проверка на столкновение с землей (по тегу "Ground")
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
