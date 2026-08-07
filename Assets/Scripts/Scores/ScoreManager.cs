using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Terresquall;

public class ScoreManager : PersistentObject {

    private const string DEFAULT_SAVE_ID = "ScoreManagerData";

    public static ScoreManager instance;

    public TextMeshProUGUI Score;
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textCoins;
    public TextMeshProUGUI textGems;
    public TextMeshProUGUI textStars;

    // Wrappers for ScoreManager variables, which draw data
    // from the save data directly.
    private int score { 
        get { return data.score; } 
        set { data.score = value; }
    }
    private int scoreCoins {
        get { return data.scoreCoins; } 
        set { data.scoreCoins = value; }
    }
    private int scoreGems { 
        get { return data.scoreGems; }
        set { data.scoreGems = value; }
    }
    private int scoreStars { 
        get { return data.scoreStars; }
        set { data.scoreStars = value; }
    }

    // We will use the save data to store runtime values directly.
    [System.Serializable]
    public new class SaveData : PersistentObject.SaveData {
        public int score, scoreCoins, scoreGems, scoreStars;
    }
    public SaveData data = new SaveData();

    // The Save function just needs to record the saveID.
    // Everything else is updated during runtime.
    public override PersistentObject.SaveData Save() {
        if (!CanSave()) return null;
        data.saveID = saveID;
        return data;
    }

    // We need to cast the data to our version of SaveData before using it.
    public override bool Load(PersistentObject.SaveData loadedData) {
        SaveData scoreData = loadedData as SaveData;

        if (scoreData == null) return false;

        data = scoreData;
        RefreshUI();
        Debug.Log("Score data loaded with Bench Save System.");

        return true;
    }

    private static bool created = false;

    // Override the default reset function, which assigns it a random save ID.
    // We always want the same save ID to be used by default.
    protected override void Reset() {
        // Automatically assign the save ID if it is empty.
        if (string.IsNullOrEmpty(saveID)) {
            saveID = DEFAULT_SAVE_ID;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.score = PlayerPrefs.GetInt("Score", 0);
        this.scoreCoins = PlayerPrefs.GetInt("ScoreCoins", 0);
        this.scoreGems = PlayerPrefs.GetInt("ScoreGems", 0);
        this.scoreStars = PlayerPrefs.GetInt("ScoreStars", 0);

        Bench.SaveFile currentSave = Bench.GetCurrentSaveFile();

        if (currentSave != null && currentSave.slot == Bench.currentSlot) {
            Bench.QuickLoad(this);
        }

        RefreshUI();
    }

    public void ChangeScore(int scoreValue)
    {
        score += scoreValue;
        RefreshUI();

        PlayerPrefs.SetInt("Score", score);
    }

    public void ChangeScoreCoin(int coinValue)
    {
        scoreCoins += coinValue;
        RefreshUI();

        PlayerPrefs.SetInt("ScoreCoins", scoreCoins);
    }

    public void ChangeScoreGem(int gemValue)
    {
        scoreGems += gemValue;
        RefreshUI();

        PlayerPrefs.SetInt("ScoreGems", scoreGems);
    }

    public void ChangeScoreStar(int starsValue)
    {
        scoreStars += starsValue;
        RefreshUI();

        PlayerPrefs.SetInt("ScoreStars", scoreStars);
    }

    private void RefreshUI()
    {
        if (Score != null)
        {
            Score.text = score.ToString();
        }

        if (textScore != null)
        {
            textScore.text = score.ToString();
        }

        if (textCoins != null)
        {
            textCoins.text = "150/" + scoreCoins.ToString();
        }

        if (textGems != null)
        {
            textGems.text = "60/" + scoreGems.ToString();
        }

        if (textStars != null)
        {
            textStars.text = "3/" + scoreStars.ToString();
        }
    }

    private void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("ScoreCoins", scoreCoins);
        PlayerPrefs.SetInt("ScoreGems", scoreGems);
        PlayerPrefs.SetInt("ScoreStars", scoreStars);
    }
    public int getScoreTotal()
    {
        return this.score;
    }

    public int getScoreStars()
    {
        return this.scoreStars;
    }
}