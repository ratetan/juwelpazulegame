using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ScoreEntry
{
    public int score;
    public string date;
    public string rapTime;
}

[System.Serializable]
public class ScoreList
{
    public List<ScoreEntry> entries = new List<ScoreEntry>();
}

public static class OfflineRankingSystem
{
    private const int MaxRankings = 5;

    public enum Mode
    {
        Easy,
        ScoreAttack,
        TimeAttack
    }

    private static string GetKey(Mode mode)
    {
        return $"Ranking_{mode}";
    }

    public static void SaveScore(Mode mode, int score, string rapTime = "")
    {
        string key = GetKey(mode);
        string json = PlayerPrefs.GetString(key, "{}");

        ScoreList scoreList = JsonUtility.FromJson<ScoreList>(json) ?? new ScoreList();

        scoreList.entries.Add(new ScoreEntry
        {
            score = score,
            date = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
            rapTime = rapTime
        });

        // ���ʕt��
        scoreList.entries = scoreList.entries
            .OrderByDescending(e => e.score)
            .ThenBy(e => ParseRapTimeToSeconds(e.rapTime)) // rapTime���Z��������
            .ThenByDescending(e => DateTime.Parse(e.date)) // �V�������t��D��
            .Take(MaxRankings)
            .ToList();

        string updatedJson = JsonUtility.ToJson(scoreList);
        PlayerPrefs.SetString(key, updatedJson);
        PlayerPrefs.Save();
    }

    public static List<ScoreEntry> LoadScores(Mode mode)
    {
        string key = GetKey(mode);
        string json = PlayerPrefs.GetString(key, "{}");

        ScoreList scoreList = JsonUtility.FromJson<ScoreList>(json);
        return scoreList?.entries ?? new List<ScoreEntry>();
    }

    private static int ParseRapTimeToSeconds(string rapTime)
    {
        if (string.IsNullOrWhiteSpace(rapTime)) return int.MaxValue;

        // "RAP: 00:00:09" �̂悤�ȕ����񂩂� "00:00:09" �𔲂��o��
        string clean = rapTime.Replace("RAP: ", "").Trim();

        if (TimeSpan.TryParse(clean, out TimeSpan ts))
        {
            return (int)ts.TotalSeconds;
        }

        return int.MaxValue;
    }
}
