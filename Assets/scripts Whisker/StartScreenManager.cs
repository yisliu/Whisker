using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    [SerializeField] private string levelSceneName = "level 1";

    public void StartGame()
    {
        SceneManager.LoadScene(levelSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}