using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text sandwichText;
    [SerializeField] private TMP_Text coffeeText;
    [SerializeField] private TMP_Text cakeText;
    [SerializeField] private TMP_Text knifeText;
    [SerializeField] private Button useSandwichButton; // Добавляем кнопки
    [SerializeField] private Button useCoffeeButton;
    [SerializeField] private Button useCakeButton;
    [SerializeField] private Button useKnifeButton;

    private Animator animator;
    private const float animationDuration = 0.5f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found on InventoryPanel!");
        }
        gameObject.SetActive(false);

        // Привязываем обработчики к кнопкам
        if (useSandwichButton != null)
            useSandwichButton.onClick.AddListener(() => UseItem("Sandwich"));
        if (useCoffeeButton != null)
            useCoffeeButton.onClick.AddListener(() => UseItem("Coffee"));
        if (useCakeButton != null)
            useCakeButton.onClick.AddListener(() => UseItem("Cake"));
        if (useKnifeButton != null)
            useKnifeButton.onClick.AddListener(() => UseItem("Knife"));
    }

    public void UpdateUI(Dictionary<string, int> items)
    {
        if (sandwichText == null || coffeeText == null || cakeText == null || knifeText == null)
        {
            Debug.LogError("One or more TMP_Text fields are not assigned in InventoryUI!");
            return;
        }

        sandwichText.text = $"Sandwich: {(items.ContainsKey("Sandwich") ? items["Sandwich"] : 0)}";
        coffeeText.text = $"Coffee: {(items.ContainsKey("Coffee") ? items["Coffee"] : 0)}";
        cakeText.text = $"Cake: {(items.ContainsKey("Cake") ? items["Cake"] : 0)}";
        knifeText.text = $"Knife: {(items.ContainsKey("Knife") ? items["Knife"] : 0)}";
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (animator != null)
        {
            animator.ResetTrigger("FadeOut");
            animator.SetTrigger("FadeIn");
        }
    }

    public void Hide()
    {
        if (animator != null)
        {
            animator.ResetTrigger("FadeIn");
            animator.SetTrigger("FadeOut");
        }
        StartCoroutine(HideAfterAnimation());
    }

    private IEnumerator HideAfterAnimation()
    {
        yield return new WaitForSecondsRealtime(animationDuration);
        gameObject.SetActive(false);
    }

    private void UseItem(string itemName)
    {
        // Проверяем, есть ли предмет
        if (Inventory.Instance.GetItemCount(itemName) > 0)
        {
            Inventory.Instance.UseItem(itemName); // Вызываем метод UseItem
        }
        else
        {
            Debug.Log($"No {itemName} in inventory to use!");
        }
    }
}