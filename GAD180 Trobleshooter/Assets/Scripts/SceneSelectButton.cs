using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelectButton : MonoBehaviour
{
    public string scene;

    public bool lockCursor = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ButtonClick();
        }
    }

    public void ButtonClick()
    {
        Debug.Log("Change scene to " + scene);

        SceneManager.LoadScene(scene);

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
