using UnityEngine;
using System.Collections;

public class PlayerEffects : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private GroundedCharacterController characterController; // ������ �� GroundedCharacterController

    private float baseWalkForce; // ��� �������� ��������� �������� m_WalkForce
    private float baseAirControl; // ��� �������� ��������� �������� m_AirControl

    private void Start()
    {
        // ���������, ��� characterController ��������
        if (characterController == null)
        {
            characterController = GetComponent<GroundedCharacterController>();
            if (characterController == null)
            {
                Debug.LogError("GroundedCharacterController not found on this GameObject!");
                return;
            }
        }

        // ��������� ������� �������� �������� �� GroundedCharacterController
        baseWalkForce = characterController.GetWalkForce();
        baseAirControl = characterController.GetInputForce() / characterController.GetWalkForce(); // m_AirControl = GetInputForce() / m_WalkForce � �������
    }

    // �������������� ��������
    public void RestoreHealth(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        Debug.Log($"Health restored! Current health: {health}");
    }

    // ��������� ���������� ��������
    public void BoostSpeed(float multiplier, float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
    {
        // �������� ������� �������� ����� ��������� ������
        float currentWalkForce = characterController.GetWalkForce();
        float currentAirControl = characterController.GetInputForce() / currentWalkForce;

        // �������� �������� ����� ��������� ��� ������� ��������� ������ � GroundedCharacterController
        // ��� �������� �����������, ��� � GroundedCharacterController ���� ��������� ������� (������� �� ����)
        characterController.SetWalkForce(currentWalkForce * multiplier);
        characterController.SetAirControl(currentAirControl * multiplier);

        Debug.Log($"Speed boosted: WalkForce={currentWalkForce * multiplier}, AirControl={currentAirControl * multiplier}");
        yield return new WaitForSeconds(duration);

        // ��������������� �������� ��������
        characterController.SetWalkForce(baseWalkForce);
        characterController.SetAirControl(baseAirControl);
        Debug.Log($"Speed returned to normal: WalkForce={baseWalkForce}, AirControl={baseAirControl}");
    }

    // �����
    public void Attack()
    {
        Debug.Log("Player performed an attack!");
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("AttackTrigger");
        }
        else
        {
            Debug.LogWarning("Animator not found on player for attack animation!");
        }
    }

    // ��� �������: ��������� �������� ��������
    public float GetHealth()
    {
        return health;
    }
}