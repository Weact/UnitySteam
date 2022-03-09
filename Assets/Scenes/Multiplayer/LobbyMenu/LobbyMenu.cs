using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

public class LobbyMenu : MonoBehaviour
{
    public TMPro.TMP_Text LobbyMemberHostLabel;
    public TMPro.TMP_Text LobbyMemberSecondLabel;

    void Awake()
    {
        if (STEAMAPIMANAGER.instance != null)
        {
            if (STEAMAPIMANAGER.instance.IsInitialized())
            {
                string lobby_host = STEAMAPIMANAGER.instance.network_manager.user.username;
                Debug.Log($"Lobby for {lobby_host}");

                if (LobbyMemberHostLabel == null || LobbyMemberSecondLabel == null)
                {
                    return;
                }

                LobbyMemberHostLabel.SetText(lobby_host);
                LobbyMemberSecondLabel.SetText("");
                
            }
        }
    }

    public void StartGame()
    {
        if (STEAMAPIMANAGER.instance.GetLobbyMembersCount() != 2)
        {
            STEAMAPIMANAGER.instance.SendLobbyMessage(STEAMAPIMANAGER.SteamCustomCodes.STEAM_LOBBY_PLAYERS_COUNT_INVALID_ABORT.ToString());
            return;
        }
        STEAMAPIMANAGER.instance.SendLobbyMessage(STEAMAPIMANAGER.SteamCustomCodes.STEAM_LOBBY_PLAYERS_COUNT_VALID_GAMESTART.ToString()) ;
    }

    public void BackToMultiplayerMenu()
    {
        SceneManager.LoadScene("MultiplayerMenu");
        STEAMAPIMANAGER.instance.LeaveLobby();
    }
}
