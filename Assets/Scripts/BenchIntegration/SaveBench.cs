using UnityEngine;
using Terresquall;
using TMPro;

public class SaveBench : MonoBehaviour
{
    [Header("Save Settings")]
    public KeyCode interactKey = KeyCode.J;

    [Header("Player Detection")]
    public string playerTag = "Player";

    [Header("Hint UI")]
    public GameObject hintCanvas;
    public string savedText = "Game Saved";
    public string hintText = "Press J to Save";

    private bool playerInRange = false;
    private TMP_Text hintTextMesh;

    private void Awake()
    {
        if (hintCanvas != null)
        {
            hintTextMesh = hintCanvas.GetComponentInChildren<TMP_Text>(true);
            hintCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            SaveAtBench();
        }
    }

    private void SaveAtBench()
    {
        int targetSlot = Bench.currentSlot;

        Bench.SaveGame(targetSlot);

        if (hintTextMesh != null)
        {
            hintTextMesh.text = savedText + " - File " + (targetSlot + 1);
        }

        Debug.Log("Game saved at bench. Slot: " + targetSlot);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            playerInRange = true;

            if (hintTextMesh != null)
            {
                hintTextMesh.text = hintText;
            }

            if (hintCanvas != null)
            {
                hintCanvas.SetActive(true);
            }

            Debug.Log("Player entered save bench range. Press " + interactKey + " to save.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            playerInRange = false;

            if (hintCanvas != null)
            {
                hintCanvas.SetActive(false);
            }

            Debug.Log("Player left save bench range.");
        }
    }
}