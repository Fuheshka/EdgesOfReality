using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button backButton;

    private Animator mainMenuAnimator;
    private Animator optionsAnimator;
    private const string VolumeKey = "MasterVolume";
    private const float animationDuration = 0.5f;

    private void Start()
    {
        mainMenuAnimator = mainMenuPanel.GetComponent<Animator>();
        optionsAnimator = optionsPanel.GetComponent<Animator>();

        startButton.onClick.AddListener(StartGame);
        optionsButton.onClick.AddListener(OpenOptions);
        exitButton.onClick.AddListener(ExitGame);
        backButton.onClick.AddListener(BackToMainMenu);

        volumeSlider.onValueChanged.AddListener(SetVolume);

        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);

        mainMenuAnimator.ResetTrigger("FadeOut");
        mainMenuAnimator.SetTrigger("FadeIn");

        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.5f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
    }

    private void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void OpenOptions()
    {
        StartCoroutine(SwitchToOptions());
    }

    private void BackToMainMenu()
    {
        StartCoroutine(SwitchToMainMenu());
    }

    private IEnumerator SwitchToOptions()
    {
        Debug.Log("Starting FadeOut for MainMenu");
        mainMenuAnimator.ResetTrigger("FadeIn");
        mainMenuAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(animationDuration);
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        Debug.Log("Starting FadeIn for Options");
        optionsAnimator.ResetTrigger("FadeOut");
        optionsAnimator.SetTrigger("FadeIn");
    }

    private IEnumerator SwitchToMainMenu()
    {
        Debug.Log("Starting FadeOut for Options");
        optionsAnimator.ResetTrigger("FadeIn");
        optionsAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(animationDuration);
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        Debug.Log("Starting FadeIn for MainMenu");
        mainMenuAnimator.ResetTrigger("FadeOut");
        mainMenuAnimator.SetTrigger("FadeIn");
    }

    private void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }

    private void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}