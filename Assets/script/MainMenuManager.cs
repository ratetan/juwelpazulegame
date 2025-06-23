using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject firstSecondaryButton;
    public GameObject initialButtons;
    public GameObject secondaryButtons;
    public GameObject tutorialCanvas;
    public GameObject MainMenuCanvas;
    public Button tutorialFirstButton;
    public GameObject optionCanvas;
    public RectTransform cursor;
    public RectTransform optionBackButton;
    public GameObject optionButton;
    public GameObject creditCanvas;
    public Button creditFirstButton;
    public GameObject creditButton;
    public GameObject ScorebordCanvas;
    public Button ScorebordCanvasFirstButton;

    private bool inputLocked = false;
    private GameObject lastActiveButtons = null;

    void Update()
    {
        if (inputLocked)
        {
            // Z�L�[�𗣂��̂�҂�
            if (!Input.GetKey(KeyCode.Z))
            {
                inputLocked = false;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null)
            {
                UnityEngine.UI.Button button = selected.GetComponent<UnityEngine.UI.Button>();
                if (button != null)
                {
                    button.onClick.Invoke();
                }
            }
        }
    }

    public void StartGame()
    {
        if (initialButtons != null) initialButtons.SetActive(false);
        if (secondaryButtons != null) secondaryButtons.SetActive(true);

        if (firstSecondaryButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSecondaryButton);
        }

        inputLocked = true; // Z�L�[�A�Ŗh�~
    }

    public void QuitGame()
    {
        Debug.Log("�Q�[���I��");
        Application.Quit();
    }

    public void EasyGameScene()
    {
        SceneManager.LoadScene("EasyGameScene"); // ���ۂ̃Q�[���V�[�����ɕύX
    }
    public void ScoreGameScene()
    {
        SceneManager.LoadScene("ScoreGameScene"); // ���ۂ̃Q�[���V�[�����ɕύX
    }

    public void TimeGameScene()
    {
        SceneManager.LoadScene("TimeGameScene"); // ���ۂ̃Q�[���V�[�����ɕύX
    }

    public void ReturnToTitle()
    {
        if (secondaryButtons != null) secondaryButtons.SetActive(false);
        if (initialButtons != null) initialButtons.SetActive(true);

        UnityEngine.UI.Button firstInitial = initialButtons.GetComponentInChildren<UnityEngine.UI.Button>();
        if (firstInitial != null)
            EventSystem.current.SetSelectedGameObject(firstInitial.gameObject);
    }
    public void ShowTutorial()
    {
        if (tutorialCanvas != null) tutorialCanvas.SetActive(true);
        if (MainMenuCanvas != null) MainMenuCanvas.SetActive(false);

        // �J�[�\���� NextButton1 �ɍ��킹�鏈����ǉ�
        if (tutorialFirstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(tutorialFirstButton.gameObject);
        }
    }

    public void ShowScorebord()
    {
        if (ScorebordCanvas != null) ScorebordCanvas.SetActive(true);
        if (MainMenuCanvas != null) MainMenuCanvas.SetActive(false);

        // �J�[�\���� NextButton1 �ɍ��킹�鏈����ǉ�
        if (tutorialFirstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(ScorebordCanvasFirstButton.gameObject);
        }
    }

    public void ShowOption()
    {
        if (initialButtons != null && initialButtons.activeSelf)
            lastActiveButtons = initialButtons;
        else if (secondaryButtons != null && secondaryButtons.activeSelf)
            lastActiveButtons = secondaryButtons;

        if (optionCanvas != null) optionCanvas.SetActive(true);
        if (initialButtons != null) initialButtons.SetActive(false); // ���{�^�����\���ɁI
        if (secondaryButtons != null) secondaryButtons.SetActive(false);
        if (optionButton != null) optionButton.SetActive(false);
        if (creditButton != null) creditButton.SetActive(false);

        if (optionBackButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(optionBackButton.gameObject);

            if (cursor != null)
            {
                cursor.position = optionBackButton.position;
            }
        }

        inputLocked = true;
    }
    public void CloseOption()
    {
        if (optionCanvas != null) optionCanvas.SetActive(false);
        if (optionButton != null) optionButton.SetActive(true);
        if (creditButton != null) creditButton.SetActive(true);

        // ���Ƃ������j���[�ɖ߂�
        if (lastActiveButtons != null)
        {
            lastActiveButtons.SetActive(true);

            UnityEngine.UI.Button firstButton = lastActiveButtons.GetComponentInChildren<UnityEngine.UI.Button>();
            if (firstButton != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
            }
        }
    }

    public void ShowCredits()
    {
        if (initialButtons != null && initialButtons.activeSelf)
            lastActiveButtons = initialButtons;
        else if (secondaryButtons != null && secondaryButtons.activeSelf)
            lastActiveButtons = secondaryButtons;

        if (creditCanvas != null) creditCanvas.SetActive(true);
        if (initialButtons != null) initialButtons.SetActive(false); // ���{�^�����\���ɁI
        if (secondaryButtons != null) secondaryButtons.SetActive(false);
        if (optionButton != null) optionButton.SetActive(false);
        if (creditButton != null) creditButton.SetActive(false);

        if (creditFirstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(creditFirstButton.gameObject);
        }
    }

    public void BackFromCredits()
    {
        if (creditCanvas != null) creditCanvas.SetActive(false);
        if (optionButton != null) optionButton.SetActive(true);
        if (creditButton != null) creditButton.SetActive(true);

        if (lastActiveButtons != null)
        {
            lastActiveButtons.SetActive(true);

            UnityEngine.UI.Button firstButton = lastActiveButtons.GetComponentInChildren<UnityEngine.UI.Button>();
            if (firstButton != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
            }
        }
    }

}
