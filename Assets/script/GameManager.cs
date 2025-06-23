// 宝石落下ゲームの基礎枠組み（2D、Unity C#）

using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static OfflineRankingSystem;  // ← Mode を省略できるように
using System.Collections;


public enum GameMode
{
    Easy,
    ScoreAttack,
    TimeAttack
}

public class GameManager : MonoBehaviour
{
    public GameMode currentMode = GameMode.ScoreAttack; // モード選択
    public GameObject GameCanvas;
    public UnityEngine.UI.Button pauseRetryButton;
    public UnityEngine.UI.Button pauseMenuButton;
    public UnityEngine.UI.Button resumeButton;
    public GameObject tutorialCanvas;
    public Button tutorialFirstButton;
    public GameObject pauseMenuUI;
    public GameObject pauseCursor;

    public UnityEngine.UI.Button retryButton;
    public UnityEngine.UI.Button menuButton;
    public GameObject gameOverCursor;
    public TextMeshProUGUI gameoverScoreText;
    public GameObject[] gemPrefabs;
    public Transform spawnPoint;

    public GameObject scoreCanvasUI;
    public GameObject scoreCursor;
    public TextMeshProUGUI finalScoreText;
    public UnityEngine.UI.Button scoreRetryButton;
    public UnityEngine.UI.Button scoreMenuButton;

    private List<GameObject> usableGems = new List<GameObject>();
    private GameObject currentGem;
    private GameObject nextGemPreview;
    public float moveSpeed = 5f;
    public float spawnGemScale = 1.0f;
    public float gemFallSpeed = 1.0f;
    public float minX = -2.5f;
    public float maxX = 2.5f;

    public GameObject whiteGemPrefab;

    private Dictionary<string, string> oppositeColors = new Dictionary<string, string>()
    {
        {"bluegem", "orangegem"},
        {"orangegem", "bluegem"},
        {"greengem", "redgem"},
        {"redgem", "greengem"},
        {"pinkgem", "skybluegem"},
        {"skybluegem", "pinkgem"},
        {"purplegem", "yellowgem"},
        {"yellowgem", "purplegem"},
    };

    private HashSet<string> collidedPairs = new HashSet<string>();
    private int score = -100;
    private const int maxScore = 9999999;
    public AudioSource bgmSource;
    public AudioClip mergeClip;
    public AudioClip blackCreateClip;
    public AudioClip blackDestroyClip;
    public AudioClip gameOverClip;
    public AudioClip destroyClip;
    public GameObject gameOverUI;
    public TextMeshProUGUI scoreText;
    private bool isGameOver = false;
    private bool isPaused = false;
    private bool isTimeAtacck = false;
    private float elapsedTime = 0f;
    private bool hasRecordedRapTime = false;
    private string rapTimeText = "";
    public TextMeshProUGUI rapText;
    public TextMeshProUGUI timerText;
    public float gameOverHeight = 4.5f;
    public float gameOverHeight2 = 6.0f;


    private bool inputLocked = false;
    private const float timeAttackLimit = 180f; // 3分間

    void Start()
    {
        Time.timeScale = 1f; // シーン開始時にタイムスケールをリセット
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (pauseRetryButton != null) pauseRetryButton.onClick.AddListener(RestartGame);
        if (pauseMenuButton != null) pauseMenuButton.onClick.AddListener(ReturnToMenu);
        if (retryButton != null) retryButton.onClick.AddListener(RestartGame);
        if (menuButton != null) menuButton.onClick.AddListener(ReturnToMenu);
        if (scoreRetryButton != null) scoreRetryButton.onClick.AddListener(RestartGame);
        if (scoreMenuButton != null) scoreMenuButton.onClick.AddListener(ReturnToMenu);
        // CreateGameOverHeightLine();
        if (nextGemPreview != null) Destroy(nextGemPreview);
        if (bgmSource != null) bgmSource.Play();
        Debug.Log("GameManager Start() called");
        foreach (var gem in gemPrefabs)
        {
            string gemName = gem.name.ToLower();

            if (gemName.Contains("black")) continue;

            // Easyモードのときは skybluegem と pinkgem を除外
            if (currentMode == GameMode.Easy && (gemName.Contains("skybluegem") || gemName.Contains("pinkgem")))
                continue;

            usableGems.Add(gem);
        }
        SpawnGem();
        DrawGameOverLine();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
        if (!isGameOver && !isPaused && !isTimeAtacck)
        {
            if (timerText != null)
            {
                if (currentMode == GameMode.TimeAttack)
                {
                    float remainingTime = Mathf.Max(timeAttackLimit - elapsedTime, 0f);
                    int minutes = Mathf.FloorToInt(remainingTime / 60);
                    int seconds = Mathf.FloorToInt(remainingTime % 60);
                    timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
                }
                else
                {
                    int hours = Mathf.FloorToInt(elapsedTime / 3600);
                    int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
                    int seconds = Mathf.FloorToInt(elapsedTime % 60);
                    timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
                }

            }
            elapsedTime += Time.deltaTime;

            if (currentMode == GameMode.TimeAttack && elapsedTime >= timeAttackLimit)
            {
                isTimeAtacck = true;
                CancelInvoke("SpawnGem");
                Debug.Log("Time Attack Ended");

                StartCoroutine(ReplaceWithWhiteGemsAndShowResult(true)); // ← 修正ポイント

                if (score >= maxScore)
                {
                    OfflineRankingSystem.SaveScore((OfflineRankingSystem.Mode)System.Enum.Parse(typeof(OfflineRankingSystem.Mode), currentMode.ToString()),
                        score,
                        rapTimeText
                        );
                }
                else
                {
                    OfflineRankingSystem.SaveScore((OfflineRankingSystem.Mode)System.Enum.Parse(typeof(OfflineRankingSystem.Mode), currentMode.ToString()),
                        score
                        );
                }

                return;
            }
        }
            if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.Min(score, maxScore).ToString();
        }

