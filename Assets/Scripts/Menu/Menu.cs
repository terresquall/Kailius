using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public TitleSaveMenu titleSaveMenu;
    //public void PlayGame () {
    //    PlayerPrefs.SetInt("AttackDamage", 0);
    //    PlayerPrefs.SetInt("Defense", 0);
    //    PlayerPrefs.SetInt("Score", 0);
    //    PlayerPrefs.SetInt("ScoreCoins", 0);
    //    PlayerPrefs.SetInt("ScoreGems", 0);
    //    PlayerPrefs.SetInt("ScoreStars", 0);
    //    SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
    //}
    public void PlayGame()
    {
        if (titleSaveMenu != null)
        {
            titleSaveMenu.OpenSlotMenu();
            return;
        }

        TitleSaveMenu foundMenu = FindObjectOfType<TitleSaveMenu>();

        if (foundMenu != null)
        {
            foundMenu.OpenSlotMenu();
        }
        else
        {
            Debug.LogWarning("No TitleSaveMenu found in the title scene.");
        }
    }

    public void ResumeGame() {
        Time.timeScale = 1;  
    }

    public void QuitGame () {
        Application.Quit();
    }

    public void PlayAgain() {
        SceneManager.LoadScene("Menu");
    }
}