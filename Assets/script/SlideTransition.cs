using UnityEngine;
using UnityEngine.EventSystems;

public class SlideTransition : MonoBehaviour
{
    public RectTransform currentCanvas;
    public RectTransform nextCanvas;
    public float duration = 0.5f;
    public bool isForward = true; // true = 次へ, false = 戻る
    public UnityEngine.UI.Button defaultNextButton; // スライド後に選択状態にしたいボタン

    public GameObject mainMenuCanvas; // フルバック用
    public GameObject tutorialCanvas; // フルバック用
    public GameObject GameCanvas;
    public bool isFullBack = false; // true の場合、完全にメニューに戻す

    public RectTransform tutorialCanvas1; // 座標リセット用
    public RectTransform falsetutorialCanvas; // 任意で利用

    private Vector2 tutorialCanvas1InitialPos = Vector2.zero; // 初期位置保存用
    private bool isInitialPositionSet = false;

    private void LateUpdate()
    {
        if (!isInitialPositionSet && tutorialCanvas1 != null && tutorialCanvas1.gameObject.activeSelf)
        {
            tutorialCanvas1InitialPos = tutorialCanvas1.anchoredPosition;
            isInitialPositionSet = true;
            Debug.Log("初期位置を保存: " + tutorialCanvas1InitialPos);
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
                tutorialCanvas1.anchoredPosition = tutorialCanvas1InitialPos; // 初期位置に戻す
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

        // 常に (0,0) を基準にスライド
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

        // スライド後にカーソル移動
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
