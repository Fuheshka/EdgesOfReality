using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target; // Цель, за которой следует камера (например, персонаж)
    [SerializeField] private Vector2 offset; // Смещение камеры относительно цели (по X и Y)
    [SerializeField] private float smoothSpeed = 0.125f; // Скорость сглаживания движения камеры
    [SerializeField] private BoxCollider2D boundsCollider; // Коллайдер, задающий границы локации
    [SerializeField] private float penetrationDistance = 0.5f; // На сколько камера может заходить в коллайдеры

    private Vector2 minBounds; // Минимальные границы (левый нижний угол)
    private Vector2 maxBounds; // Максимальные границы (правый верхний угол)
    private Camera cam; // Ссылка на компонент камеры
    private float camHalfWidth; // Половина ширины камеры
    private float camHalfHeight; // Половина высоты камеры

    private void Start()
    {
        // Получаем компонент камеры
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Camera component not found on this GameObject!");
            return;
        }

        // Проверяем наличие цели
        if (target == null)
        {
            Debug.LogError("Target not assigned in CameraFollow2D!");
            return;
        }

        // Проверяем наличие коллайдера границ
        if (boundsCollider != null)
        {
            // Вычисляем границы на основе BoxCollider2D
            minBounds = boundsCollider.bounds.min;
            maxBounds = boundsCollider.bounds.max;
        }
        else
        {
            Debug.LogWarning("BoundsCollider not assigned! Camera will not be constrained.");
            minBounds = Vector2.negativeInfinity;
            maxBounds = Vector2.positiveInfinity;
        }

        // Вычисляем половину размера камеры в мировых координатах
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Вычисляем желаемую позицию камеры (цель + смещение)
        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z // Сохраняем текущую Z-позицию камеры
        );

        // Плавное следование за целью
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime * 60f);

        // Ограничиваем позицию камеры в пределах границ с учётом penetrationDistance
        if (boundsCollider != null)
        {
            smoothedPosition.x = Mathf.Clamp(
                smoothedPosition.x,
                minBounds.x + camHalfWidth - penetrationDistance, // Расширяем минимальную границу
                maxBounds.x - camHalfWidth + penetrationDistance  // Расширяем максимальную границу
            );
            smoothedPosition.y = Mathf.Clamp(
                smoothedPosition.y,
                minBounds.y + camHalfHeight - penetrationDistance, // Расширяем минимальную границу
                maxBounds.y - camHalfHeight + penetrationDistance  // Расширяем максимальную границу
            );
        }

        // Применяем позицию к камере
        transform.position = smoothedPosition;
    }

    // Метод для динамической смены цели
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Метод для динамической смены границ
    public void SetBounds(BoxCollider2D newBounds)
    {
        boundsCollider = newBounds;
        if (boundsCollider != null)
        {
            minBounds = boundsCollider.bounds.min;
            maxBounds = boundsCollider.bounds.max;
        }
        else
        {
            minBounds = Vector2.negativeInfinity;
            maxBounds = Vector2.positiveInfinity;
        }
    }

    // Метод для динамического изменения penetrationDistance
    public void SetPenetrationDistance(float newDistance)
    {
        penetrationDistance = Mathf.Max(0, newDistance);
    }

    // Визуальная отладка границ в редакторе
    private void OnDrawGizmos()
    {
        if (boundsCollider == null || cam == null) return;

        Gizmos.color = Color.green;
        Vector2 size = new Vector2(
            (maxBounds.x - minBounds.x) + 2 * penetrationDistance,
            (maxBounds.y - minBounds.y) + 2 * penetrationDistance
        );
        Vector2 center = (minBounds + maxBounds) / 2;
        Gizmos.DrawWireCube(center, size);
    }
}