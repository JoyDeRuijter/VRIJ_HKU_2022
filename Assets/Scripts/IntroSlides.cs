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

    [SerializeField] private Button backButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button goButton;
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] slides;
    [SerializeField] private GameObject[] texts;
    [SerializeField] private float fadeTime = 2f;

    private int currentSlide;
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

        if (currentSlide > 0 && currentSlide < 11)
        { 
            backButton.interactable = true;
            nextButton.interactable = true;
            nextButton.gameObject.SetActive(true);
            goButton.interactable = false;
            goButton.gameObject.SetActive(false);
        }

        if (currentSlide == 11)
        { 
            backButton.interactable = true;
            nextButton.interactable = false;
            nextButton.gameObject.SetActive(false);
            goButton.interactable = true;
            goButton.gameObject.SetActive(true);
        }
    }
}