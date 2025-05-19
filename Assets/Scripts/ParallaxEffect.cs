using UnityEngine;

public class Parallax : MonoBehaviour 
{
    public float parallaxSpeed;
    public bool infiniteHorizontal = false; // Для бесконечного фона
    private float textureUnitSizeX;
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    private void Awake() 
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        
        // Рассчитываем размер текстуры для бесконечного фона
        if (infiniteHorizontal) 
        {
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            textureUnitSizeX = sprite.texture.width / sprite.pixelsPerUnit;
        }
    }

    private void Start() 
    {
        // Принудительное выравнивание с камерой
        transform.position = new Vector3(
            cameraTransform.position.x,
            cameraTransform.position.y,
            transform.position.z
        );
    }

    private void LateUpdate() 
    {
        Vector3 delta = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(delta.x * parallaxSpeed, delta.y * parallaxSpeed, 0);
        lastCameraPosition = cameraTransform.position;

        // Бесконечный повтор по X
        if (infiniteHorizontal && Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX) 
        {
            float offsetX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(
                cameraTransform.position.x + offsetX,
                transform.position.y,
                transform.position.z
            );
        }
    }
}