using UnityEngine;
using TMPro;
using System.Collections.Generic;
using static OfflineRankingSystem; // �� �����Mode�����ȗ��\

public class ScoreboardCanvasManager : MonoBehaviour
{
    public GameObject entryTemplate;
    public Transform entryContainer;

    public Mode currentMode = Mode.ScoreAttack;

    void Start()
    {
        // �f�o�b�O�p�F�X�R�A�������i�����Ƃŏ����j
        //PlayerPrefs.DeleteKey("Ranking_Easy");
        //PlayerPrefs.DeleteKey("Ranking_ScoreAttack");
        //PlayerPrefs.DeleteKey("Ranking_TimeAttack");
        //PlayerPrefs.Save();
        UpdateRankingUI(currentMode);
    }

    public void UpdateRankingUI(OfflineRankingSystem.Mode mode)
    {

        // �����̃G���g�����폜
        foreach (Transform child in entryContainer)
        {
            if (child != entryTemplate.transform)
                Destroy(child.gameObject);
        }

        var entries = OfflineRankingSystem.LoadScores(mode);

        for (int i = 0; i < 5; i++)
        {
            GameObject obj = Instantiate(entryTemplate, entryContainer);
            obj.SetActive(true);

            // ���O�ŌʂɎ擾
            TextMeshProUGUI rankText = obj.transform.Find("RankText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI scoreText = obj.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI rapText = obj.transform.Find("RapTimeText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI dateText = obj.transform.Find("DateText").GetComponent<TextMeshProUGUI>();


            rankText.text = $"{i + 1}�ʁF";

            if (i < entries.Count)
            {
                var entry = entries[i];
                scoreText.text = entry.score.ToString("D7");

                if (entry.score == 9999999 && !string.IsNullOrEmpty(entry.rapTime))
                {
                    rapText.text = $"[{entry.rapTime}]";
                }
                else
                {
                    rapText.text = "";
                }

                dateText.text = $"�F{entry.date}";
            }
            else
            {
                scoreText.text = "---";
                dateText.text = "�F---";

            }

        }

    }

}
