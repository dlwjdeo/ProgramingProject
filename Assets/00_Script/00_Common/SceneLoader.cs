using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            return;
        }

        SceneManager.LoadScene(sceneName);
        
        SoundManager.Instance?.PlayGameStartSfx();
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
