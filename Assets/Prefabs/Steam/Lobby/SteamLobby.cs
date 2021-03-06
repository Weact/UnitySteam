using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Text;
using UnityEngine.SceneManagement;

/// <summary>
/// Author : DRUCKES Lucas
/// This class handles everything lobby related.
/// It has all lobby callbacks, lobby methods as CreateLobby, LeaveLobby and such methods.
/// All the callbacks are in #callback-results region near line 150
/// </summary>

public class SteamLobby : MonoBehaviour
{
    //TAKE THE REFERENCE OF NETWORK MANAGER IN EDITOR
    public SteamNetwork m_network_manager = null;

    //USED TO STORE THE LOBBY NAME
    private string tmp_lobbyname = "";

    #region callbacks

    protected Callback<LobbyChatMsg_t> m_LobbyChatMsgReceived;
    protected Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;
    protected Callback<LobbyCreated_t> m_LobbyCreated;
    protected Callback<LobbyDataUpdate_t> m_LobbyDataUpdate;
    protected Callback<LobbyEnter_t> m_LobbyEntered;
    protected Callback<LobbyMatchList_t> m_LobbyRequestList;
    protected Callback<GameLobbyJoinRequested_t> m_LobbyJoinRequestFriendList;
    protected Callback<GameRichPresenceJoinRequested_t> m_GameRichPresenceJoinRequested;

    #endregion

    #region callresults
    #endregion

    #region builtin-methods

    //INITIALIZE LOBBY MANAGER API
    //CREATE CALLBACKS AND CALLRESULTS IF STEAMMANAGER IS INITIALIZED
    public void InitAPI()
    {
        if (SteamManager.Initialized)
        {
            CreateCallbacks();
            CreateCallResults();
        }
    }

    #endregion

    #region logic