        if (!hasRecordedRapTime && score >= maxScore)
        {
            int hours = Mathf.FloorToInt(elapsedTime / 3600);
            int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            rapTimeText = string.Format("RAP: {0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
            hasRecordedRapTime = true;
        }

        if (rapText != null)
        {
            rapText.text = rapTimeText;
        }

        foreach (GameObject gem in GameObject.FindGameObjectsWithTag("Gem"))
        {
            if (gem == currentGem) continue;

            var rb = gem.GetComponent<Rigidbody2D>();
            var gb = gem.GetComponent<GemBehavior>();
            var sr = gem.GetComponent<SpriteRenderer>();

            if (rb == null || gb == null) continue;

            float gemHeight = sr.bounds.size.y; // スプライトの高さ
            float gemBottom = gem.transform.position.y - gemHeight / 2f + gemHeight / 3f; // Gemの底辺のY座標
            float gemTop = gem.transform.position.y + gemHeight / 2f;

            // 床または他の Gem に触れたことがある Gem のみ対象
            if (!isGameOver && rb.simulated && gb.hasTouchedSomething && gemBottom > gameOverHeight)
            {
                TriggerGameOver(gem);
                return;
            }

            if (!isGameOver && rb.simulated && gb.hasTouchedSomething && gemTop > gameOverHeight2)
            {
                TriggerGameOver(gem);
                return;
            }
        }


        if (currentGem != null && !isPaused && !isGameOver && !isTimeAtacck)
        {
            float move = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            Vector3 newPosition = currentGem.transform.position + new Vector3(move, 0, 0);
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            currentGem.transform.position = newPosition;

            if (Input.GetKeyDown(KeyCode.Z) || Input.GetAxisRaw("Vertical") < 0f && !inputLocked)
            {
                Rigidbody2D rb = currentGem.GetComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Dynamic; // 実際に落下させる
                currentGem = null;
                Invoke("SpawnGem", 1.0f);
                inputLocked = true;
            }
        }

        if ((isPaused || isGameOver || isTimeAtacck) && !inputLocked)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                GameObject selected = EventSystem.current.currentSelectedGameObject;
                if (selected != null)
                {
                    var button = selected.GetComponent<UnityEngine.UI.Button>();
                    if (button != null)
                    {
                        button.onClick.Invoke();
                        inputLocked = true;
                    }
                }
            }
        }
        else if (inputLocked && !Input.GetKey(KeyCode.Z))
        {
            inputLocked = false;  // ボタンが離されたらロック解除
        }
    }

    void TriggerGameOver(GameObject gem)
    {
        isGameOver = true;
        CancelInvoke("SpawnGem");
        Debug.Log($"Game Over! Gem {gem.name} stopped at Y={gem.transform.position.y}");

        StartCoroutine(ReplaceWithWhiteGemsAndShowResult(false)); // ← 修正ポイント

        if (score >= maxScore)
        {
            OfflineRankingSystem.SaveScore((OfflineRankingSystem.Mode)System.Enum.Parse(typeof(OfflineRankingSystem.Mode), currentMode.ToString()),
                score,
                rapTimeText
                );
        }
        else
        {
            OfflineRankingSystem.SaveScore((OfflineRankingSystem.Mode)System.Enum.Parse(typeof(OfflineRankingSystem.Mode), currentMode.ToString()),
                score
                );

        }
    }

        void ReturnToMenu()
    {
        Time.timeScale = 1f; // メニューに戻る前にタイムスケールをリセット
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(isPaused);
        if (pauseCursor != null) pauseCursor.SetActive(isPaused);
        if (isPaused)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject); // 最初に選ぶボタン
        }
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        inputLocked = true;
        Invoke(nameof(UnlockInputAfterDelay), 0.5f);
    }

    private void UnlockInputAfterDelay()
    {
        inputLocked = false;
    }

    void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void ShowTutorial()
    {
        if (tutorialCanvas != null) tutorialCanvas.SetActive(true);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (GameCanvas != null) GameCanvas.SetActive(false);

        // カーソルを NextButton1 に合わせる処理を追加
        if (tutorialFirstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(tutorialFirstButton.gameObject);
        }
    }

    void SpawnGem()
    {
        score += 100;
        if (score > maxScore) score = maxScore;
        Debug.Log("Score: " + score);
        Debug.Log("Spawning new gem");
        if (usableGems.Count == 0) return;

        GameObject prefabToSpawn;

        if (nextGemPreview != null)
        {
            string baseName = nextGemPreview.name.Replace("(Clone)", "");
            prefabToSpawn = System.Array.Find(gemPrefabs, g => g.name == baseName);
            currentGem = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
            currentGem.transform.localScale = Vector3.one * spawnGemScale;

            Rigidbody2D rb = currentGem.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic; // 初期状態は操作待ち
            rb.simulated = true;
            rb.gravityScale = gemFallSpeed;

            currentGem.name = prefabToSpawn.name + "_" + System.Guid.NewGuid().ToString();
            Destroy(nextGemPreview);

            GemBehavior gemBehavior = currentGem.GetComponent<GemBehavior>();
            if (gemBehavior != null)
            {
                gemBehavior.hasTouchedSomething = false;
            }
        }
        else
        {
            int randomIndex = Random.Range(0, usableGems.Count);
            prefabToSpawn = usableGems[randomIndex];
            currentGem = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
            currentGem.transform.localScale = Vector3.one * spawnGemScale;

            Rigidbody2D rb = currentGem.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
            rb.gravityScale = gemFallSpeed;

            currentGem.name = prefabToSpawn.name + "_" + System.Guid.NewGuid().ToString();

            GemBehavior gemBehavior = currentGem.GetComponent<GemBehavior>();
            if (gemBehavior != null)
            {
                gemBehavior.hasTouchedSomething = false;
            }
        }

        int nextIndex = Random.Range(0, usableGems.Count);
        GameObject nextGem = usableGems[nextIndex];
        nextGemPreview = Instantiate(nextGem, new Vector3(6f, -2.5f, 0), Quaternion.identity);
        nextGemPreview.transform.localScale = Vector3.one * 10f;
        nextGemPreview.GetComponent<Rigidbody2D>().simulated = false;
        nextGemPreview.GetComponent<Collider2D>().enabled = false;
        nextGemPreview.name = nextGem.name;
    }


    public void HandleGemCollision(GameObject a, GameObject b)
    {
        if (a == currentGem || b == currentGem) return;
        if (a == null || b == null) return;
        if (!a.CompareTag("Gem") || !b.CompareTag("Gem")) return;

        string baseNameA = a.name.ToLower().Split('_')[0];
        string baseNameB = b.name.ToLower().Split('_')[0];
        string idA = a.name;
        string idB = b.name;

        string pairKey = idA.CompareTo(idB) < 0 ? idA + "_" + idB : idB + "_" + idA;

        // 黒同士の場合はペアチェックより先に処理
        if (baseNameA.Contains("black") && baseNameB.Contains("black"))
        {
            if (Mathf.Approximately(a.transform.localScale.x, b.transform.localScale.x))
            {
                score += Mathf.RoundToInt(a.transform.localScale.x - 1f) * 5000;
                if (score > maxScore) score = maxScore;
                Debug.Log("Score: " + score);
                if (blackDestroyClip != null) AudioSource.PlayClipAtPoint(blackDestroyClip, a.transform.position);
                Destroy(a);
                Destroy(b);
            }
            return;
        }

        if (collidedPairs.Contains(pairKey)) return;
        collidedPairs.Add(pairKey);

        if (baseNameA == baseNameB && baseNameA != "blackgem")
        {
            Vector3 pos = (a.transform.position + b.transform.position) / 2f;
            float newScale = Mathf.Max(a.transform.localScale.x, b.transform.localScale.x) + 1.5f;
            GameObject mergedGem = Instantiate(a, pos, Quaternion.identity);
            mergedGem.transform.localScale = Vector3.one * newScale;
            mergedGem.name = baseNameA + "_" + System.Guid.NewGuid().ToString();
            if (mergeClip != null) AudioSource.PlayClipAtPoint(mergeClip, pos);
            Destroy(a);
            Destroy(b);
        }
        else if (
            oppositeColors.ContainsKey(baseNameA) &&
            oppositeColors[baseNameA] == baseNameB &&
            Mathf.Approximately(a.transform.localScale.x, b.transform.localScale.x)
        )
        {
            GameObject blackGem = null;
            foreach (var prefab in gemPrefabs)
            {
                if (prefab.name.ToLower().Contains("black"))
                {
                    blackGem = prefab;
                    break;
                }
            }
            if (blackGem != null)
            {
                Vector3 pos = (a.transform.position + b.transform.position) / 2f;
                score += Mathf.RoundToInt(a.transform.localScale.x - 1f) * 1000;
                if (score > maxScore) score = maxScore;
                Debug.Log("Score: " + score);
                if (blackCreateClip != null) AudioSource.PlayClipAtPoint(blackCreateClip, pos);
                Destroy(a);
                Destroy(b);
                GameObject newBlack = Instantiate(blackGem, pos, Quaternion.identity);
                newBlack.transform.localScale = Vector3.one * a.transform.localScale.x;
            }
        }
    }
    void DrawGameOverLine()
    {
        GameObject line = new GameObject("GameOverLine");
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, new Vector3(minX - 1f, gameOverHeight, 0));
        lr.SetPosition(1, new Vector3(maxX + 1f, gameOverHeight, 0));
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.red;
        lr.endColor = Color.red;
    }

    // 1. ReplaceWithWhiteGems を拡張し、演出後に結果表示を行うコルーチンを用意
    IEnumerator ReplaceWithWhiteGemsAndShowResult(bool isTimeAttackMode)
    {
        List<GameObject> gems = new List<GameObject>(GameObject.FindGameObjectsWithTag("Gem"));
        gems.Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));

        Time.timeScale = 0f;

        foreach (GameObject gem in gems)
        {
            if (gem == currentGem || gem == nextGemPreview) continue;

            Vector3 pos = gem.transform.position;
            Vector3 scale = gem.transform.localScale;

            Destroy(gem);

            GameObject whiteGem = Instantiate(whiteGemPrefab, pos, Quaternion.identity);
            whiteGem.transform.localScale = scale;

            whiteGem.name = "WhiteGem_" + System.Guid.NewGuid().ToString();

            yield return new WaitForSecondsRealtime(0.05f);
        }

        FreezeAllGems();
        Time.timeScale = 1f;

        if (gameOverClip != null)
            AudioSource.PlayClipAtPoint(gameOverClip, Camera.main.transform.position, 0.4f);

        if (score >= maxScore)
        {
            OfflineRankingSystem.SaveScore(
                (OfflineRankingSystem.Mode)System.Enum.Parse(typeof(OfflineRankingSystem.Mode), currentMode.ToString()),
                score,
                rapTimeText
            );
        }
        else
        {
            OfflineRankingSystem.SaveScore(
                (OfflineRankingSystem.Mode)System.Enum.Parse(typeof(OfflineRankingSystem.Mode), currentMode.ToString()),
                score
            );
        }

        if (isTimeAttackMode)
        {
            if (scoreCanvasUI != null) scoreCanvasUI.SetActive(true);
            if (scoreCursor != null) scoreCursor.SetActive(true);
            if (finalScoreText != null) finalScoreText.text = "Score: " + Mathf.Min(score, maxScore).ToString();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(scoreRetryButton.gameObject);
        }
        else
        {
            if (gameOverUI != null) gameOverUI.SetActive(true);
            if (gameOverCursor != null) gameOverCursor.SetActive(true);
            if (gameoverScoreText != null) gameoverScoreText.text = "Score: " + Mathf.Min(score, maxScore).ToString();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(retryButton.gameObject);
        }
    }

    void FreezeAllGems()
    {
        foreach (var gem in GameObject.FindGameObjectsWithTag("WhiteGem"))
        {
            Rigidbody2D rb = gem.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }
}