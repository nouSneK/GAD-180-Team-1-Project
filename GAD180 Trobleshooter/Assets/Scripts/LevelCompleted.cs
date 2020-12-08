using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleted : MonoBehaviour
{
    public string levelSelectScene = "MainMenu";

    private GameObject player;

    private Scene currentScene;

    private bool levelIsCompleted;

    private void Start()
    {
        player = GameObject.Find("Player");

        currentScene = SceneManager.GetActiveScene();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!levelIsCompleted && other.gameObject == player)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneManager.LoadScene(levelSelectScene);
            SceneManager.UnloadScene(currentScene);

            levelIsCompleted = true;
        }
    }
}
