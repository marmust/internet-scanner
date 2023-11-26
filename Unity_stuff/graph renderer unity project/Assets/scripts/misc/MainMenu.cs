using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void GoGraphButton()
    {
        // load graph scene
        SceneManager.LoadScene(0);
        // unload main menu scene
        SceneManager.UnloadScene(1);
    }

    public void GoCreditsButton()
    {
        // load credits scene
        SceneManager.LoadScene(2);
        // unload main menu scene
        SceneManager.UnloadScene(1);
    }

    public void GoQuitButton()
    {
        print("exiting");
        Application.Quit();
    }
}
