using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STEAMAPIMANAGER : MonoBehaviour
{
    public static STEAMAPIMANAGER instance;

    public SteamNetwork network_manager;
    public SteamLobby lobby_manager;

    private void Awake()
    {
        if (instance != null || network_manager == null || lobby_manager == null)
        {
            Debug.Log("ERROR, Could not create more than one instance of STEAMAPIMANAGER / Network Manager or Lobby Manager is null");
            return;
        }

        Debug.Log("Setting instance, network manager and lobby manager from STEAMAPIMANAGER...");

        instance = this;
        network_manager.m_lobby_manager = lobby_manager;
        lobby_manager.m_network_manager = network_manager;

        InitAPIs();
    }

    private void InitAPIs()
    {
        if(network_manager != null && lobby_manager != null)
        {
            network_manager.InitAPI();
            lobby_manager.InitAPI();
        }
    }
    public void CallApi()
    {
        Debug.Log("API Successfully Called !");
    }

    public void CreateLobby(string lobbyname, Steamworks.ELobbyType type, int maxmembers)
    {
        lobby_manager.CreateLobby(lobbyname, type, maxmembers);
    }

    public void JoinLobby(Steamworks.CSteamID lobbyId)
    {
        lobby_manager.JoinLobby(lobbyId);
    }

    public void LeaveLobby()
    {
        lobby_manager.LeaveLobby();
    }

}
