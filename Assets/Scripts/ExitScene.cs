using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ExitScene : MonoBehaviour
{
    public Animator animator; // Анимационный контроллер объекта
    public string sceneName = ""; // Название новой сцены
    private bool isNearObject = false; // Флаг близости игрока к нужному объекту

    void Update()
    {
        if (isNearObject && Input.GetKeyDown(KeyCode.E)) // Проверка нажатия кнопки Е
        {
            StartCoroutine(LoadNewScene());
        }
    }

    // Метод проверки попадания игрока внутрь триггер-зоны
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Убедимся, что вошли именно игрок (его тег должен быть Player)
        {
            isNearObject = true;
        }
    }

    // Обработка выхода из зоны
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearObject = false;
        }
    }

    IEnumerator LoadNewScene() // Корутина для плавности перехода между сценами
    {
        // Запускаем анимацию перед переходом
        animator.SetBool("StartAnimation", true);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // Ждём завершения анимации

        // Загружаем новую сцену асинхронно
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
