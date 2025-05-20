using UnityEngine;

public class InGameMenuLoader : MonoBehaviour
{
    private void Awake()
    {
        if (InGameMenu.Instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/InGameMenuManager");
            if (prefab != null)
            {
                Instantiate(prefab);
            }
            else
            {
                Debug.LogError("InGameMenuManager prefab not found in Resources/Prefabs!");
            }
        }
    }
}
