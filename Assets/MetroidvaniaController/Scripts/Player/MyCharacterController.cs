using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterController : MonoBehaviour
{
    public float moveSpeed = 10f; // �������� �����������
    public float jumpForce = 6f; // ���� ������
    private Rigidbody2D rb; // ��������� Rigidbody2D
    private bool isGrounded = false; // ��������, �� ����� �� ��������
    public LayerMask groundLayer; // ���� �����

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();

        // ������ ������ ���� �������� �� �����
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
        isGrounded = false; // ������������� � false ����� ����� ������
    }

    private void FixedUpdate()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        // ����������, �� ����� �� ��������
        Vector2 position = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, 0.6f, groundLayer);
        isGrounded = hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        // ��������� ��������� � �������� ����� ����
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * 0.6f;

        // ��������� �������� Raycast
        RaycastHit2D hit = Physics2D.Raycast(start, Vector2.down, 0.6f, groundLayer);

        // ������ ���� ���� � ����������� �� ����������
        if (hit.collider != null)
        {
            Gizmos.color = Color.green; // ��� �������� �����
        }
        else
        {
            Gizmos.color = Color.red; // ��� �� �������� �����
        }

        // ������ ���
        Gizmos.DrawLine(start, end);
    }
}