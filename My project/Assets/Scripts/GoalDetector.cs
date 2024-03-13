using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    public GameObject ballPrefab; // Префаб мяча
    public int maxGoals = 3; // Максимальное количество забитых голов, после которых игра завершается
    public Color ballColorOnGameOver = Color.red; // Цвет мяча после завершения игры

    private static Vector3 initialBallPosition; // Изначальная позиция мяча
    private int goalsScored = 0; // Количество забитых голов
    private bool gameOver = false; // Флаг для отслеживания завершения игры
    private GameObject lastBall; // Последний созданный мяч

    private void Start()
    {
        // Если изначальная позиция еще не была установлена, устанавливаем ее
        if (initialBallPosition == Vector3.zero)
        {
            // Запоминаем изначальную позицию первого мяча
            initialBallPosition = GameObject.FindGameObjectWithTag("Ball").transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !gameOver) // Проверяем, что мяч еще не был забит и игра не завершена
        {
            goalsScored++; // Увеличиваем счетчик забитых голов

            // Помечаем мяч как забитый, чтобы он не считался для следующих голов
            other.gameObject.tag = "ScoredBall";

            // Спавним новый мяч на изначальной позиции первого мяча
            lastBall = Instantiate(ballPrefab, initialBallPosition, Quaternion.identity);

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
}
