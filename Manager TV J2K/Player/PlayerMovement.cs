using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller; // Компонент CharacterController
    public float speed = 12f; // Скорость движения
    public float gravity = -9.81f; // Гравитация
    public float jumpHeight = 3f; // Высота прыжка

    private Vector3 velocity; // Вектор скорости
    private bool isGrounded; // Проверка на земле

    void Update()
    {
        // Проверяем, находимся ли мы на земле
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Устанавливаем скорость вниз, если на земле
        }

        // Получаем входные данные для движения
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Переводим входные данные в направление движения
        Vector3 move = transform.right * x + transform.forward * z;

        // Двигаем персонажа
        controller.Move(move * speed * Time.deltaTime);

        // Добавляем гравитацию
        velocity.y += gravity * Time.deltaTime;

        // Перемещаем персонажа с учетом гравитации
        controller.Move(velocity * Time.deltaTime);

        // Прыжок
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
