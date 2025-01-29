using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyCharacterController : MonoBehaviour
{
    public float moveSpeed = 30f; // �������� �����������
    public float jumpForce = 35f; // ���� ������
    
    public int hp = 10;
    public Text healthText; // ����� ��� ����������� ������ "Health"
    public Text healthRemainText; // ����� ��� ����������� ����������� ��������


    public float attackRange = 3f; // ������ �����
    public LayerMask enemyLayer; // ���� ������
    public int attackDamage = 1; // ���� �� ����
    private float timeLeft = 0;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool hasKicked = false;
    private SpriteRenderer sprite;

    public LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        UpdateHealthUI();
    }

    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // �����
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Attack(Vector2.down); // ����� ������

        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Attack(Vector2.right * Mathf.Sign(transform.localScale.x)); // ����� ������
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Attack(Vector2.up); // ����� �����
        }

        if (hp <= 0)
        {
            Destroy(gameObject); // ���������� ���� ������
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (hp != int.Parse(healthRemainText.text)) // ������ ���� �������� �������� ����������
        {
            UpdateHealthUI();
        }
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        // Flip ��������� � ����������� �� �����������
        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
        Debug.Log("Jump");
    }

    private void FixedUpdate()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        Vector2 position = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, 5.5f, groundLayer);
        isGrounded = hit.collider != null;

        // ������������ ���� (����� ����� � ����� � Play Mode)
        Debug.DrawRay(position, Vector2.down * 5.5f, Color.red);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !hasKicked)
        {
            hp -= 1;
            hasKicked = true;
            sprite.color = new Color(1, 0, 0, 1);
            UpdateHealthUI();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            hasKicked = false;
            sprite.color = new Color(0.5189569f, 0.6270496f, 0.9245283f, 1);
        }
    }

    private void Attack(Vector2 direction)
    {
        // ������ ��������� ����� �����
        Vector2 attackPosition = (Vector2)transform.position + direction * 0.5f;

        // ����� ��� ������ � ������� �����
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);

        // ������� ���� ���� ������
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Attacked " + enemy.name);
            enemy.GetComponent<MyEnemy>().TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        sprite.color = new Color(1, 0, 0, 1); // ������� ���� ��� ��������� �����
        timeLeft = 0.1f;

        if (hp <= 0)
        {
            Destroy(gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        healthText.text = "��������: "; // ���������� ����� "Health"
        healthRemainText.text = hp.ToString(); // ���������� ������� ���������� ��������
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // �������� ������ ����� ��� �������
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.down * 5f, attackRange); // ����� ����
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.right * Mathf.Sign(transform.localScale.x) * 5f, attackRange); // ����� ������
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.up * 5f, attackRange); // ����� �����
    }
}