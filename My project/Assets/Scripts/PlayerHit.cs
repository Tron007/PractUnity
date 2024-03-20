using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    private float hitForce = 10f; // ���� �����
    private float hitRadius = 1f; // ������ �����

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            // ����� �������� � ��������� �������
            Collider[] colliders = Physics.OverlapSphere(transform.position, hitRadius);

            // ��������� ������ ��������� ������
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Ball"))
                {
                    // ��������� ���� ����� � ����
                    Rigidbody ballRigidbody = collider.GetComponent<Rigidbody>();
                    Vector3 direction = collider.transform.position - transform.position;
                    ballRigidbody.AddForce(direction.normalized * hitForce, ForceMode.Impulse);
                    break; // ������� �� ����� ����� ����� �� ������� ����
                }
            }
        }
    }
}
