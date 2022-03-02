using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                    Debug.Log("bullshit software");
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
            return;
        }
        SceneManager.LoadScene("Level01");
    }

    public void BackToMultiplayerMenu()
    {
        SceneManager.LoadScene("MultiplayerMenu");
        STEAMAPIMANAGER.instance.LeaveLobby();
    }
}
