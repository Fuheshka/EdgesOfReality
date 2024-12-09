using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterController : MonoBehaviour
{
    public float moveSpeed = 10f; // Скорость перемещения
    public float jumpForce = 6f; // Сила прыжка
    private Rigidbody2D rb; // Компонент Rigidbody2D
    private bool isGrounded = false; // Проверка, на земле ли персонаж
    public LayerMask groundLayer; // Слой земли

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();

        // Прыжок только если персонаж на земле
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false; // Устанавливаем в false сразу после прыжка
    }

    private void FixedUpdate()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        // Определяем, на земле ли персонаж
        Vector2 position = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, 0.6f, groundLayer);
        isGrounded = hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        // Настройка начальной и конечной точки луча
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * 0.6f;

        // Выполняем проверку Raycast
        RaycastHit2D hit = Physics2D.Raycast(start, Vector2.down, 0.6f, groundLayer);

        // Меняем цвет луча в зависимости от результата
        if (hit.collider != null)
        {
            Gizmos.color = Color.green; // Луч касается земли
        }
        else
        {
            Gizmos.color = Color.red; // Луч не касается земли
        }

        // Рисуем луч
        Gizmos.DrawLine(start, end);
    }
}