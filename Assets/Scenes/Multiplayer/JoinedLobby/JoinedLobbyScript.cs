using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class JoinedLobbyScript : MonoBehaviour
{
    public TMPro.TMP_Text lobbyHostName;
    public TMPro.TMP_Text lobbyName;

    // Start is called before the first frame update
    //IF STEAMAPIMANAGER INITIALIZED SUCCESSFULLY AND NOT NULL
    //DISPLAY LOBBY INFORMATION ON THE WAITING SCREEN
    void Start()
    {
        if (STEAMAPIMANAGER.instance != null)
        {
            if (STEAMAPIMANAGER.instance.IsInitialized())
            {
                Debug.Log($"Host Name : {STEAMAPIMANAGER.instance.GetLobbyHostUsername()} | Lobby Name : {STEAMAPIMANAGER.instance.GetLobbyName()}");
                lobbyHostName.SetText(STEAMAPIMANAGER.instance.GetLobbyHostUsername());
                lobbyName.SetText(STEAMAPIMANAGER.instance.GetLobbyName());
                STEAMAPIMANAGER.instance.SendLobbyMessage(STEAMAPIMANAGER.SteamCustomCodes.STEAM_LOBBY_PLAYER_ENTERED.ToString());
            }
        }
        else //Just go back to titlescreen
        {
            SceneManager.LoadScene("TitleScreen");
        }
    }

    //IF STEAMAPIMANAGER NOT NULL AND INITIALIZED, LEAVE LOBBY IF MULTIPLAYER GAME AND BACK TO TITLESCREEN
    public void JoinedLobbyLeave()
    {
        if(STEAMAPIMANAGER.instance != null)
        {
            if (STEAMAPIMANAGER.instance.IsInitialized())
            {
                STEAMAPIMANAGER.instance.LeaveLobby();
                SceneManager.LoadScene("TitleScreen");
            }
        }
        else
        {
            SceneManager.LoadScene("TitleScreen");
        }
    }
}
