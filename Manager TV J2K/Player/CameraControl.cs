using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform player; // ������ �� ���������
    public float mouseSensitivity = 100f; // ���������������� ����
    public float verticalRotationLimit = 80f; // ����������� ������������� ��������

    private float xRotation = 0f; // ���� �������� �� ��� X

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // �������� �������� ����
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ��������� ���� �������� �� ��� Y (�������������� ��������)
        player.Rotate(Vector3.up * mouseX);

        // ������������ �������� �� ��� X (������������ ��������)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalRotationLimit, verticalRotationLimit);

        // ��������� �������� � ������
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
