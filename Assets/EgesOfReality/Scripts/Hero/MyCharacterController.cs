using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyCharacterController : MonoBehaviour
{
    public float moveSpeed = 10f; // Скорость перемещения
    public float jumpForce = 6f; // Сила прыжка
    public int hp = 10;

    public float attackRange = 1f; // Радиус атаки
    public LayerMask enemyLayer; // Слой врагов
    public int attackDamage = 1; // Урон от атак

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool hasKicked = false;
    private SpriteRenderer sprite;

    public LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Атаки
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Attack(Vector2.down); // Атака сверху

        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Attack(Vector2.right * Mathf.Sign(transform.localScale.x)); // Атака вперед
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Attack(Vector2.up); // Атака снизу
        }

        if (hp <= 0)
        {
            Destroy(rb);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        // Flip персонажа в зависимости от направления
        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
    }

    private void FixedUpdate()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        Vector2 position = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, 0.6f, groundLayer);
        isGrounded = hit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !hasKicked)
        {
            hp -= 1;
            hasKicked = true;
            sprite.color = new Color(1, 0, 0, 1);
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
        // Задаем начальную точку атаки
        Vector2 attackPosition = (Vector2)transform.position + direction * 0.5f;

        // Найти все врагов в радиусе атаки
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);

        // Нанести урон всем врагам
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Attacked " + enemy.name);
            enemy.GetComponent<MyEnemy>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // Показать радиус атаки для отладки
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.down * 0.5f, attackRange); // Атака вниз
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.right * Mathf.Sign(transform.localScale.x) * 0.5f, attackRange); // Атака вперед
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.up * 0.5f, attackRange); // Атака вверх
    }
}