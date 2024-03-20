using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab; // Префаб мяча
    [SerializeField] private int maxGoals = 3; // Максимальное количество забитых голов, после которых игра завершается
    [SerializeField] private Color ballColorOnGameOver = Color.red; // Цвет мяча после завершения игры

    private static Vector3 initialBallPosition; // Изначальная позиция мяча
    private GameObject lastBall; // Последний созданный мяч
    private int goalsScored = 0; // Количество забитых голов
    private bool gameOver = false; // Флаг для отслеживания завершения игры
    private SoccerNPC soccerNPC; // Ссылка на скрипт SoccerNPC

    private void Start()
    {
        // Запоминаем изначальную позицию первого мяча
        initialBallPosition = GameObject.FindGameObjectWithTag("Ball").transform.position;
        soccerNPC = FindObjectOfType<SoccerNPC>(); // Находим скрипт SoccerNPC
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !gameOver) // Проверяем, что мяч еще не был забит и игра не завершена
        {
            goalsScored++; // Увеличиваем счетчик забитых голов

            // Уничтожаем предыдущий мяч
            Destroy(other.gameObject);

            // Спавним новый мяч на изначальной позиции предыдущего мяча
            lastBall = Instantiate(ballPrefab, initialBallPosition, Quaternion.identity);

            // Обновляем позицию мяча в скрипте SoccerNPC
            if (soccerNPC != null)
            {
                soccerNPC.ball = lastBall.transform;
                soccerNPC.DribbleBall(); // Начинаем двигаться к новому мячу
            }

            if (goalsScored >= maxGoals)
            {
                gameOver = true; // Если достигнуто максимальное количество голов, завершаем игру

                // Если последний созданный мяч существует, меняем его цвет
                if (lastBall != null)
                {
                    Renderer ballRenderer = lastBall.GetComponent<Renderer>();
                    if (ballRenderer != null && ballRenderer.material != null)
                    {
                        ballRenderer.material.color = ballColorOnGameOver;
                    }
                }
            }
        }
    }

    public bool IsGameOver()
    {
        return gameOver;
    }
}
