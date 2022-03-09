using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;
using System.Text;

/// <summary>
/// AUTHOR: DRUCKES Lucas
/// This class has been made to handle "global" network purposes
/// It has network and lobby managers references so it can call methods from there directly
/// This class is mainly used to be called from outside, since it is a DontDestroy Game object and a Singleton.
/// </summary>
/// 
public class STEAMAPIMANAGER : MonoBehaviour
{
    //THIS CLASS WILL HANDLE BASIC NETWORK BEHAVIOR

    //CUSTOM CODES TO IDENTIFY WHAT HAS BEEN DONE
    public enum SteamCustomCodes : int
    {
        STEAM_LOBBY_PLAYERS_COUNT_VALID_GAMESTART = 10001,
        STEAM_LOBBY_PLAYERS_COUNT_INVALID_ABORT = 10002,
        STEAM_LOBBY_PLAYER_ENTERED = 10003,
        STEAM_LOBBY_PLAYER_LEFT = 10004
    }

    //INSTANCE OF THE OBJECT SO THAT IT CAN BE ACCESSED FROM OUTSIDE WITH STEAMAPIMANAGER.instance
    public static STEAMAPIMANAGER instance;


    //NETWORK MANAGER HAS USER INFORMATION (user steamid, username, lobbyid, haslobby, ...)
    public SteamNetwork network_manager;

    //LOBBY MANAGER HAS LOBBY METHODS
    public SteamLobby lobby_manager;

    //DEFINE IF THE STEAMAPIMANAGER HAS BEEN INITIALIZED LIKE STEAMWORKS.STEAMAPI
    private bool initialized = false;

    private void Awake()
    {
        //IF THE STEAMWORKS.STEAMAPI FAILED TO INITIALIZE, WE RETURN AND DO NOTHING
        if (!SteamAPI.Init())
        {
            return;
        }

        //IF THE INSTANCE OF STEAMAPIMANAGER OR NETWORK|LOBBY MANAGER INSTANCE HAS NOT BEEN SET IN EDITOR, RETURN AND DO NOTHING
        if (instance != null || network_manager == null || lobby_manager == null)
        {
            //Debug.Log("ERROR, Could not create more than one instance of STEAMAPIMANAGER / Network Manager or Lobby Manager is null");
            return;
        }
        
        Debug.Log("Setting instance, network manager and lobby manager from STEAMAPIMANAGER...");


        //SET THE INSTANCE OF STEAMAPIMANAGER | NETWORK MANAGER | LOBBY MANAGER
        instance = this;
        network_manager.m_lobby_manager = lobby_manager;
        lobby_manager.m_network_manager = network_manager;


        //INIT NETWORK AND LOBBY APIS
        InitAPIs();

        //MAKE THE OBJECT ALWAYS IN SCENE
        DontDestroyOnLoad(gameObject);
    }

    public bool IsInitialized()
    {
        return initialized;
    }

    //INITIALIZE NETWORK MANAGER AND LOBBY MANAGER
    //IF IT FAILS BECAUSE STEAMWORKS.STEAMAPI FAILED TO INITIALIZE, IT WILL RETURN AND WE DO NOTHING
    private void InitAPIs()
    {
        if ( !SteamAPI.Init())
        {
            Debug.Log("could not initialize steam, maybe start steam ?");
            initialized = false;
            return;
        }
        if(network_manager != null && lobby_manager != null)
        {
            network_manager.InitAPI();
            lobby_manager.InitAPI();
            initialized = true;

            //EXECUTE LEAVE LOBBY METHOD AT THE START OF THE GAME
            //SO THAT WE ARE SURE USER STARTS WITH CLEAR DATAS
            LeaveLobby();
        }
    }

    //DATA SENDING PURPOSES
    public void SendLobbyMessage(string data)
    {
        // CONVERT STRING TO BYTES ARRAY
        byte[] bytes = new byte[data.Length * sizeof(char)];
        bytes = Encoding.Default.GetBytes(data);
        SteamMatchmaking.SendLobbyChatMsg(network_manager.user.LobbyID, bytes, bytes.Length);
    }

    //DEBUG METHOD TO DISPLAY THAT THE API HAS BEEN CALLED
    public void CallApi()
    {
        Debug.Log("API Successfully Called !");
    }

    //CALL THE CREATELOBBY METHOD FROM LOBBY MANAGER
    //LOBBYNAME WILL BE SET IF THE LOBBY HAS BEEN SUCCESSFULLY CREATED IN OnLobbyCreated CALLBACK WITH SteamMatchmaking.SetLobbyData(lobbyId, pchKey, pchValue)
    //TYPE DEFINE IF THE LOBBY WILL BE ACCESSIBLE FROM ANYONE, FRIENDS, INVISIBLE OR PRIVATE (invite only)
    public void CreateLobby(string lobbyname, ELobbyType type, int maxmembers)
    {
        lobby_manager.CreateLobby(lobbyname, type, maxmembers);
    }

    //CALL THE JOIN LOBBY METHOD FROM LOBBY MANAGER
    public void JoinLobby(CSteamID lobbyId)
    {
        lobby_manager.JoinLobby(lobbyId);
    }

    //CALL THE LEAVE LOBBY METHOD FROM LOBBY MANAGER
    public void LeaveLobby()
    {
        lobby_manager.LeaveLobby();
    }

    //CALL SEARCH LOBBY METHOD FROM LOBBY MANAGER
    public void SearchForLobbies()
    {
        lobby_manager.SearchLobby();
    }

    //RETURN THE NAME OF THE LOBBY OWNER BY GETTING ITS STEAMID
    public string GetLobbyHostUsername()
    {
        return SteamFriends.GetFriendPersonaName(GetLobbyHostSteamID());
    }

    //RETURN THE ID OF LOBBY OWNER
    public CSteamID GetLobbyHostSteamID()
    {
        return SteamMatchmaking.GetLobbyOwner(network_manager.user.LobbyID);
    }

    //RETURN NUMBERS OF PLAYER IN LOBBY IF VALID LOBBY ELSE RETURN 0
    public int GetLobbyMembersCount()
    {
        if( (uint)network_manager.user.LobbyID != 0)
        {
            return SteamMatchmaking.GetNumLobbyMembers(network_manager.user.LobbyID);
        }
        return 0;
    }

    //RETURN THE NAME OF THE LOBBY IF THE LOBBY METADATA NAME HAS BEEN SET AND IS VALID ELSE RETURN ""
    public string GetLobbyName()
    {
        if (!IsInitialized())
        {
            return "";
        }

        return SteamMatchmaking.GetLobbyData(network_manager.user.LobbyID, "name");
    }

}
