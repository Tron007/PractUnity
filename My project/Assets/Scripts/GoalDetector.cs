using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    public GameObject ballPrefab; // ������ ����
    public int maxGoals = 3; // ������������ ���������� ������� �����, ����� ������� ���� �����������
    public Color ballColorOnGameOver = Color.red; // ���� ���� ����� ���������� ����

    private static Vector3 initialBallPosition; // ����������� ������� ����
    private int goalsScored = 0; // ���������� ������� �����
    private bool gameOver = false; // ���� ��� ������������ ���������� ����
    private GameObject lastBall; // ��������� ��������� ���

    private void Start()
    {
        // ���� ����������� ������� ��� �� ���� �����������, ������������� ��
        if (initialBallPosition == Vector3.zero)
        {
            // ���������� ����������� ������� ������� ����
            initialBallPosition = GameObject.FindGameObjectWithTag("Ball").transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !gameOver) // ���������, ��� ��� ��� �� ��� ����� � ���� �� ���������
        {
            goalsScored++; // ����������� ������� ������� �����

            // �������� ��� ��� �������, ����� �� �� �������� ��� ��������� �����
            other.gameObject.tag = "ScoredBall";

            // ������� ����� ��� �� ����������� ������� ������� ����
            lastBall = Instantiate(ballPrefab, initialBallPosition, Quaternion.identity);

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
}
