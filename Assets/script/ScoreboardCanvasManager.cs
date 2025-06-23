using UnityEngine;
using TMPro;
using System.Collections.Generic;
using static OfflineRankingSystem; // ← これでModeだけ省略可能

public class ScoreboardCanvasManager : MonoBehaviour
{
    public GameObject entryTemplate;
    public Transform entryContainer;

    public Mode currentMode = Mode.ScoreAttack;

    void Start()
    {
        // デバッグ用：スコア初期化（※あとで消す）
        //PlayerPrefs.DeleteKey("Ranking_Easy");
        //PlayerPrefs.DeleteKey("Ranking_ScoreAttack");
        //PlayerPrefs.DeleteKey("Ranking_TimeAttack");
        //PlayerPrefs.Save();
        UpdateRankingUI(currentMode);
    }

    public void UpdateRankingUI(OfflineRankingSystem.Mode mode)
    {

        // 既存のエントリを削除
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

            // 名前で個別に取得
            TextMeshProUGUI rankText = obj.transform.Find("RankText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI scoreText = obj.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI rapText = obj.transform.Find("RapTimeText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI dateText = obj.transform.Find("DateText").GetComponent<TextMeshProUGUI>();


            rankText.text = $"{i + 1}位：";

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

                dateText.text = $"：{entry.date}";
            }
            else
            {
                scoreText.text = "---";
                dateText.text = "：---";

            }

        }

    }

}
