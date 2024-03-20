using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    private GameObject heldObject;
    private bool isHoldingObject = false;
    private float objectHeightOffset = 0.1f; // �������� ������ ������� ��� ������
    private float pickupRadius = 0.01f; // ������ ������� ��������

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isHoldingObject)
            {
                ReleaseObject();
            }
            else
            {
                PickObjectFromFloor();
            }
        }

        if (isHoldingObject && heldObject != null) // ��������� �������� �� null
        {
            // ��������� ������� �������, �������� �������� ������
            heldObject.transform.position = transform.position + transform.forward * 2f + transform.up * objectHeightOffset;
        }
    }

    void PickObjectFromFloor()
    {
        RaycastHit hit;

        // �������� ����������� ���, ����� ���������� ������� �� ���� ������ ������
        if (Physics.SphereCast(transform.position, pickupRadius, transform.forward, out hit, 2f))
        {
            if (hit.collider.CompareTag("Ball"))
            {
                // ���� ������ ������, ��������� ���
                heldObject = hit.collider.gameObject;
                heldObject.transform.parent = transform;
                isHoldingObject = true;
            }
        }
    }

    void ReleaseObject()
    {
        if (heldObject != null) // ��������� �������� �� null
        {
            // ������������� ������� �������� null, ����� �� �������� ���� �������� �������� ������
            heldObject.transform.parent = null;

            // ���������� ������
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            heldObject = null;
            isHoldingObject = false;
        }
    }
}
