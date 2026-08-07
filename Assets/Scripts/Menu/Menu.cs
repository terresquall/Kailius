using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public GameObject saveSlotsUI;

    public void PlayGame() {
        if (saveSlotsUI != null) {
            saveSlotsUI.SetActive(true);
        }
    }

    public void ResumeGame() {
        Time.timeScale = 1;
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void PlayAgain() {
        SceneManager.LoadScene("Menu");
    }
}