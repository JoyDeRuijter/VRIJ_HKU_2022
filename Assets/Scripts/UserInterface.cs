using UnityEngine;
using UnityEngine.SceneManagement;

public class UserInterface : MonoBehaviour
{
    private bool gameIsPaused;
    [SerializeField] GameObject PauseMenuObject;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused)
            gameIsPaused = true;
        else if (Input.GetKeyDown(KeyCode.Escape) && gameIsPaused)
            gameIsPaused = false;

        if (gameIsPaused)
        {
            // pause rules
            Time.timeScale = 0f;
            PauseMenuObject.SetActive(true);
        }
        else
        {
            // play rules
            Time.timeScale = 1;
            PauseMenuObject.SetActive(false);
        }

    }

    public void ContinueButton()
    {
        gameIsPaused = false;
    }

    public void RestartButton()
    {
        reloadScene();
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public static void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
