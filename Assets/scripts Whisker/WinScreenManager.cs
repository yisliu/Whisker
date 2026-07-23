using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WinScreenManager : MonoBehaviour
{
    public static WinScreenManager Instance { get; private set; }

    public enum WinCondition { KillCount, Score, Either }

    [SerializeField] private GameObject winPanel;
    [SerializeField] private string mainMenuScene = "Start Screen";
    [SerializeField] private Button nextLevelButton;
#if UNITY_EDITOR
    [SerializeField] private SceneAsset nextSceneAsset;
    private void OnValidate()
    {
        if (nextSceneAsset != null)
            nextScene = nextSceneAsset.name;
    }
#endif
    [SerializeField] private string nextScene = "";

    [Header("Win Condition")]
    [SerializeField] private WinCondition winCondition = WinCondition.KillCount;
    [Tooltip("Required kills to win (used by KillCount and Either modes)")]
    [SerializeField] private int targetKills = 0;
    [Tooltip("Required score to win (used by Score and Either modes)")]
    [SerializeField] private int targetPoints = 0;

    private int enemiesKilled = 0;
    private bool hasWon = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (winPanel != null) winPanel.SetActive(false);
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        CheckWinCondition();
    }

    public void CheckWinCondition()
    {
        if (hasWon) return;

        bool killsMet = targetKills > 0 && enemiesKilled >= targetKills;
        bool scoreMet = targetPoints > 0 && ScoreManager.Instance != null && ScoreManager.Instance.Score >= targetPoints;

        bool won;
        switch (winCondition)
        {
            case WinCondition.KillCount: won = killsMet; break;
            case WinCondition.Score:     won = scoreMet; break;
            default:                     won = killsMet || scoreMet; break; // Either
        }

        if (won) ShowWin();
    }

    private void ShowWin()
    {
        hasWon = true;
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        Time.timeScale = 0f;
        if (nextLevelButton != null)
        {
            nextLevelButton.interactable = !string.IsNullOrEmpty(nextScene);
        }
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextScene);
    }
}