using UnityEngine;

public class Thorns : MonoBehaviour
{
    [SerializeField] private PlayerEffects playerEffects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float damage = 10.0f;
    private void OnCollisionEnter (Collision collision)
    {
        playerEffects.TakeDamage(damage);
    }
}
