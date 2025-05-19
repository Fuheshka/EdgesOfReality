using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyEnemy : MonoBehaviour
{
    public int hp = 3;
    public float moveSpeed = 3f; // �������� �������� �����
    public float attackRange = 1.5f; // ������ �����
    public int attackDamage = 7; // ���� ��� �����
    public float attackCooldown = 1f; // ����� ����� �������
    public Transform[] patrolPoints; // ����� ��� ��������������
    private int currentPatrolPointIndex = 0; // ������ ������� ����� �������
    public float detectionRange = 5f; // ������ ����������� ������

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

        // ���������� ��������� ���� ��������������
        currentPatrolPointIndex = 0; // ���������, ��� ������ ����������
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        timeSinceLastAttack -= Time.deltaTime;

        // ��������� ���������� �� ������
        if (player != null && Vector2.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            Patrol();
        }

        // �������� �����
        if (player != null && Vector2.Distance(transform.position, player.transform.position) <= attackRange && timeSinceLastAttack <= 0)
        {
            AttackPlayer();
        }

        // ����� ���� �������� ����, �� ������ ����
        if (timeLeft <= 0)
        {
            sprite.color = new Color(1, 1, 0, 1); // �������������� ����������� �����
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolPointIndex];

        // ��������� � ������� ����� �������
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
        animator.SetBool("isWalking", true); // �������� �������� ��������

        // ������������ ����� � ������� ��������
        if (transform.position.x < targetPoint.position.x)
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
            // ����� �� ����� ������������ ���� �� �� ��������� ��� �������� ��������
            // currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length; // ��������� �����
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length; // ��������� �����
        }
        else
        {
            // ���� ���� �� ������ �����, ������� �� ������� � ���������� ��������
            return;
        }
    }

    private void MoveTowardsPlayer()
    {
        // ��������� � ������
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        animator.SetBool("isWalking", true); // �������� �������� ��������

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
        player.GetComponent<PlayerEffects>().TakeDamage(attackDamage);
        animator.SetTrigger("attack"); // �������� �������� �����
        timeSinceLastAttack = attackCooldown;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        sprite.color = new Color(1, 0, 0, 1); // ������� ���� ��� ��������� �����
        timeLeft = 0.1f;

        if (hp <= 0)
        {
            animator.SetTrigger("die"); // �������� �������� ������
            Destroy(gameObject, 1f); // ���������� ������ ����� 1 ������� ����� ��������
        }
    }
}