using UnityEngine;
using UnityEngine.EventSystems;

public class SlideTransition : MonoBehaviour
{
    public RectTransform currentCanvas;
    public RectTransform nextCanvas;
    public float duration = 0.5f;
    public bool isForward = true; // true = ����, false = �߂�
    public UnityEngine.UI.Button defaultNextButton; // �X���C�h��ɑI����Ԃɂ������{�^��

    public GameObject mainMenuCanvas; // �t���o�b�N�p
    public GameObject tutorialCanvas; // �t���o�b�N�p
    public GameObject GameCanvas;
    public bool isFullBack = false; // true �̏ꍇ�A���S�Ƀ��j���[�ɖ߂�

    public RectTransform tutorialCanvas1; // ���W���Z�b�g�p
    public RectTransform falsetutorialCanvas; // �C�ӂŗ��p

    private Vector2 tutorialCanvas1InitialPos = Vector2.zero; // �����ʒu�ۑ��p
    private bool isInitialPositionSet = false;

    private void LateUpdate()
    {
        if (!isInitialPositionSet && tutorialCanvas1 != null && tutorialCanvas1.gameObject.activeSelf)
        {
            tutorialCanvas1InitialPos = tutorialCanvas1.anchoredPosition;
            isInitialPositionSet = true;
            Debug.Log("�����ʒu��ۑ�: " + tutorialCanvas1InitialPos);
        }
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject &&
            (Input.GetKeyDown(KeyCode.Z)))
        {
            Slide();
        }
    }

    public void Slide()
    {
        if (isFullBack)
        {
            if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
            if (GameCanvas != null) GameCanvas.SetActive(true);
            if (tutorialCanvas != null) tutorialCanvas.SetActive(false);
            UnityEngine.UI.Button buttonToSelect = defaultNextButton != null
            ? defaultNextButton
            : nextCanvas.GetComponentInChildren<UnityEngine.UI.Button>();

            if (buttonToSelect != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(buttonToSelect.gameObject);
            }
            if (tutorialCanvas1 != null)
            {
                tutorialCanvas1.anchoredPosition = tutorialCanvas1InitialPos; // �����ʒu�ɖ߂�
                tutorialCanvas1.gameObject.SetActive(true);
            }
            if (falsetutorialCanvas != null) falsetutorialCanvas.gameObject.SetActive(false);

            return;
        }
        StartCoroutine(SlideCanvas());
    }

    private System.Collections.IEnumerator SlideCanvas()
    {

        float width = currentCanvas.rect.width;

        // ��� (0,0) ����ɃX���C�h
        Vector2 fixedStart = Vector2.zero;

        Vector2 startCurrent = fixedStart;
        Vector2 targetCurrent = startCurrent + (isForward ? new Vector2(-width, 0) : new Vector2(width, 0));

        Vector2 targetNext = fixedStart;
        Vector2 startNext = targetNext + (isForward ? new Vector2(width, 0) : new Vector2(-width, 0));

        currentCanvas.anchoredPosition = startCurrent;
        nextCanvas.anchoredPosition = startNext;
        nextCanvas.gameObject.SetActive(true);

        float time = 0;
        while (time < duration)
        {
            float t = time / duration;
            currentCanvas.anchoredPosition = Vector2.Lerp(startCurrent, targetCurrent, t);
            nextCanvas.anchoredPosition = Vector2.Lerp(startNext, targetNext, t);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        currentCanvas.anchoredPosition = targetCurrent;
        nextCanvas.anchoredPosition = targetNext;
        currentCanvas.gameObject.SetActive(false);

        // �X���C�h��ɃJ�[�\���ړ�
        UnityEngine.UI.Button buttonToSelect = defaultNextButton != null
            ? defaultNextButton
            : nextCanvas.GetComponentInChildren<UnityEngine.UI.Button>();

        if (buttonToSelect != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttonToSelect.gameObject);
        }
    }

}
