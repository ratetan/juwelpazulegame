using UnityEngine;
using UnityEngine.EventSystems;

public class CursorFollower : MonoBehaviour
{
    public RectTransform cursor; // �J�[�\���I�u�W�F�N�g
    public Vector2 offset = new Vector2(50f, 0f); // �I�t�Z�b�g

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected != null && selected.GetComponent<RectTransform>() != null)
        {
            RectTransform selectedRect = selected.GetComponent<RectTransform>();

            // �e������Canvas����UI�v�f�Ȃ� anchoredPosition ���g��
            Vector2 targetPos = selectedRect.anchoredPosition + offset;
            cursor.anchoredPosition = targetPos;
        }
    }
}
