using UnityEngine;

public class CursorUnlocker : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;  // ロック解除
        Cursor.visible = true;                   // カーソル表示
    }
}
