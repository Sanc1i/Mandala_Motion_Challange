using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameMenuController : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Exiting Game...");

        // Logic to close the application
#if UNITY_EDITOR
        // If running in the Unity Editor, stop playing
        EditorApplication.isPlaying = false;
#else
            // If running as a built application, quit
            Application.Quit();
#endif
    }
}