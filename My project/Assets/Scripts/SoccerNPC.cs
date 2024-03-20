using UnityEngine;

public class SoccerNPC : MonoBehaviour
{
    public Transform ball; // Мяч
    public Transform goal; // Ворота, куда NPC будет стремиться забить гол
    public Transform player; // Игрок
    public float dribbleDistance = 2f; // Расстояние, на котором NPC будет дриблить мяч
    public float kickForce = 10f; // Сила удара мяча

    private UnityEngine.AI.NavMeshAgent agent; // Компонент для перемещения NPC
    private bool isKicking = false; // Флаг для обозначения совершения удара
    private GoalDetector goalDetector; // Переменная для доступа к GoalDetector

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.enabled = true;
        if (ball == null || goal == null || player == null)
        {
            Debug.LogError("Ball, Goal, or Player is not assigned to NPC!");
            enabled = false; // Отключаем скрипт, если не все объекты назначены
        }

        // Получаем доступ к GoalDetector
        goalDetector = FindObjectOfType<GoalDetector>();
    }

    void Update()
    {
        if (!goalDetector.IsGameOver()) // Проверяем, не завершена ли игра
        {
            // Если мяч ближе, чем дриблировать
            if (Vector3.Distance(transform.position, ball.position) <= dribbleDistance)
            {
                DribbleBall();
            }
            else // Иначе двигаемся к мячу
            {
                agent.SetDestination(ball.position);
            }

            // Если NPC достаточно близко к мячу, попытаться забить гол
            if (Vector3.Distance(transform.position, ball.position) <= dribbleDistance && !isKicking)
            {
                KickBall();
            }
        }
        else // Если игра завершена, остановить NPC в центре поля
        {
            // Остановить движение NPC
            agent.isStopped = true;

            // Переместить NPC в центр поля
            agent.SetDestination(ball.position);
        }
    }

    public void DribbleBall()
    {
        // Указываем агенту двигаться к позиции мяча
        agent.SetDestination(ball.position);
    }

    void KickBall()
    {
        // Ориентируем мяч в сторону ворот
        Vector3 goalDirection = (goal.position - ball.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(ball.position, goalDirection, out hit))
        {
            if (hit.transform == goal)
            {
                ball.GetComponent<Rigidbody>().AddForce(goalDirection * kickForce, ForceMode.Impulse);
                isKicking = true;
                // Ждем некоторое время перед тем, как снова пытаться забить гол
                Invoke("ResetKick", 2.0f);
                return;
            }
        }

        // Если направление к воротам недоступно, обойти мяч
        Vector3 newDestination = transform.position + Vector3.Cross(Vector3.up, goalDirection).normalized * dribbleDistance * 2f;
        agent.SetDestination(newDestination);
    }

    void ResetKick()
    {
        isKicking = false;
    }
}
