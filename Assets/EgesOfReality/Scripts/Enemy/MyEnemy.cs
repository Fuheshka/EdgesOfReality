using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyEnemy : MonoBehaviour
{
    public int hp = 3;
    public float moveSpeed = 3f; // �������� �������� �����
    public float attackRange = 1.5f; // ������ �����
    public int attackDamage = 1; // ���� ��� �����
    public float attackCooldown = 1f; // ����� ����� �������
    public Transform[] patrolPoints; // ����� ��� ��������������
    private int currentPatrolPointIndex = 0; // ������ ������� ����� �������
    public float detectionRange = 5f; // ������ ����������� ������

    private SpriteRenderer sprite;
    private GameObject player;
    private float timeLeft = 0f;
    private float timeSinceLastAttack = 0f;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player"); // ���� ������ �� ����

        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points assigned!");
        }
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        timeSinceLastAttack -= Time.deltaTime;

        // ��������� ���������� �� ������ � �������� �������������, ���� � �������� �������
        if (player != null && Vector2.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            MoveTowardsPlayer();

            // �������� �� �����
            if (Vector2.Distance(transform.position, player.transform.position) <= attackRange && timeSinceLastAttack <= 0)
            {
                AttackPlayer();
            }
        }
        else
        {
            Patrol();
        }

        // ����� ���� �������� ����, �� ������ ����
        if (timeLeft <= 0)
        {
            sprite.color = new Color(1, 0, 1, 1); // �������������� ����������� �����
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        // ��������� � ������� ����� �������
        Transform targetPoint = patrolPoints[currentPatrolPointIndex];
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        // ������������ ����� � ������� ��������
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // ������������ ������
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // ������������ �����
        }

        // ���� ���� ������ ����� �������, �������� ��������� �����
        if (Vector2.Distance(transform.position, targetPoint.position) <= 0.1f)
        {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length; // ��������� �����
        }
    }

    private void MoveTowardsPlayer()
    {
        // ��������� � ������
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);

        // ������������ ����� � ������� ������
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // ������������ ������
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // ������������ �����
        }
    }

    private void AttackPlayer()
    {
        // ������� ���� ������
        player.GetComponent<MyCharacterController>().TakeDamage(attackDamage);

        // ����� ����� �������
        timeSinceLastAttack = attackCooldown;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        sprite.color = new Color(1, 0, 0, 1); // ������� ���� ��� ��������� �����
        timeLeft = 0.1f;

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}