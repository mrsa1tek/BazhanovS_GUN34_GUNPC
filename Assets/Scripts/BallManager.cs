using UnityEngine;

public class BallManager : MonoBehaviour
{
    public static BallManager Instance;

    [Header("Ball Settings")]
    public GameObject[] ballPrefabs;
    public Transform ballSpawnPoint;
    public Transform throwPoint;

    [Header("Ball Properties")]
    public float[] forceMultipliers = { 120f, 100f, 80f };
    public float[] massValues = { 1f, 2f, 3f };

    private GameObject currentBall;
    private int currentBallIndex = 0;
    private BallController currentBallController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CreateBall(0);
    }

    public void CreateBall(int ballIndex)
    {
        // Удаляем старый мяч
        if (currentBall != null)
        {
            Destroy(currentBall);
        }

        if (ballPrefabs.Length > ballIndex && ballPrefabs[ballIndex] != null)
        {
            // Создаем новый мяч
            currentBall = Instantiate(ballPrefabs[ballIndex], ballSpawnPoint.position, Quaternion.identity);
            currentBallIndex = ballIndex;

            // Настраиваем компоненты мяча
            SetupBallComponents(currentBall);

            Debug.Log($"Создан мяч типа #{ballIndex + 1}");
        }
    }

    private void SetupBallComponents(GameObject ball)
    {
        // Добавляем или получаем компонент BallController
        currentBallController = ball.GetComponent<BallController>();
        if (currentBallController == null)
        {
            currentBallController = ball.AddComponent<BallController>();
        }

        // Настраиваем параметры мяча
        currentBallController.forceMultiplier = forceMultipliers[currentBallIndex];
        currentBallController.throwPoint = throwPoint;

        // Настраиваем Rigidbody
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = massValues[currentBallIndex];
        }

        // Добавляем коллайдер если нет
        SphereCollider collider = ball.GetComponent<SphereCollider>();
        if (collider == null)
        {
            collider = ball.AddComponent<SphereCollider>();
        }
    }

    public void ChangeBall()
    {
        int newBallIndex = (currentBallIndex + 1) % ballPrefabs.Length;
        CreateBall(newBallIndex);
    }

    public void ResetCurrentBall()
    {
        if (currentBallController != null)
        {
            currentBallController.ResetBall();
        }
    }

    public string GetCurrentBallName()
    {
        string[] ballNames = { "Легкий", "Средний", "Тяжелый" };
        return currentBallIndex < ballNames.Length ? ballNames[currentBallIndex] : $"Тип {currentBallIndex + 1}";
    }

    public int GetCurrentBallIndex()
    {
        return currentBallIndex;
    }

    public bool CanControlBall()
    {
        return currentBallController != null && currentBallController.CanControl();
    }
}