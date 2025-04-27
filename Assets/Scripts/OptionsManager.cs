using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    private static OptionsManager instance;
    public static OptionsManager Instance
    {
        get
        {
            if (instance == null)
            {
                // »щем существующий OptionsManager в сцене
                instance = FindObjectOfType<OptionsManager>();
                if (instance == null)
                {
                    // ≈сли не нашли, создаЄм новый из префаба
                    GameObject prefab = Resources.Load<GameObject>("Prefabs/OptionsManager");
                    if (prefab == null)
                    {
                        Debug.LogError("OptionsManager prefab not found in Resources/Prefabs! Please create a prefab and place it in Resources/Prefabs.");
                        return null;
                    }
                    GameObject managerObj = Instantiate(prefab);
                    instance = managerObj.GetComponent<OptionsManager>();
                    if (instance == null)
                    {
                        Debug.LogError("OptionsManager component not found on prefab!");
                        return null;
                    }
                    instance.name = "OptionsManager";
                    DontDestroyOnLoad(instance.gameObject);
                    Debug.Log("Created new OptionsManager instance");
                }
            }
            return instance;
        }
    }

    [SerializeField] private GameObject optionsPanelPrefab; // —сылка на префаб OptionsPanel
    private GameObject optionsPanelInstance; // Ёкземпл€р OptionsPanel в текущей сцене

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("OptionsManager initialized");
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        FindOrCreateOptionsPanel();
    }

    private void FindOrCreateOptionsPanel()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        if (canvases.Length == 0)
        {
            Debug.LogError("No Canvas found in the scene! OptionsPanel cannot be displayed.");
            return;
        }

        Canvas mainCanvas = canvases[0];
        foreach (Canvas canvas in canvases)
        {
            if (canvas.gameObject.name.Contains("UI Canvas"))
            {
                mainCanvas = canvas;
                break;
            }
        }

        int existingPanelsCount = 0;
        GameObject existingPanel = null;
        foreach (Transform child in mainCanvas.transform)
        {
            if (child.name == "OptionsPanel" || child.name.StartsWith("OptionsPanel(Clone)"))
            {
                existingPanelsCount++;
                if (existingPanel == null)
                {
                    existingPanel = child.gameObject;
                }
            }
        }

        if (existingPanelsCount > 1)
        {
            foreach (Transform child in mainCanvas.transform)
            {
                if (child.name == "OptionsPanel" || child.name.StartsWith("OptionsPanel(Clone)"))
                {
                    if (child.gameObject != existingPanel)
                    {
                        Destroy(child.gameObject);
                        Debug.Log("Destroyed duplicate OptionsPanel: " + child.name);
                    }
                }
            }
        }

        if (existingPanel != null)
        {
            optionsPanelInstance = existingPanel;
            Debug.Log("Found existing OptionsPanel in scene: " + optionsPanelInstance.name);
        }
        else
        {
            if (optionsPanelPrefab == null)
            {
                Debug.LogError("OptionsPanelPrefab is not assigned in OptionsManager!");
                return;
            }

            optionsPanelInstance = Instantiate(optionsPanelPrefab, mainCanvas.transform);
            optionsPanelInstance.name = "OptionsPanel";
            Debug.Log("Created new OptionsPanel instance in scene: " + optionsPanelInstance.name);
        }

        if (optionsPanelInstance != null)
        {
            optionsPanelInstance.SetActive(false);
        }
    }

    public GameObject GetOptionsPanel()
    {
        if (optionsPanelInstance == null)
        {
            Debug.LogError("OptionsPanelInstance is null! Trying to find or create it.");
            FindOrCreateOptionsPanel();
        }
        return optionsPanelInstance;
    }
}