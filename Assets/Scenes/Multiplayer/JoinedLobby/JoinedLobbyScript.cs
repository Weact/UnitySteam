using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class JoinedLobbyScript : MonoBehaviour
{
    public TMPro.TMP_Text lobbyHostName;
    public TMPro.TMP_Text lobbyName;

    // Start is called before the first frame update
    void Start()
    {
        if (STEAMAPIMANAGER.instance != null)
        {
            if (STEAMAPIMANAGER.instance.IsInitialized())
            {
                lobbyHostName.SetText(STEAMAPIMANAGER.instance.GetLobbyHostUsername());
                lobbyHostName.SetText(STEAMAPIMANAGER.instance.GetLobbyName());
                STEAMAPIMANAGER.instance.SendLobbyMessage(STEAMAPIMANAGER.SteamCustomCodes.STEAM_LOBBY_PLAYER_ENTERED.ToString());
            }
        }
        else
        {
            SceneManager.LoadScene("TitleScreen");
        }
    }

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