    void CreateCallbacks()
    {
        m_LobbyChatMsgReceived = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMsgReceived);
        m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        m_LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        m_LobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
        m_LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        m_LobbyRequestList = Callback<LobbyMatchList_t>.Create(OnLobbyRequestList);
        m_LobbyJoinRequestFriendList = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequestedFriendList);
        m_GameRichPresenceJoinRequested = Callback<GameRichPresenceJoinRequested_t>.Create(OnGameRichPresenceJoinRequested);

    }

    void CreateCallResults()
    {
    }

    //CALL STEAMWORKS CREATE LOBBY METHOD
    //lobbyName<string> IS USED TO SET LOBBY'S METADATA
    public void CreateLobby(string lobbyName, ELobbyType type, int maxMembers)
    {
        // ! network.user.LobbyID is already done in EnterLobby_t and LobbyCreated_t Callbacks !

        if(lobbyName == "")
        {
            Debug.Log("Lobby name must have a valid value");
            return;
        }

        if(type != ELobbyType.k_ELobbyTypePrivate && //0
            type != ELobbyType.k_ELobbyTypePublic && //1
            type != ELobbyType.k_ELobbyTypeFriendsOnly && //2
            type != ELobbyType.k_ELobbyTypeInvisible) //3
        {
            Debug.Log("Invalid type, returning");
            return;
        }

        if( m_network_manager.user.hasLobby )
        {
            Debug.Log("User already has a lobby, leaving..");
            LeaveLobby();
        }

        SteamMatchmaking.CreateLobby(type, maxMembers);
        tmp_lobbyname = lobbyName;
    }
    
    //SEARCH FOR LOBBIES WITH A DISTANCE FILDER SET BY DEFAULT
    public void SearchLobby()
    {
        if (SteamManager.Initialized)
        {
            SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterDefault);
            SteamMatchmaking.RequestLobbyList();
        }
    }

    //LEAVE THE CURRENT LOBBY IF THE USER IS IN A VALID LOBBY
    public void LeaveLobby()
    {
        if (SteamManager.Initialized)
        {
            if (m_network_manager.user.hasLobby )
            {
                Debug.Log($"Lobby ID {m_network_manager.user.LobbyID} has been left");
                STEAMAPIMANAGER.instance.SendLobbyMessage(STEAMAPIMANAGER.SteamCustomCodes.STEAM_LOBBY_PLAYER_LEFT.ToString()); //NOTIFY ALL PLAYERS IN LOBBY
                SteamMatchmaking.LeaveLobby(m_network_manager.user.LobbyID); //LEAVE THE LOBBY
                m_network_manager.user.LobbyID = (CSteamID) 0; //SET THE USER'S LOBBY ID TO 0 SINCE WE LEFT
            }
        }
    }

    //JOIN A LOBBY IF THE ID IS VALID AND STEAM MANAGER IS INITIALIZED
    public void JoinLobby(CSteamID lobbyId)
    {
        // ! m_network_manager.user.LobbyID is already done in EnterLobby_t and LobbyCreated_t Callbacks !

        if (SteamManager.Initialized)
        {
            if (m_network_manager.user.hasLobby )
            {
                LeaveLobby(); //LEAVE LOBBY FIRST IF THE USER HAS A LOBBY ALREADY
            }
            if ( (uint) lobbyId != 0)
            {
                SteamMatchmaking.JoinLobby(lobbyId); // JOIN LOBBY IS EVERYTHING WAS SUCCESSFUL
            }
        }
    }


    #endregion

    #region callback-results

    //ON RECEIVED A MESSAGE IN LOBBY FROM A USER
    void OnLobbyChatMsgReceived(LobbyChatMsg_t pCallBack)
    {
        /// <summary>
        ///m_ulSteamIDLobby  The Steam ID of the lobby this message was sent in.
        ///m_ulSteamIDUser uint64  Steam ID of the user who sent this message.Note that it could have been the local user.
        ///m_eChatEntryType uint8   Type of message received. This is actually a EChatEntryType.
        ///m_iChatID uint32  The index of the chat entry to use with GetLobbyChatEntry, this is not valid outside of the scope of this callback and should never be stored.
        /// </summary>

        CSteamID lobby_id = (CSteamID)pCallBack.m_ulSteamIDLobby;
        CSteamID steamuser_id = (CSteamID)pCallBack.m_ulSteamIDUser;
        EChatEntryType entry_type = (EChatEntryType)pCallBack.m_eChatEntryType;
        uint message_id = pCallBack.m_iChatID;

        string sMessage; //STRING VARIABLE TO STORE THE MESSAGE IN
        byte[] bytes = new byte[4096];
        SteamMatchmaking.GetLobbyChatEntry(lobby_id, (int)message_id, out steamuser_id, bytes, 4096, out entry_type); //GET MESSAGE CONTENT AS BINARY
        sMessage = Encoding.Default.GetString(bytes); //CONVERT TO STRING

        Debug.Log($"ChatMsgReceived | Lobby ID : {(uint)lobby_id} | SteamUser_ID : {(uint)steamuser_id} | chatEntry : {(uint)entry_type}"); //DEBUG RESULT

        //string.Compare(str1, str2); //THIS METHOD COMPARES 2 STRING OBJECT THE RIGHT WAY, WE MUST USE COMPARE


        //BIG IF THING, USED TO COMPARE THE MESSAGE TO EACH CODES AND DO SOMETHING
        if(string.Compare(sMessage, STEAMAPIMANAGER.SteamCustomCodes.STEAM_LOBBY_PLAYERS_COUNT_VALID_GAMESTART.ToString() ) == 0)
        {
            SceneManager.LoadScene("Level01");
            return;
        }
        else if(string.Compare(sMessage, STEAMAPIMANAGER.SteamCustomCodes.STEAM_LOBBY_PLAYERS_COUNT_INVALID_ABORT.ToString()) == 0)
        {
            // [...] DO SOMETHING IF THERE ARE TOO FEW PLAYER IN LOBBY
            // MAYBE DISPLAY IT IN THE LOBBY SCREEN ?
            Debug.Log("TOO FEW PLAYERS IN LOBBY"); //WE'LL JUST DEBUG MESSAGE FOR NOW
            return;
        }
        else if(string.Compare(sMessage, STEAMAPIMANAGER.SteamCustomCodes.STEAM_LOBBY_PLAYER_ENTERED.ToString()) == 0)
        {
            // [...] DO SOMETHING IF A PLAYER ENTERED THE LOBBY
            // MAYBE DISPLAY IT IN THE LOBBY SCREEN ?
            Debug.Log("A PLAYER ENTERED THE LOBBY"); //WE'LL JUST DEBUG MESSAGE FOR NOW
            return;
        }
        else if(string.Compare(sMessage, STEAMAPIMANAGER.SteamCustomCodes.STEAM_LOBBY_PLAYER_LEFT.ToString()) == 0)
        {
            // [...] DO SOMETHING IF A PLAYER LEFT THE LOBBY
            // MAYBE DISPLAY IT IN THE LOBBY SCREEN ?
            Debug.Log("A PLAYER LEFT THE LOBBY"); //WE'LL JUST DEBUG MESSAGE FOR NOW
            return;
        }
        else
        {
            // INVALID MESSAGE, JUST PRINTING IT FOR DEBUG PURPOSES
            Debug.Log($"Message : {sMessage}"); 
            return;
        }

    }

    // WHEN LOBBY GOT CHANGEMENTS :
    // check method summary below
    void OnLobbyChatUpdate(LobbyChatUpdate_t pCallBack)
    {
        ///<summary>
        ///m_ulSteamIDLobby	uint64	The Steam ID of the lobby.
        ///m_ulSteamIDUserChanged uint64  The user who's status in the lobby just changed - can be recipient.
        ///m_ulSteamIDMakingChange uint64  Chat member who made the change. This can be different from m_ulSteamIDUserChanged if kicking, muting, etc.For example, if one user kicks another from the lobby, this will be set to the id of the user who initiated the kick.
        ///m_rgfChatMemberStateChange uint32  Bitfield of EChatMemberStateChange values.
        ///</summary>
        
        uint updatedLobbyId = (uint)pCallBack.m_ulSteamIDLobby;
        uint changedUserId = (uint)pCallBack.m_ulSteamIDUserChanged;
        uint changerUserId = (uint)pCallBack.m_ulSteamIDMakingChange;
        uint state_change = (uint)pCallBack.m_rgfChatMemberStateChange;

        Debug.Log($"LobbyChatUpdate | Lobby ID : {updatedLobbyId} | User Changed ID : {changedUserId} | User Changer ID : {changerUserId} | StateChange : {state_change}");
    }

    void OnLobbyCreated(LobbyCreated_t pCallBack)
    {
        ///<summary>
        ///m_eResult	EResult	The result of the operation.
        ///
        /// Possible values:
        ///k_EResultOK - The lobby was successfully created.
        ///k_EResultFail - The server responded, but with an unknown internal error.
        ///k_EResultTimeout - The message was sent to the Steam servers, but it didn't respond.
        ///k_EResultLimitExceeded - Your game client has created too many lobbies and is being rate limited.
        ///k_EResultAccessDenied - Your game isn't set to allow lobbies, or your client does haven't rights to play the game
        ///k_EResultNoConnection - Your Steam client doesn't have a connection to the back-end.
        ///
        ///m_ulSteamIDLobby uint64  The Steam ID of the lobby that was created, 0 if failed.
        ///</summary>
        
        EResult result = pCallBack.m_eResult;
        CSteamID createdLobbyId = (CSteamID) pCallBack.m_ulSteamIDLobby;

        bool successJoinable = SteamMatchmaking.SetLobbyJoinable(createdLobbyId, true);
        SteamMatchmaking.SetLobbyData(createdLobbyId, "name", tmp_lobbyname);

        Debug.Log($"OnLobbyCreated | Result : {result} | Lobby ID : {createdLobbyId} | Joinable Succes : {successJoinable}");
    }

    void OnLobbyDataUpdate(LobbyDataUpdate_t pCallBack)
    {
        ///<summary>
        ///m_ulSteamIDLobby uint64  The Steam ID of the Lobby.
        ///m_ulSteamIDMember uint64  Steam ID of either the member whose data changed, or the room itself.
        ///m_bSuccess uint8   true if the lobby data was successfully changed, otherwise false.
        ///</summary>
        
        CSteamID lobby_id = (CSteamID)pCallBack.m_ulSteamIDLobby;
        CSteamID member_steam_id =  (CSteamID)pCallBack.m_ulSteamIDMember;
        byte success = pCallBack.m_bSuccess;

        Debug.Log($"OnLobbyDataUpdate | Lobby ID : {(uint)lobby_id} | Member ID : {(uint)member_steam_id} | Success : {success} | With {SteamMatchmaking.GetNumLobbyMembers(lobby_id)} members in lobby");
    }

    void OnLobbyEntered(LobbyEnter_t pCallBack)
    {
        ///<summary>
        /// m_ulSteamIDLobby	uint64	The steam ID of the Lobby you have entered.
        /// m_rgfChatPermissions uint32  Unused - Always 0.
        /// m_bLocked   bool If true, then only invited users may join.
        ///
        /// m_EChatRoomEnterResponse uint32  This is actually a EChatRoomEnterResponse value.
        /// This will be set to k_EChatRoomEnterResponseSuccess if the lobby was successfully joined
        /// otherwise it will be k_EChatRoomEnterResponseError.
        /// 
        ///</summary>

        CSteamID enteredLobbyId = (CSteamID) pCallBack.m_ulSteamIDLobby;
        bool locked = pCallBack.m_bLocked;
        uint response = pCallBack.m_EChatRoomEnterResponse;

        m_network_manager.user.LobbyID = enteredLobbyId;

        Debug.Log($"OnLobbyEntered | Lobby ID : {enteredLobbyId} | Locked : {locked} | Response : {response} | User Lobby : {m_network_manager.user.LobbyID}");

        if (m_network_manager.user.steamid != STEAMAPIMANAGER.instance.GetLobbyHostSteamID())
        {
            STEAMAPIMANAGER.instance.SendLobbyMessage("entered");
            SceneManager.LoadScene("JoinedLobbyMenu");
        }
    }

    //REMINDER :
    //TO GET ALL THE MATCHING LOBBIES :
    //FOR LOOP AND USE GETLOBBYBYINDEX(i); i BEING THE INDEX OF THE CURRENT FOR LOOP, ITERATING THROUGH LOBBIES
    //for(int i = 0; i < nMatchingLobbies; i++){ lobby = SteamMatchmaking.GetLobbyByIndex(i); }
    // To do later? ...
    // No clue on how to update lobby list in UI after we got all the lobby honestly..
    // Started something to update in MultiplayerMenu.cs:UpdateLobbyList:72
    void OnLobbyRequestList(LobbyMatchList_t pCallBack)
    {
        ///<summary>
        ///m_nLobbiesMatching	uint32	Number of lobbies that matched search criteria and we have Steam IDs for.
        ///</summary>
        
        uint matched_lobbies = pCallBack.m_nLobbiesMatching;

        Debug.Log($"OnLobbyRequestList | Number of Matching Lobbies : {matched_lobbies}");
    }

    //WHEN WE INVITE A FRIEND TO JOIN, OR WHEN THEY JOIN BY THEMSELVES FROM FRIEND LIST
    //WE MAKE THEM JOIN THE LOBBY IF THEY'RE VALID + LOBBY VALID
    void OnLobbyJoinRequestedFriendList(GameLobbyJoinRequested_t pCallBack)
    {
        CSteamID requestedLobbyID = pCallBack.m_steamIDLobby;
        CSteamID throughFriendID = pCallBack.m_steamIDFriend;

        Debug.Log($"User {(uint)throughFriendID} tried to join the lobby : {(uint)requestedLobbyID}");
        STEAMAPIMANAGER.instance.JoinLobby(requestedLobbyID);
    }

    // not clear on what it is, but game rich presence must be by joining with commands like +connect <ip> or by discord etc?
    void OnGameRichPresenceJoinRequested(GameRichPresenceJoinRequested_t pCallback)
    {
        CSteamID throughFriendID = pCallback.m_steamIDFriend;
        string gamePresenceStatus = pCallback.m_rgchConnect;

        Debug.Log($"User {(uint)throughFriendID} requested join with game rich presence. Status : {gamePresenceStatus}");
    }

    #endregion
}
