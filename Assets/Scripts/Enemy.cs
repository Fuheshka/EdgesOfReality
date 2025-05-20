using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyEnemy : MonoBehaviour
{
    public int hp = 3;
    public float moveSpeed = 3f; // �������� �������� �����
    public float attackRange = 1.5f; // ������ �����
    public float stopDistance = 1f; // ����������� ���������� �� ������, �� ������� ���� ���������������
    public int attackDamage = 7; // ���� ��� �����
    public float attackCooldown = 1f; // ����� ����� �������
    public Transform[] patrolPoints; // ����� ��� ��������������
    private int currentPatrolPointIndex = 0; // ������ ������� ����� �������
    public float detectionRange = 5f; // ������ ����������� ������
    public float raycastDistance = 0.5f; // ��������� ��� ������� �����������
    public float jumpHeight = 2f; // ������������ ������ ������
    public LayerMask obstacleLayer; // ���� ��� ���� � ����������� (�� ������)

    private SpriteRenderer sprite;
    private GameObject player;
    private float timeLeft = 0f;
    private float timeSinceLastAttack = 0f;
    private Animator animator;
    private Rigidbody rb; // ���������� Rigidbody ��� 3D

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); // �������� Rigidbody ��� 3D
        player = GameObject.FindGameObjectWithTag("Player");

        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points assigned!");
        }

        if (rb != null)
        {
            rb.freezeRotation = true; // ������������ ��������
        }

        currentPatrolPointIndex = 0; // ���������� ��������� ���� ��������������
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        timeSinceLastAttack -= Time.deltaTime;

        // ��������� ���������� �� ������
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            Patrol();
        }

        // �������� �����
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= attackRange && timeSinceLastAttack <= 0)
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

        // ��������� ������� ���������� x
        float targetX = targetPoint.position.x;
        float currentX = transform.position.x;

        // ������� ����� ������ �� ��� x
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.MoveTowards(currentX, targetX, moveSpeed * Time.deltaTime);
        transform.position = newPosition;

        animator.SetBool("isWalking", true); // �������� �������� ��������

        // ������������ ����� � ������� ��������
        if (targetX > currentX)
        {
            transform.localScale = new Vector3(1, 1, 1); // ������������ ������
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // ������������ �����
        }

        // ���� ���� ������ ����� ������� �� ��� x, �������� ��������� �����
        if (Mathf.Abs(currentX - targetX) <= 0.1f)
        {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length; // ��������� �����
        }
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        // ��������� ������� ���������� x ������
        float targetX = player.transform.position.x;
        float currentX = transform.position.x;
        float distanceToPlayer = Mathf.Abs(targetX - currentX);

        // ��������� ������� ����������� ����� ������
        bool isPathBlocked = CheckForObstacles(targetX > currentX);

        // ���� ���� �� ������������ � ���� ������, ��� stopDistance, ��������� � ������
        if (!isPathBlocked && distanceToPlayer > stopDistance)
        {
            // ��������� � ������ ������ �� ��� x
            Vector3 newPosition = transform.position;
            newPosition.x = Mathf.MoveTowards(currentX, targetX, moveSpeed * Time.deltaTime);
            transform.position = newPosition;

            animator.SetBool("isWalking", true); // �������� �������� ��������
        }
        else if (isPathBlocked && rb != null)
        {
            // �������, ���� ���� ������������
            Jump();
            animator.SetBool("isWalking", false); // ������������� �������� ������ �� ����� ������
        }
        else
        {
            animator.SetBool("isWalking", false); // ������������� ��������, ���� ���������� ���������
        }

        // ������������ ����� � ������� ������
        if (targetX > currentX)
        {
            transform.localScale = new Vector3(1, 1, 1); // ������������ ������
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // ������������ �����
        }
    }

    private bool CheckForObstacles(bool movingRight)
    {
        // ����������� �������� ����� ������
        Vector3 direction = movingRight ? transform.right : -transform.right;
        Vector3 checkPosition = transform.position + direction * raycastDistance;

        // ��������� ����� ������ ����� ����� ������
        Collider[] hitColliders = Physics.OverlapSphere(checkPosition, 0.5f, obstacleLayer);
        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject != player) // ���������� ������
            {
                return true; // ���� ������������
            }
        }
        return false; // ���� ��������
    }

    private void Jump()
    {
        if (rb != null && Mathf.Abs(rb.linearVelocity.y) < 0.1f) // Проверка, стоит ли на земле
        {
            float jumpVelocity = Mathf.Sqrt(2f * jumpHeight * Mathf.Abs(Physics.gravity.y));
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
            animator.SetTrigger("jump");
        }
    }

    private void AttackPlayer()
    {
        if (player != null)
        {
            PlayerEffects playerEffects = player.GetComponent<PlayerEffects>();
            playerEffects.TakeDamage(attackDamage); // урон
        }

        animator.SetTrigger("attack");
        timeSinceLastAttack = attackCooldown;
    }

    public void TakeDamage(int damage, float knockbackForce1)
    {
        hp -= damage;
        sprite.color = new Color(1, 0, 0, 1);
        timeLeft = 0.1f;

        // Отталкивание от игрока
        Vector3 knockbackDir = (transform.position - player.transform.position).normalized;
        ApplyKnockback(knockbackDir, knockbackForce1);

        if (hp <= 0)
        {
            animator.SetTrigger("die");
            Destroy(gameObject, 1f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerEffects playerEffects = collision.gameObject.GetComponent<PlayerEffects>();
            if (playerEffects != null)
            {
                playerEffects.TakeDamage(attackDamage);
            }
        }
    }

    // ��� �������: ������������ ����� ��������
    private void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            bool movingRight = player.transform.position.x > transform.position.x;
            Vector3 direction = movingRight ? transform.right : -transform.right;
            Vector3 checkPosition = transform.position + direction * raycastDistance;
            Gizmos.DrawWireSphere(checkPosition, 0.5f);
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (rb != null)
        {
            rb.AddForce(direction.normalized * force, ForceMode.Impulse);
        }
    }
}