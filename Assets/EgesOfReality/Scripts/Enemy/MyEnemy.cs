using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyEnemy : MonoBehaviour
{
    public int hp = 3;
    public float moveSpeed = 3f; // Скорость движения врага
    public float attackRange = 1.5f; // Радиус атаки
    public int attackDamage = 1; // Урон при атаке
    public float attackCooldown = 1f; // Время между атаками
    public Transform[] patrolPoints; // Точки для патрулирования
    private int currentPatrolPointIndex = 0; // Индекс текущей точки патруля
    public float detectionRange = 5f; // Радиус обнаружения игрока

    private SpriteRenderer sprite;
    private GameObject player;
    private float timeLeft = 0f;
    private float timeSinceLastAttack = 0f;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player"); // Ищем игрока по тегу

        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points assigned!");
        }
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        timeSinceLastAttack -= Time.deltaTime;

        // Проверяем расстояние до игрока и начинаем преследование, если в пределах радиуса
        if (player != null && Vector2.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            MoveTowardsPlayer();

            // Проверка на атаку
            if (Vector2.Distance(transform.position, player.transform.position) <= attackRange && timeSinceLastAttack <= 0)
            {
                AttackPlayer();
            }
        }
        else
        {
            Patrol();
        }

        // Когда враг получает урон, он меняет цвет
        if (timeLeft <= 0)
        {
            sprite.color = new Color(1, 0, 1, 1); // Восстановление нормального цвета
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        // Двигаемся к текущей точке патруля
        Transform targetPoint = patrolPoints[currentPatrolPointIndex];
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        // Поворачиваем врага в сторону движения
        if (direction.x > 0)
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
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length; // Следующая точка
        }
    }

    private void MoveTowardsPlayer()
    {
        // Двигаемся к игроку
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);

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
        // Наносим урон игроку
        player.GetComponent<MyCharacterController>().TakeDamage(attackDamage);

        // Время между атаками
        timeSinceLastAttack = attackCooldown;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        sprite.color = new Color(1, 0, 0, 1); // Красный цвет при получении урона
        timeLeft = 0.1f;

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}