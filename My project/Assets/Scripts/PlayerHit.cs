using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    private float hitForce = 10f; // Сила удара
    private float hitRadius = 1f; // Радиус удара

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            // Поиск объектов в указанном радиусе
            Collider[] colliders = Physics.OverlapSphere(transform.position, hitRadius);

            // Проверяем каждый найденный объект
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Ball"))
                {
                    // Применяем силу удара к мячу
                    Rigidbody ballRigidbody = collider.GetComponent<Rigidbody>();
                    Vector3 direction = collider.transform.position - transform.position;
                    ballRigidbody.AddForce(direction.normalized * hitForce, ForceMode.Impulse);
                    break; // Выходим из цикла после удара по первому мячу
                }
            }
        }
    }
}
