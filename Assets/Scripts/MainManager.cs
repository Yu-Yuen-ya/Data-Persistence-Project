using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text BestScoreText;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;
    private bool m_GameOver = false;

    private string playerName;        // aktueller Spielername aus StartMenu
    private string bestPlayerName;    // Name, der Highscore hält
    private int bestScore;            // Highscore-Wert

    [System.Serializable]
    class SaveData
    {
        public string bestPlayerName;
        public int bestScore;
    }

    void Awake()
    {
        // Spielername aus StartMenu übernehmen
        playerName = PlayerPrefs.GetString("PlayerName", "");

        // Highscore laden
        LoadHighScore();

        // Anzeige initial aktualisieren
        UpdateBestScoreUI();
    }

    void Start()
    {
        // Score starten
        m_Points = 0;
        ScoreText.text = $"Score : {m_Points}";

        // Bricks erzeugen
        const float step = 0.6f;
        int perLine = 6;
        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };

        for (int i = 0; i < LineCount; i++)
        {
            for (int x = 0; x < perLine; x++)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                Vector3 forceDir = new Vector3(Random.Range(-1f, 1f), 1, 0).normalized;
                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);

        // Prüfen, ob neuer Highscore erreicht wurde
        if (m_Points > bestScore)
        {
            bestScore = m_Points;
            bestPlayerName = playerName; // Name aus StartMenu übernehmen
            SaveHighScore();
        }

        UpdateBestScoreUI();
    }

    void UpdateBestScoreUI()
    {
        BestScoreText.text = $"Best Score : {bestPlayerName} : {bestScore}";
    }

    void SaveHighScore()
    {
        SaveData data = new SaveData
        {
            bestPlayerName = bestPlayerName,
            bestScore = bestScore
        };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    void LoadHighScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            bestPlayerName = data.bestPlayerName;
            bestScore = data.bestScore;
        }
        else
        {
            bestPlayerName = "";
            bestScore = 0;
        }
    }
}
