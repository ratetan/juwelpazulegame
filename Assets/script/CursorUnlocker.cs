using UnityEngine;

public class CursorUnlocker : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;  // ���b�N����
        Cursor.visible = true;                   // �J�[�\���\��
    }
}
