using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

/// <summary>
/// Author : DRUCKES Lucas
/// This script handles the start of the game when a lobby has been created
/// If the lobby has 2 members (host + someone joined), the host will be able to start the game
/// Otherwise, the host must wait for a friend to join in order to start the game
/// </summary>

public class LobbyMenu : MonoBehaviour
{
    public TMPro.TMP_Text LobbyMemberHostLabel;
    public TMPro.TMP_Text LobbyMemberSecondLabel; //NEVER UPDATED SO FAR

    //IF STEAMAPIMANAGER NOT NULL AND INITIALIZED
    //DISPLAY LOBBY INFORMATION ON SCREEN AND HOST USERNAME
    void Awake()
    {
        if (STEAMAPIMANAGER.instance != null)
        {
            if (STEAMAPIMANAGER.instance.IsInitialized())
            {
                string lobby_host = STEAMAPIMANAGER.instance.network_manager.user.username; // GET HOST USERNAME, local player
                Debug.Log($"Lobby for {lobby_host}");

                if (LobbyMemberHostLabel == null || LobbyMemberSecondLabel == null)
                {
                    return;
                }

                LobbyMemberHostLabel.SetText(lobby_host); //SET HOST USERNAME ON SCREEN
                LobbyMemberSecondLabel.SetText("");
                
            }
        }
    }

    // WHEN TRYING TO START THE GAME, WE FIRST CHECK IF THERE ARE 2 PLAYERS IN LOBBY
    // IF NOT WE DO NOTHING
    // IF THERE ARE, START THE GAME FOR BOTH PLAYERS BY SENDING MESSAGE TO NETWORK
    // WHEN THE MESSAGE HAS BEEN RECEIVED, PLAYERS WILL BE ENTERING LEVEL01 SCENE
    // FOR NOW, BOTH PLAYER WILL BE ABLE TO PLAY THEIR CHARACTER, ONE WITH PLAYER1 AND PLAYER1MATERIAL
    // THE OTHER WITH PLAYER2 AND PLAYER2MATERIAL
    // BUT THEY WONT BE ABLE TO SEE EACH OTHER
    // GAME MOVEMENTS NO SYNCED YET
    public void StartGame()
    {
        if (STEAMAPIMANAGER.instance.GetLobbyMembersCount() != 2)
        {
            STEAMAPIMANAGER.instance.SendLobbyMessage(STEAMAPIMANAGER.SteamCustomCodes.STEAM_LOBBY_PLAYERS_COUNT_INVALID_ABORT.ToString());
            return;
        }
        STEAMAPIMANAGER.instance.SendLobbyMessage(STEAMAPIMANAGER.SteamCustomCodes.STEAM_LOBBY_PLAYERS_COUNT_VALID_GAMESTART.ToString()) ;
    }

    // Just leave lobby and back to menu if pressed on button
    public void BackToMultiplayerMenu()
    {
        SceneManager.LoadScene("MultiplayerMenu");
        STEAMAPIMANAGER.instance.LeaveLobby();
    }
}
