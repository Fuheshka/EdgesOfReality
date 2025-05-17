using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ExitScene : MonoBehaviour
{
    public Animator animator; // ������������ ���������� �������
    public string sceneName = ""; // �������� ����� �����
    private bool isNearObject = false; // ���� �������� ������ � ������� �������

    void Update()
    {
        if (isNearObject && Input.GetKeyDown(KeyCode.E)) // �������� ������� ������ �
        {
            StartCoroutine(LoadNewScene());
        }
    }

    // ����� �������� ��������� ������ ������ �������-����
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ��������, ��� ����� ������ ����� (��� ��� ������ ���� Player)
        {
            isNearObject = true;
        }
    }

    // ��������� ������ �� ����
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearObject = false;
        }
    }

    IEnumerator LoadNewScene() // �������� ��� ��������� �������� ����� �������
    {
        // ��������� �������� ����� ���������
        animator.SetBool("StartAnimation", true);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // ��� ���������� ��������

        // ��������� ����� ����� ����������
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
