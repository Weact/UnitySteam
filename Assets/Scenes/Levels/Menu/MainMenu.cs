using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public TMPro.TMP_Text LobbyErrorMultiplayer;
    public GameObject multiplayerButton;

    //DISABLE MULTIPLAYER BUTTON IF STEAMAPI FAILED TO INITIALIZE
    private void Start()
    {
        if (!Steamworks.SteamAPI.Init())
        {
            LobbyErrorMultiplayer.gameObject.SetActive(true);
            multiplayerButton.SetActive(false);
        }
        else
        {
            LobbyErrorMultiplayer.gameObject.SetActive(false);
            multiplayerButton.SetActive(true);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level01");
    }

    public void Multiplayer()
    {
        SceneManager.LoadScene("MultiplayerMenu");
    }
    
    //QUIT GAME AND MAKE SURE TO LEAVE LOBBY
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
