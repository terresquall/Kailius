using UnityEngine;
using Terresquall;

public class BenchSaveHotkeys : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Bench.SaveGame(0);
            Debug.Log("Game saved to Bench slot 0.");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Bench.LoadGame(0);
            Debug.Log("Game loaded from Bench slot 0.");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Bench.Delete(0);
            Debug.Log("Bench slot 0 deleted.");
        }
    }
}