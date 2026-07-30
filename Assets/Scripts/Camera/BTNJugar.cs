using UnityEngine;

public class BTNJugar : MonoBehaviour
{
    public TitleSaveMenu titleSaveMenu;

    private void OnMouseDown()
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
}