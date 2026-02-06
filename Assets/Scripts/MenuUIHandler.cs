using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class MenuUIHandler : MonoBehaviour
{
    public TMP_InputField nameInput;          // InputField für Spielername
    public TextMeshProUGUI bestScoreText;     // Anzeige für Best Score

    private string savedBestPlayerName = "";
    private int savedBestScore = 0;

    void Start()
    {
        // Vorbelegen des InputFields mit zuletzt eingegebenem Spielernamen
        nameInput.text = PlayerPrefs.GetString("PlayerName", "");

        // Highscore laden und zwischenspeichern
        LoadHighScore();

        // Listener für Textänderungen → live BestScoreText aktualisieren
        nameInput.onValueChanged.AddListener(OnNameChanged);
    }

    // StartButton klick
    public void StartGame()
    {
        // Spielername speichern, damit MainScene ihn übernehmen kann
        PlayerPrefs.SetString("PlayerName", nameInput.text);

        // Main-Szene laden
        SceneManager.LoadScene("main");
    }

    // QuitButton klick
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    // Highscore aus JSON laden
    void LoadHighScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            savedBestPlayerName = data.bestPlayerName;
            savedBestScore = data.bestScore;

            bestScoreText.text = $"Best Score : {savedBestPlayerName} : {savedBestScore}";
        }
        else
        {
            savedBestPlayerName = "";
            savedBestScore = 0;
            bestScoreText.text = "Best Score :  : 0";
        }
    }

    // Wird aufgerufen, sobald der Spieler den Namen ändert
    void OnNameChanged(string newName)
    {
        // Live-Anzeige: alter Score, aber neuer Name
        bestScoreText.text = $"Best Score : {newName} : {savedBestScore}";
    }

    [System.Serializable]
    class SaveData
    {
        public string bestPlayerName;
        public int bestScore;
    }
}
