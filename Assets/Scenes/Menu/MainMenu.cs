using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Level01");
    }

    public void Multiplayer()
    {
        SceneManager.LoadScene("MultiplayerMenu");
    }
    
    public void QuitGame()
    {
        if (STEAMAPIMANAGER.instance != null)
        {
            if (STEAMAPIMANAGER.instance.IsInitialized())
            {
                STEAMAPIMANAGER.instance.LeaveLobby();
            }
        }
        
        Debug.Log("QUIT");
        Application.Quit();
    }
}
