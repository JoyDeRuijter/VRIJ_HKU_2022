using UnityEngine;
using UnityEngine.SceneManagement;

public class UserInterface : MonoBehaviour
{
    private bool gameIsPaused;
    [SerializeField] GameObject PauseMenuObject;
    [SerializeField] GameObject PlayUIObject;

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
            PlayUIObject.SetActive(false);
        }
        else
        {
            // play rules
            Time.timeScale = 1;
            PauseMenuObject.SetActive(false);
            PlayUIObject.SetActive(true);
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

    public void Move()
    {
        Character character = FindObjectOfType<Character>();
        if (character != null) character.MoveNext();
    }
}
