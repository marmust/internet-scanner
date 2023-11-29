using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void GoGraphButton()
    {
        // load graph scene
        SceneManager.LoadScene(1);
        // unload main menu scene
        SceneManager.UnloadScene(0);
    }

    public void GoCreditsButton()
    {
        // load credits scene
        SceneManager.LoadScene(2);
        // unload main menu scene
        SceneManager.UnloadScene(0);
    }

    public void GoTutorialButton()
    {
        // load Tutorial scene
        SceneManager.LoadScene(3);
        // unload main menu scene
        SceneManager.UnloadScene(0);
    }

  public void GoQuitButton()
    {
        print("exiting");
        Application.Quit();
    }
}
