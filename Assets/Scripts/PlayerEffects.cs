using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerEffects : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private GroundedCharacterController characterController; // Ссылка на GroundedCharacterController

    public float attackRange = 1.5f; // Дальность атаки
    public int attackDamage = 1; // Урон атаки
    public LayerMask enemyLayer; // Слой, на котором находятся враги

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) // Замените на вашу клавишу атаки
        {
            Attack();
        }
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

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("TakeDmagePlayer" + damage);
        
        if (health <= 0)
        {
            Destroy(gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Атака
    public void Attack()
    {
        // Проверяем врагов в радиусе атаки
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position + transform.right * attackRange, 1f, enemyLayer);

        // Наносим урон каждому врагу, который попадает в радиус атаки
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<MyEnemy>().TakeDamage(attackDamage);
        }

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

    // Метод для визуализации области атаки (для отладки)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.right * attackRange, 0.5f);
    }

    // Для отладки: получение текущего здоровья
    public float GetHealth()
    {
        return health;
    }
}