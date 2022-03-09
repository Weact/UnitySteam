using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles everything related to when someone join the lobby
/// It will directly redirect the player to a Waiting screen, until the host of the lobby start the game
/// In this script, we set the lobby name and host's name to some UI text elements. From start method.
/// </summary>

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
