using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    private static OptionsManager instance;
    public static OptionsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<OptionsManager>();
                if (instance == null)
                {
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

    [SerializeField] private GameObject optionsPanelPrefab; // Ссылка на префаб OptionsPanel
    private GameObject optionsPanelInstance; // Экземпляр OptionsPanel в текущей сцене

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
        // Находим Canvas в сцене
        Canvas mainCanvas = null;
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        if (canvases.Length > 0)
        {
            mainCanvas = canvases[0];
            foreach (Canvas canvas in canvases)
            {
                if (canvas.gameObject.name.Contains("UI Canvas"))
                {
                    mainCanvas = canvas;
                    break;
                }
            }
        }
        else
        {
            Debug.LogWarning("No Canvas found in the scene! Creating a new one.");
            GameObject canvasObj = new GameObject("UI Canvas");
            mainCanvas = canvasObj.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Проверяем, существует ли OptionsPanel
        if (optionsPanelInstance != null && optionsPanelInstance.transform.parent == mainCanvas.transform)
        {
            Debug.Log("OptionsPanel already exists in scene: " + optionsPanelInstance.name);
            return;
        }

        // Пытаемся найти существующий OptionsPanel в Canvas
        foreach (Transform child in mainCanvas.transform)
        {
            if (child.name == "OptionsPanel" || child.name.StartsWith("OptionsPanel(Clone)"))
            {
                optionsPanelInstance = child.gameObject;
                Debug.Log("Found existing OptionsPanel in scene: " + optionsPanelInstance.name);
                DontDestroyOnLoad(optionsPanelInstance);
                optionsPanelInstance.SetActive(false);
                return;
            }
        }

        // Если не нашли, создаём новый OptionsPanel
        if (optionsPanelPrefab == null)
        {
            Debug.LogWarning("OptionsPanelPrefab is not assigned in OptionsManager! Attempting to load from Resources.");
            optionsPanelPrefab = Resources.Load<GameObject>("Prefabs/OptionsPanel");
            if (optionsPanelPrefab == null)
            {
                Debug.LogError("Could not find OptionsPanel prefab in Resources/Prefabs/OptionsPanel! Please assign the prefab or place it in Resources/Prefabs.");
                return;
            }
        }

        optionsPanelInstance = Instantiate(optionsPanelPrefab, mainCanvas.transform);
        optionsPanelInstance.name = "OptionsPanel";
        DontDestroyOnLoad(optionsPanelInstance);
        optionsPanelInstance.SetActive(false);
        Debug.Log("Created new OptionsPanel instance in scene: " + optionsPanelInstance.name);
    }

    public GameObject GetOptionsPanel()
    {
        if (optionsPanelInstance == null)
        {
            Debug.LogWarning("OptionsPanelInstance is null! Trying to find or create it.");
            FindOrCreateOptionsPanel();
        }

        if (optionsPanelInstance == null)
        {
            Debug.LogError("Failed to create or find OptionsPanel!");
        }

        return optionsPanelInstance;
    }
}