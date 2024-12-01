using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform player; // Ссылка на персонажа
    public float mouseSensitivity = 100f; // Чувствительность мыши
    public float verticalRotationLimit = 80f; // Ограничение вертикального вращения

    private float xRotation = 0f; // Угол вращения по оси X

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Получаем движение мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Обновляем угол вращения по оси Y (горизонтальное вращение)
        player.Rotate(Vector3.up * mouseX);

        // Ограничиваем вращение по оси X (вертикальное вращение)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalRotationLimit, verticalRotationLimit);

        // Применяем вращение к камере
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
