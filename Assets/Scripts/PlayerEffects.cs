using UnityEngine;
using System.Collections;

public class PlayerEffects : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private GroundedCharacterController characterController; // Ссылка на GroundedCharacterController

    private float baseWalkForce; // Для хранения исходного значения m_WalkForce
    private float baseAirControl; // Для хранения исходного значения m_AirControl

    private void Start()
    {
        // Проверяем, что characterController привязан
        if (characterController == null)
        {
            characterController = GetComponent<GroundedCharacterController>();
            if (characterController == null)
            {
                Debug.LogError("GroundedCharacterController not found on this GameObject!");
                return;
            }
        }

        // Сохраняем базовые значения скорости из GroundedCharacterController
        baseWalkForce = characterController.GetWalkForce();
        baseAirControl = characterController.GetInputForce() / characterController.GetWalkForce(); // m_AirControl = GetInputForce() / m_WalkForce в воздухе
    }

    // Восстановление здоровья
    public void RestoreHealth(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        Debug.Log($"Health restored! Current health: {health}");
    }

    // Временное увеличение скорости
    public void BoostSpeed(float multiplier, float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
    {
        // Получаем текущие значения через публичные методы
        float currentWalkForce = characterController.GetWalkForce();
        float currentAirControl = characterController.GetInputForce() / currentWalkForce;

        // Изменяем значения через рефлексию или добавим публичные методы в GroundedCharacterController
        // Для простоты предположим, что у GroundedCharacterController есть публичные сеттеры (добавим их ниже)
        characterController.SetWalkForce(currentWalkForce * multiplier);
        characterController.SetAirControl(currentAirControl * multiplier);

        Debug.Log($"Speed boosted: WalkForce={currentWalkForce * multiplier}, AirControl={currentAirControl * multiplier}");
        yield return new WaitForSeconds(duration);

        // Восстанавливаем исходные значения
        characterController.SetWalkForce(baseWalkForce);
        characterController.SetAirControl(baseAirControl);
        Debug.Log($"Speed returned to normal: WalkForce={baseWalkForce}, AirControl={baseAirControl}");
    }

    // Атака
    public void Attack()
    {
        Debug.Log("Player performed an attack!");
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("AttackTrigger");
        }
        else
        {
            Debug.LogWarning("Animator not found on player for attack animation!");
        }
    }

    // Для отладки: получение текущего здоровья
    public float GetHealth()
    {
        return health;
    }
}