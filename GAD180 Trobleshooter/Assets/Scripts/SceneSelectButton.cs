using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelectButton : MonoBehaviour
{
    public string scene;

    public bool lockCursor = true;

    public bool restartButton = true;

    private Scene currentScene;

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();
    }

    private void Update()
    {
        if ((restartButton && Input.GetKeyDown(KeyCode.R)) || (!restartButton && Input.GetKeyDown(KeyCode.M)))
        {
            ButtonClick();
        }
    }

    public void ButtonClick()
    {
        Debug.Log("Change scene to " + scene);

        SceneManager.LoadScene(scene);
        SceneManager.UnloadScene(currentScene);

        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
