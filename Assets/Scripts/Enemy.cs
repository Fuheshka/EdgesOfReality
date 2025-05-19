using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyEnemy : MonoBehaviour
{
    public int hp = 3;
    public float moveSpeed = 3f; // Скорость движения врага
    public float attackRange = 1.5f; // Радиус атаки
    public int attackDamage = 7; // Урон при атаке
    public float attackCooldown = 1f; // Время между атаками
    public Transform[] patrolPoints; // Точки для патрулирования
    private int currentPatrolPointIndex = 0; // Индекс текущей точки патруля
    public float detectionRange = 5f; // Радиус обнаружения игрока

    private SpriteRenderer sprite;
    private GameObject player;
    private float timeLeft = 0f;
    private float timeSinceLastAttack = 0f;
    private Animator animator;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points assigned!");
        }

        // Установите начальную цель патрулирования
        currentPatrolPointIndex = 0; // Убедитесь, что индекс корректный
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        timeSinceLastAttack -= Time.deltaTime;

        // Проверяем расстояние до игрока
        if (player != null && Vector2.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            Patrol();
        }

        // Проверка атаки
        if (player != null && Vector2.Distance(transform.position, player.transform.position) <= attackRange && timeSinceLastAttack <= 0)
        {
            AttackPlayer();
        }

        // Когда враг получает урон, он меняет цвет
        if (timeLeft <= 0)
        {
            sprite.color = new Color(1, 1, 0, 1); // Восстановление нормального цвета
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolPointIndex];

        // Двигаемся к текущей точке патруля
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
        animator.SetBool("isWalking", true); // Включаем анимацию движения

        // Поворачиваем врага в сторону движения
        if (transform.position.x < targetPoint.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1); // Поворачиваем вправо
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // Поворачиваем влево
        }

        // Если враг достиг точки патруля, выбираем следующую точку
        if (Vector2.Distance(transform.position, targetPoint.position) <= 0.1f)
        {
            // Здесь мы можем остановиться хотя бы на мгновение или добавить задержку
            // currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length; // Следующая точка
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length; // Следующая точка
        }
        else
        {
            // Если враг не достиг точки, выходим из функции и продолжаем движение
            return;
        }
    }

    private void MoveTowardsPlayer()
    {
        // Двигаемся к игроку
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        animator.SetBool("isWalking", true); // Включаем анимацию движения

        // Поворачиваем врага в сторону игрока
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Поворачиваем вправо
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // Поворачиваем влево
        }
    }

    private void AttackPlayer()
    {
        player.GetComponent<PlayerEffects>().TakeDamage(attackDamage);
        animator.SetTrigger("attack"); // Включаем анимацию атаки
        timeSinceLastAttack = attackCooldown;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        sprite.color = new Color(1, 0, 0, 1); // Красный цвет при получении урона
        timeLeft = 0.1f;

        if (hp <= 0)
        {
            animator.SetTrigger("die"); // Включаем анимацию смерти
            Destroy(gameObject, 1f); // Уничтожаем объект через 1 секунду после анимации
        }
    }
}