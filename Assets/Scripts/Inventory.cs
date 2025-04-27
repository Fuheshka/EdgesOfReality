using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    private static Inventory instance;
    public static Inventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Inventory>();
                if (instance == null)
                {
                    GameObject prefab = Resources.Load<GameObject>("Prefabs/InventoryManager");
                    if (prefab == null)
                    {
                        Debug.LogError("InventoryManager prefab not found in Resources/Prefabs!");
                        return null;
                    }
                    GameObject managerObj = Instantiate(prefab);
                    instance = managerObj.GetComponent<Inventory>();
                    instance.name = "InventoryManager";
                    DontDestroyOnLoad(instance.gameObject);
                    Debug.Log("Created new InventoryManager instance");
                }
            }
            return instance;
        }
    }

    private Dictionary<string, int> items = new Dictionary<string, int>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("InventoryManager initialized");
    }

    public void AddItem(string itemName)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName]++;
        }
        else
        {
            items[itemName] = 1;
        }
        Debug.Log($"Added {itemName} to inventory. Count: {items[itemName]}");
        UpdateInventoryUI();
    }

    public void RemoveItem(string itemName)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName]--;
            if (items[itemName] <= 0)
            {
                items.Remove(itemName);
            }
            Debug.Log($"Removed {itemName} from inventory. Count: {(items.ContainsKey(itemName) ? items[itemName] : 0)}");
            UpdateInventoryUI();
        }
    }

    public void UseItem(string itemName)
    {
        if (items.ContainsKey(itemName) && items[itemName] > 0)
        {
            RemoveItem(itemName);

            // ָשול PlayerEffects גלוסעמ GroundedCharacterController
            PlayerEffects playerEffects = FindObjectOfType<PlayerEffects>();
            if (playerEffects == null)
            {
                Debug.LogError("PlayerEffects not found in scene!");
                return;
            }

            switch (itemName)
            {
                case "Sandwich":
                    playerEffects.RestoreHealth(20);
                    Debug.Log("Used Sandwich: Restored 20 health");
                    break;
                case "Coffee":
                    playerEffects.BoostSpeed(1.5f, 10f);
                    Debug.Log("Used Coffee: Speed boosted by 50% for 10 seconds");
                    break;
                case "Cake":
                    playerEffects.RestoreHealth(30);
                    Debug.Log("Used Cake: Restored 30 health");
                    break;
                case "Knife":
                    playerEffects.Attack();
                    Debug.Log("Used Knife: Performed an attack");
                    break;
                default:
                    Debug.LogWarning($"No use effect defined for {itemName}");
                    break;
            }
        }
    }

    public int GetItemCount(string itemName)
    {
        return items.ContainsKey(itemName) ? items[itemName] : 0;
    }

    private void UpdateInventoryUI()
    {
        InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI != null)
        {
            inventoryUI.UpdateUI(items);
        }
        else
        {
            Debug.LogWarning("InventoryUI not found in scene!");
        }
    }
}