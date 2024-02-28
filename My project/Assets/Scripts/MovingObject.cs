using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed = 100f;
    public float hSpeed = 100f;
    private Rigidbody _rb;
    public Camera mainCamera;
    public float smoothness = 5f; // �������� ��� �������� ���������� ������
    public float cameraHeight = 3f; // ������ ������ ��� ��������

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float horizontalSpeed = Input.GetAxis("Horizontal") * hSpeed * Time.fixedDeltaTime;
        float verticalSpeed = Input.GetAxis("Vertical") * speed * Time.fixedDeltaTime;

        _rb.velocity = transform.TransformDirection(new Vector3(horizontalSpeed, _rb.velocity.y, verticalSpeed));

        MoveCamera();
    }

    void MoveCamera()
    {
        // ���������� ������ �� ��������
        if (mainCamera != null)
        {
            // ������ ������� ������ ����� � ��� ��������
            Vector3 targetPosition = transform.position - transform.forward * 5f + Vector3.up * cameraHeight;

            // ��������� ������� ����������
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * smoothness);

            // ���������� ������ �� ������
            mainCamera.transform.LookAt(transform.position);
        }
    }
}
