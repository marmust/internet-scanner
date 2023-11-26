using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuSystem : MonoBehaviour
{
    public VarHolder vars;
    public GameObject PauseMenuObject;

    private void Update()
    {
        PauseMenuObject.active = vars.IsPuased;
    }

    public void GoMainMenu()
    {
        // load main menu scene
        SceneManager.LoadScene(1);
        // unload graph scene
        SceneManager.UnloadScene(SceneManager.GetActiveScene());
    }

    public void ResumeGame()
    {
        vars.IsPuased = !vars.IsPuased;
    }
}
