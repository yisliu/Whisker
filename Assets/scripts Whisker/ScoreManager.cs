using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;
    public int pointsPerKill = 10;
    public int pointsLostOnHit = 5;

    private int score;
    public int Score => score;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        UpdateHUD();
    }

    public void AddPoints(int amount)
    {
        score += amount;
        UpdateHUD();
        WinScreenManager.Instance?.CheckWinCondition();
    }

    public void SubtractPoints(int amount)
    {
        score = Mathf.Max(0, score - amount);
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }
}