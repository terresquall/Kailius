using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Terresquall;

public class TitleSaveMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    public string firstGameSceneName = "Scene_1";

    [Header("Panels")]
    public GameObject slotMenuPanel;
    public GameObject actionPanel;

    [Header("Slot Buttons")]
    public Button[] slotButtons;
    public TMP_Text[] slotTexts;

    [Header("Action Buttons")]
    public Button newGameButton;
    public Button overwriteButton;
    public Button loadGameButton;

    private int selectedSlot = -1;

    private void Awake()
    {
        SetupButtons();

        if (slotMenuPanel != null)
        {
            slotMenuPanel.SetActive(false);
        }

        if (actionPanel != null)
        {
            actionPanel.SetActive(false);
        }
    }

    private void SetupButtons()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int slotIndex = i;

            if (slotButtons[i] != null)
            {
                slotButtons[i].onClick.RemoveAllListeners();
                slotButtons[i].onClick.AddListener(() => SelectSlot(slotIndex));
            }
        }

        if (newGameButton != null)
        {
            newGameButton.onClick.RemoveAllListeners();
            newGameButton.onClick.AddListener(NewGame);
        }

        if (overwriteButton != null)
        {
            overwriteButton.onClick.RemoveAllListeners();
            overwriteButton.onClick.AddListener(Overwrite);
        }

        if (loadGameButton != null)
        {
            loadGameButton.onClick.RemoveAllListeners();
            loadGameButton.onClick.AddListener(LoadGame);
        }
    }

    public void OpenSlotMenu()
    {
        selectedSlot = -1;

        RefreshSlotTexts();

        if (slotMenuPanel != null)
        {
            slotMenuPanel.SetActive(true);
        }

        if (actionPanel != null)
        {
            actionPanel.SetActive(false);
        }

        Debug.Log("Save slot menu opened.");
    }

    private void RefreshSlotTexts()
    {
        for (int i = 0; i < slotTexts.Length; i++)
        {
            if (slotTexts[i] == null)
            {
                continue;
            }

            bool hasSave = Bench.SlotHasSave(i);

            if (hasSave)
            {
                slotTexts[i].text = "Load File " + (i + 1);
            }
            else
            {
                slotTexts[i].text = "Empty File " + (i + 1);
            }
        }
    }

    private void SelectSlot(int slot)
    {
        selectedSlot = slot;

        bool hasSave = Bench.SlotHasSave(selectedSlot);

        if (actionPanel != null)
        {
            actionPanel.SetActive(true);
        }

        if (newGameButton != null)
        {
            newGameButton.gameObject.SetActive(!hasSave);
        }

        if (overwriteButton != null)
        {
            overwriteButton.gameObject.SetActive(hasSave);
        }

        if (loadGameButton != null)
        {
            loadGameButton.gameObject.SetActive(hasSave);
        }

        Debug.Log("Selected save slot: " + selectedSlot + ". Has save: " + hasSave);
    }

    private void NewGame()
    {
        if (selectedSlot < 0)
        {
            Debug.LogWarning("No save slot selected.");
            return;
        }

        StartNewGameOnSlot(selectedSlot);
    }

    private void Overwrite()
    {
        if (selectedSlot < 0)
        {
            Debug.LogWarning("No save slot selected.");
            return;
        }

        Bench.Delete(selectedSlot);
        StartNewGameOnSlot(selectedSlot);
    }

    private void StartNewGameOnSlot(int slot)
    {
        Bench.currentSlot = slot;

        PlayerPrefs.SetInt("AttackDamage", 0);
        PlayerPrefs.SetInt("Defense", 0);
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("ScoreCoins", 0);
        PlayerPrefs.SetInt("ScoreGems", 0);
        PlayerPrefs.SetInt("ScoreStars", 0);

        Debug.Log("Starting new game on slot: " + slot);

        SceneManager.LoadScene(firstGameSceneName);
    }

    private void LoadGame()
    {
        if (selectedSlot < 0)
        {
            Debug.LogWarning("No save slot selected.");
            return;
        }

        if (!Bench.SlotHasSave(selectedSlot))
        {
            Debug.LogWarning("Selected slot has no save file.");
            return;
        }

        Bench.currentSlot = selectedSlot;

        string targetSceneName = firstGameSceneName;

        Dictionary<string, string> metadata = Bench.PeekGame(selectedSlot);

        if (metadata != null && metadata.ContainsKey("current_scene_name"))
        {
            targetSceneName = metadata["current_scene_name"];
        }

        Debug.Log("Loading save slot " + selectedSlot + " from scene: " + targetSceneName);

        StartCoroutine(LoadSceneThenLoadGame(targetSceneName, selectedSlot));
    }

    private IEnumerator LoadSceneThenLoadGame(string sceneName, int slot)
    {
        // Keep this object alive long enough to load the game after the scene changes.
        DontDestroyOnLoad(gameObject);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        // Wait one frame so PlayerBenchSave and ScoreManager can initialize.
        yield return null;

        Bench.LoadGame(slot);

        Debug.Log("Loaded Bench save slot: " + slot);

        Destroy(gameObject);
    }
}