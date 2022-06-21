using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroSlides : MonoBehaviour
{
    #region Variables

    [SerializeField] private bool isOutro;
    [SerializeField] private Button backButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button goButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] slides;
    [SerializeField] private GameObject[] texts;
    [SerializeField] private float fadeTime = 0.5f;

    private int currentSlide = 0;
    private GameObject currentText;

    #endregion

    private void Awake()
    {
        backButton.interactable = false;
        goButton.interactable = false;
        goButton.gameObject.SetActive(false);
        image.sprite = slides[currentSlide];
        FadeInNewText();
    }

    private IEnumerator Fade()
    {
        image.CrossFadeAlpha(0, fadeTime, false);
        StartCoroutine(FadeOutOldText());
        yield return new WaitUntil(HasFadedOutSlide);
        image.sprite = slides[currentSlide];
        image.CrossFadeAlpha(1, fadeTime, false);
        FadeInNewText();
    }

    private bool HasFadedOutSlide()
    {
        if (image.canvasRenderer.GetAlpha() == 0f)
            return true;
        else
            return false;
    }

    public void NextSlide()
    {
        currentSlide++;
        SetButtons();

        if (currentSlide >= slides.Length)
            currentSlide = 11;
        StartCoroutine(Fade());
    }

    public void PreviousSlide()
    {
        currentSlide--;
        SetButtons();

        if (currentSlide < 0)
            currentSlide = 0;
        StartCoroutine(Fade());
    }

    public void GO(string sceneName)
    { 
        SceneManager.LoadScene(sceneName);
    }

    public void Close()
    { 
        Application.Quit();
    }

    private IEnumerator FadeOutOldText()
    {
        if (currentText != null)
        { 
            currentText.GetComponent<TextMeshProUGUI>().CrossFadeAlpha(0, fadeTime, false);
            yield return new WaitForSeconds(fadeTime - 0.05f);
            Destroy(currentText);
        }
        yield return null;
    }

    private void FadeInNewText()
    {
        currentText = Instantiate(texts[currentSlide], this.transform);
        currentText.GetComponent<TextMeshProUGUI>().CrossFadeAlpha(0, 0f, false);
        currentText.GetComponent<TextMeshProUGUI>().CrossFadeAlpha(1, fadeTime, false);
    }

    private void SetButtons()
    {
        if (currentSlide == 0)
        { 
            backButton.interactable = false;
            nextButton.interactable = true;
            nextButton.gameObject.SetActive(true);
            goButton.interactable = false;
            goButton.gameObject.SetActive(false);
        }

        if (currentSlide > 0 && currentSlide < 4 && isOutro)
        {
            closeButton.interactable = false;
            closeButton.gameObject.SetActive(false);
        }

        if (currentSlide > 0 && currentSlide < 11)
        { 
            backButton.interactable = true;
            nextButton.interactable = true;
            nextButton.gameObject.SetActive(true);
            goButton.interactable = false;
            goButton.gameObject.SetActive(false);
        }

        if (currentSlide == 11 && !isOutro)
        { 
            backButton.interactable = true;
            nextButton.interactable = false;
            nextButton.gameObject.SetActive(false);
            goButton.interactable = true;
            goButton.gameObject.SetActive(true);
        }

        if (currentSlide == 3 && isOutro)
        {
            backButton.interactable = true;
            nextButton.interactable = false;
            nextButton.gameObject.SetActive(false);
            closeButton.interactable = true;
            closeButton.gameObject.SetActive(true);
        }
    }
}