using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuSystem : MonoBehaviour
{
    public VarHolder vars;
    public GameObject PauseMenuObject;

    private void Update()
    {
        PauseMenuObject.active = vars.IsPaused;
    }

    public void GoMainMenu()
    {
        // load main menu scene
        SceneManager.LoadScene(0);
        // unload currently active scene
        SceneManager.UnloadScene(SceneManager.GetActiveScene());
    }

    public void ResumeGame()
    {
        vars.IsPaused = !vars.IsPaused;
    }
}
