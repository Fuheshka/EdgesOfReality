using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    Slider slider;
    [SerializeField] private PlayerEffects playerEffects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = playerEffects.GetHealth();
    }
}
