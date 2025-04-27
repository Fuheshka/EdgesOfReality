using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private GameObject inGameMenuPanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button backButton;
    [SerializeField] private InventoryUI inventoryUI;

    private Animator inGameMenuAnimator;
    private Animator optionsAnimator;
    private GameObject optionsPanel;
    private const float animationDuration = 0.5f;
    private bool isPaused = false;

    private void Start()
    {
        if (inGameMenuPanel == null)
        {
            Debug.LogError("InGameMenuPanel is not assigned!");
            return;
        }

        inGameMenuAnimator = inGameMenuPanel.GetComponent<Animator>();
        if (inGameMenuAnimator == null)
        {
            Debug.LogError("Animator not found on InGameMenuPanel!");
        }
        else if (inGameMenuAnimator.runtimeAnimatorController == null)
        {
            Debug.LogError("AnimatorController not assigned to InGameMenuPanel!");
        }

        optionsPanel = OptionsManager.Instance?.GetOptionsPanel();
        if (optionsPanel == null)
        {
            Debug.LogError("OptionsPanel not found in OptionsManager!");
            return;
        }

        optionsAnimator = optionsPanel.GetComponent<Animator>();
        if (optionsAnimator == null)
        {
            Debug.LogError("Animator not found on OptionsPanel!");
        }
        else if (optionsAnimator.runtimeAnimatorController == null)
        {
            Debug.LogError("AnimatorController not assigned to OptionsPanel!");
        }

        if (inventoryUI == null)
        {
            Debug.LogError("InventoryUI is not assigned in InGameMenu!");
        }

        continueButton.onClick.AddListener(ContinueGame);
        settingsButton.onClick.AddListener(OpenSettings);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
        backButton.onClick.AddListener(BackToInGameMenu);

        if (backButton == null)
        {
            Debug.LogError("BackButton is not assigned in InGameMenu!");
        }
        else
        {
            Debug.Log("BackButton assigned: " + backButton.name);
        }

        inGameMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape pressed, isPaused: " + isPaused);
            if (!isPaused)
            {
                PauseGame();
            }
            else if (inGameMenuPanel.activeSelf)
            {
                ContinueGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            // Вариант 1: Запрещаем открытие/закрытие инвентаря, если другое меню активно
            if (inGameMenuPanel.activeSelf || optionsPanel.activeSelf)
            {
                Debug.Log("Cannot toggle Inventory: Another menu is active!");
                return;
            }

            if (inventoryUI.gameObject.activeSelf)
            {
                inventoryUI.Hide();
            }
            else
            {
                inventoryUI.Show();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) Inventory.Instance.AddItem("Sandwich");
        if (Input.GetKeyDown(KeyCode.Alpha2)) Inventory.Instance.AddItem("Coffee");
        if (Input.GetKeyDown(KeyCode.Alpha3)) Inventory.Instance.AddItem("Cake");
        if (Input.GetKeyDown(KeyCode.Alpha4)) Inventory.Instance.AddItem("Knife");
    }

    private void PauseGame()
    {
        Debug.Log("Pausing game");
        // Вариант 2: Автоматически закрываем инвентарь при открытии InGameMenuPanel
        if (inventoryUI.gameObject.activeSelf)
        {
            inventoryUI.Hide();
        }

        inGameMenuPanel.SetActive(true);
        if (inGameMenuAnimator != null)
        {
            inGameMenuAnimator.ResetTrigger("FadeOut");
            inGameMenuAnimator.SetTrigger("FadeIn");
        }
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("InGameMenuPanel active: " + inGameMenuPanel.activeSelf);
    }

    private void ContinueGame()
    {
        StartCoroutine(CloseInGameMenu());
    }

    private void OpenSettings()
    {
        StartCoroutine(SwitchToSettings());
    }

    private void BackToInGameMenu()
    {
        StartCoroutine(SwitchToInGameMenu());
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    private IEnumerator CloseInGameMenu()
    {
        if (inGameMenuAnimator != null)
        {
            inGameMenuAnimator.ResetTrigger("FadeIn");
            inGameMenuAnimator.SetTrigger("FadeOut");
        }
        yield return new WaitForSecondsRealtime(animationDuration);
        inGameMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    private IEnumerator SwitchToSettings()
    {
        if (inGameMenuAnimator != null)
        {
            inGameMenuAnimator.ResetTrigger("FadeIn");
            inGameMenuAnimator.SetTrigger("FadeOut");
        }
        yield return new WaitForSecondsRealtime(animationDuration);
        inGameMenuPanel.SetActive(false);
        // Вариант 2: Автоматически закрываем инвентарь при открытии OptionsPanel
        if (inventoryUI.gameObject.activeSelf)
        {
            inventoryUI.Hide();
        }
        optionsPanel.SetActive(true);
        if (optionsAnimator != null)
        {
            optionsAnimator.ResetTrigger("FadeOut");
            optionsAnimator.SetTrigger("FadeIn");
        }
    }

    private IEnumerator SwitchToInGameMenu()
    {
        Debug.Log("SwitchToInGameMenu called");
        if (optionsAnimator != null)
        {
            optionsAnimator.ResetTrigger("FadeIn");
            optionsAnimator.SetTrigger("FadeOut");
        }
        else
        {
            Debug.LogWarning("OptionsAnimator is null, skipping animation");
            optionsPanel.SetActive(false);
            inGameMenuPanel.SetActive(true);
            yield break;
        }
        yield return new WaitForSecondsRealtime(animationDuration);
        optionsPanel.SetActive(false);
        inGameMenuPanel.SetActive(true);
        if (inGameMenuAnimator != null)
        {
            inGameMenuAnimator.ResetTrigger("FadeOut");
            inGameMenuAnimator.SetTrigger("FadeIn");
        }
        Debug.Log("SwitchToInGameMenu completed");
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}