using UnityEngine;
using UnityEngine.EventSystems;

public class CursorFollower : MonoBehaviour
{
    public RectTransform cursor; // カーソルオブジェクト
    public Vector2 offset = new Vector2(50f, 0f); // オフセット

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected != null && selected.GetComponent<RectTransform>() != null)
        {
            RectTransform selectedRect = selected.GetComponent<RectTransform>();

            // 親が同じCanvas内のUI要素なら anchoredPosition を使う
            Vector2 targetPos = selectedRect.anchoredPosition + offset;
            cursor.anchoredPosition = targetPos;
        }
    }
}
