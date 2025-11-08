using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI rollsText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI ballTypeText;
    public Button resetButton;
    public Button restartButton;
    public Button changeBallButton;

    [Header("Pin Settings")]
    public GameObject pinPrefab;
    public Transform[] pinSpawnPositions;

    private int score = 0;
    private int pinsStanding;
    private int rollsInFrame = 0;
    private int[] rollScores = new int[2];
    private bool gameEnded = false;
    private List<Pin> allPins = new List<Pin>();
    private int initialPinsCount = 0;

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
        InitializeGame();
    }

    void InitializeGame()
    {
        for (int i = 0; i < rollScores.Length; i++)
        {
            rollScores[i] = 0;
        }

        CreatePins();
        pinsStanding = allPins.Count;
        initialPinsCount = allPins.Count;
        UpdateUI();
        ShowMessage("Сделайте первый бросок!");
        SetupButtons();
    }

    void CreatePins()
    {
        foreach (Pin pin in allPins)
        {
            if (pin != null) Destroy(pin.gameObject);
        }
        allPins.Clear();

        if (pinPrefab != null && pinSpawnPositions.Length > 0)
        {
            foreach (Transform spawnPos in pinSpawnPositions)
            {
                GameObject newPin = Instantiate(pinPrefab, spawnPos.position, spawnPos.rotation);
                Pin pinComponent = newPin.GetComponent<Pin>();
                if (pinComponent != null) allPins.Add(pinComponent);
            }
            Debug.Log($"Создано кеглей: {allPins.Count}");
        }
    }

    void SetupButtons()
    {
        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(ResetForSecondThrow);
            resetButton.gameObject.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
            restartButton.gameObject.SetActive(false);
        }

        if (changeBallButton != null)
        {
            changeBallButton.onClick.RemoveAllListeners();
            changeBallButton.onClick.AddListener(OnChangeBallButton);
            changeBallButton.gameObject.SetActive(true);
        }
    }

    void OnChangeBallButton()
    {
        if (BallManager.Instance != null && !gameEnded)
        {
            BallManager.Instance.ChangeBall();
            UpdateBallTypeText();
            ShowMessage($"Выбран {BallManager.Instance.GetCurrentBallName()} мяч");
        }
    }

    public void CalculateScore()
    {
        if (gameEnded) return;
        StartCoroutine(ScoreAfterDelay());
    }

    IEnumerator ScoreAfterDelay()
    {
        yield return new WaitForSeconds(3f); // Уменьшил задержку с 8 до 3 секунд
        CheckPins();
    }

    public void CheckPins()
    {
        int currentStanding = CountStandingPins();
        int pinsKnockedDown = 0;

        if (rollsInFrame == 0)
        {
            pinsKnockedDown = initialPinsCount - currentStanding;
        }
        else if (rollsInFrame == 1)
        {
            pinsKnockedDown = pinsStanding - currentStanding;
        }

        if (pinsKnockedDown < 0)
        {
            pinsKnockedDown = 0;
        }

        Debug.Log($"Бросок {rollsInFrame + 1}: было {pinsStanding}, стало {currentStanding}, сбито: {pinsKnockedDown}");

        if (rollsInFrame < rollScores.Length)
        {
            rollScores[rollsInFrame] = pinsKnockedDown;
        }

        score += pinsKnockedDown;

        bool isStrike = (rollsInFrame == 0 && pinsKnockedDown == 10);
        bool isSpare = (rollsInFrame == 1 && (rollScores[0] + pinsKnockedDown) == 10);

        rollsInFrame++;
        pinsStanding = currentStanding;

        UpdateUI();

        if (rollsInFrame == 1)
        {
            if (isStrike)
            {
                ShowMessage("СТРАЙК! Все кегли сбиты!");
                score += 10;
                UpdateUI();

                // УДАЛЯЕМ ВСЕ КЕГЛИ ПРИ СТРАЙКЕ
                RemoveAllPins();
                EndGame("СТРАЙК! Игра завершена!");
            }
            else
            {
                ShowMessage($"Первый бросок: сбито {pinsKnockedDown} кеглей. Нажмите 'Сброс' для второго броска");
                resetButton?.gameObject.SetActive(true);
            }
        }
        else if (rollsInFrame == 2)
        {
            if (isSpare)
            {
                ShowMessage("СПАР! Все кегли сбиты за два броска!");
                score += 5;
                UpdateUI();

                // УДАЛЯЕМ ВСЕ КЕГЛИ ПРИ СПАРЕ
                RemoveAllPins();
                EndGame("СПАР! Игра завершена!");
            }
            else
            {
                ShowMessage($"Второй бросок: сбито {pinsKnockedDown} кеглей. Игра завершена!");

                // УДАЛЯЕМ ВСЕ КЕГЛИ ПРИ ЗАВЕРШЕНИИ ИГРЫ
                RemoveAllPins();
                EndGame("Игра завершена!");
            }
        }
    }

    // НОВЫЙ МЕТОД: Удаление всех кеглей при завершении игры
    void RemoveAllPins()
    {
        foreach (Pin pin in allPins)
        {
            if (pin != null)
            {
                Destroy(pin.gameObject);
            }
        }
        allPins.Clear();
        pinsStanding = 0;

        Debug.Log("Все кегли удалены - игра завершена");
    }

    void EndGame(string message)
    {
        gameEnded = true;
        ShowMessage(message);
        resetButton?.gameObject.SetActive(false);
        restartButton?.gameObject.SetActive(true);
        changeBallButton?.gameObject.SetActive(false);
    }

    public void ResetForSecondThrow()
    {
        if (gameEnded || rollsInFrame != 1) return;

        RemoveKnockedDownPins();
        BallManager.Instance?.ResetCurrentBall();
        ShowMessage("Готово! Сделайте второй бросок");
        resetButton?.gameObject.SetActive(false);
    }

    void RemoveKnockedDownPins()
    {
        for (int i = allPins.Count - 1; i >= 0; i--)
        {
            Pin pin = allPins[i];
            if (pin != null && !pin.IsStanding())
            {
                Destroy(pin.gameObject);
                allPins.RemoveAt(i);
            }
        }

        pinsStanding = CountStandingPins();
        Debug.Log($"Удалены сбитые кегли. Осталось: {pinsStanding}");
    }

    public void RestartGame()
    {
        score = 0;
        rollsInFrame = 0;
        gameEnded = false;

        for (int i = 0; i < rollScores.Length; i++)
        {
            rollScores[i] = 0;
        }

        BallManager.Instance?.ResetCurrentBall();
        CreatePins();
        pinsStanding = allPins.Count;
        initialPinsCount = allPins.Count;

        UpdateUI();
        ShowMessage("Новая игра! Сделайте первый бросок");

        resetButton?.gameObject.SetActive(false);
        restartButton?.gameObject.SetActive(false);
        changeBallButton?.gameObject.SetActive(true);
    }

    void UpdateBallTypeText()
    {
        if (ballTypeText != null && BallManager.Instance != null)
        {
            ballTypeText.text = $"Мяч: {BallManager.Instance.GetCurrentBallName()}";
        }
    }

    void UpdateRollsText()
    {
        if (rollsText != null)
        {
            string rollsDisplay = "";
            for (int i = 0; i < 2; i++)
            {
                rollsDisplay += (i < rollsInFrame) ? "| " : "○ ";
            }
            rollsText.text = "Броски: " + rollsDisplay;
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Счет: " + score;
        }
    }

    void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    void UpdateUI()
    {
        UpdateScoreText();
        UpdateRollsText();
        UpdateBallTypeText();
    }

    private int CountStandingPins()
    {
        int count = 0;
        foreach (Pin pin in allPins)
        {
            if (pin != null && pin.IsStanding())
            {
                count++;
            }
        }
        return count;
    }
}