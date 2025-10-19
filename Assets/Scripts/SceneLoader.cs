using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool IsPaused;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(pauseMenu != null)
            ResumeGame();
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseMenu != null)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(IsPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
    }

    [Tooltip("The build index of the scene to load.")]
    public int sceneIndex;

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneIndex);
        ResumeGame();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in editor
#else
        Application.Quit(); // Quit the built game
#endif
    }

    public void PauseGame()
    {
     pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void ResetLevel()
    {
        // Get the active scene (the one you're currently in)
        Scene currentScene = SceneManager.GetActiveScene();
        Time.timeScale = 1f;
        IsPaused = false;

        // Reload it by name
        SceneManager.LoadScene(currentScene.name);
        ResumeGame();
    }
}
