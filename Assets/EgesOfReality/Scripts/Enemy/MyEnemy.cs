using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class MyEnemy : MonoBehaviour
{
    public int hp = 3;
    SpriteRenderer sprite;
    float timeLeft = 0f;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            sprite.color = new Color(1, 0, 0, 1);
        }
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;
        sprite.color = new Color(0, 1, 0, 1);
        timeLeft = 0.1f;

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
