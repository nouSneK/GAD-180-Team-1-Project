using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneSwitcher : MonoBehaviour
{
    public string myScene;

    public void OnClick()
    {
        SceneManager.LoadScene(myScene);
    }
}