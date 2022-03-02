using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class STEAMAPIMANAGER : MonoBehaviour
{
    public static STEAMAPIMANAGER instance;

    public SteamNetwork network_manager;
    public SteamLobby lobby_manager;

    private bool initialized = false;

    private void Awake()
    {
        if (instance != null || network_manager == null || lobby_manager == null)
        {
            //Debug.Log("ERROR, Could not create more than one instance of STEAMAPIMANAGER / Network Manager or Lobby Manager is null");
            return;
        }

        Debug.Log("Setting instance, network manager and lobby manager from STEAMAPIMANAGER...");

        instance = this;
        network_manager.m_lobby_manager = lobby_manager;
        lobby_manager.m_network_manager = network_manager;

        InitAPIs();
        DontDestroyOnLoad(gameObject);
    }

    public bool IsInitialized()
    {
        return initialized;
    }

    private void InitAPIs()
    {
        if(network_manager != null && lobby_manager != null)
        {
            network_manager.InitAPI();
            lobby_manager.InitAPI();
            initialized = true;
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

    public string GetLobbyHost()
    {
        if ( (uint)network_manager.user.LobbyID != 0 ) {
            return network_manager.user.username;
        }
        return "Unknown";
    }

    public int GetLobbyMembersCount()
    {
        if( (uint)network_manager.user.LobbyID != 0)
        {
            return SteamMatchmaking.GetNumLobbyMembers(network_manager.user.LobbyID);
        }
        return 0;
    }

}
