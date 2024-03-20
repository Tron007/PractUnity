using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab; // ������ ����
    [SerializeField] private int maxGoals = 3; // ������������ ���������� ������� �����, ����� ������� ���� �����������
    [SerializeField] private Color ballColorOnGameOver = Color.red; // ���� ���� ����� ���������� ����

    private static Vector3 initialBallPosition; // ����������� ������� ����
    private GameObject lastBall; // ��������� ��������� ���
    private int goalsScored = 0; // ���������� ������� �����
    private bool gameOver = false; // ���� ��� ������������ ���������� ����
    private SoccerNPC soccerNPC; // ������ �� ������ SoccerNPC

    private void Start()
    {
        // ���������� ����������� ������� ������� ����
        initialBallPosition = GameObject.FindGameObjectWithTag("Ball").transform.position;
        soccerNPC = FindObjectOfType<SoccerNPC>(); // ������� ������ SoccerNPC
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !gameOver) // ���������, ��� ��� ��� �� ��� ����� � ���� �� ���������
        {
            goalsScored++; // ����������� ������� ������� �����

            // ���������� ���������� ���
            Destroy(other.gameObject);

            // ������� ����� ��� �� ����������� ������� ����������� ����
            lastBall = Instantiate(ballPrefab, initialBallPosition, Quaternion.identity);

            // ��������� ������� ���� � ������� SoccerNPC
            if (soccerNPC != null)
            {
                soccerNPC.ball = lastBall.transform;
                soccerNPC.DribbleBall(); // �������� ��������� � ������ ����
            }

            if (goalsScored >= maxGoals)
            {
                gameOver = true; // ���� ���������� ������������ ���������� �����, ��������� ����

                // ���� ��������� ��������� ��� ����������, ������ ��� ����
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
