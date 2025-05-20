using UnityEngine;

public class Thorns : MonoBehaviour
{
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float damage = 10.0f;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnCollisionEnter (Collision collision)
    {
        PlayerEffects playerEffects = player.GetComponent<PlayerEffects>();
        playerEffects.TakeDamage(damage); // урон
    }
}
