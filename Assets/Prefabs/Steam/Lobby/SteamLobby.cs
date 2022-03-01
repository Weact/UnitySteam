using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamLobby : MonoBehaviour
{
    public SteamNetwork m_network_manager = null;

    #region callbacks

    protected Callback<LobbyChatMsg_t> m_LobbyChatMsgReceived;
    protected Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;
    protected Callback<LobbyCreated_t> m_LobbyCreated;
    protected Callback<LobbyDataUpdate_t> m_LobbyDataUpdate;
    protected Callback<LobbyEnter_t> m_LobbyEntered;
    protected Callback<LobbyMatchList_t> m_LobbyRequestList;

    #endregion

    #region callresults
    #endregion

    #region builtin-methods

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitAPI()
    {
        if (SteamManager.Initialized)
        {
            CreateCallbacks();
            CreateCallResults();
        }
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
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
    }

    void CreateCallResults()
    {
    }

    public void CreateLobby(string lobbyName, Steamworks.ELobbyType type, int maxMembers)
    {
        // ! network.user.LobbyID is already done in EnterLobby_t and LobbyCreated_t Callbacks !

        if(lobbyName == "")
        {
            Debug.Log("Lobby name must have a valid value");
            return;
        }

        if(type != Steamworks.ELobbyType.k_ELobbyTypePrivate &&
            type != Steamworks.ELobbyType.k_ELobbyTypePublic &&
            type != Steamworks.ELobbyType.k_ELobbyTypeFriendsOnly &&
            type != Steamworks.ELobbyType.k_ELobbyTypeInvisible)
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
    }
    
    public void SearchLobby()
    {
        if (SteamManager.Initialized)
        {
            SteamMatchmaking.AddRequestLobbyListDistanceFilter(Steamworks.ELobbyDistanceFilter.k_ELobbyDistanceFilterClose);
            SteamMatchmaking.RequestLobbyList();
        }
    }

    public void LeaveLobby()
    {
        if (SteamManager.Initialized)
        {
            if (m_network_manager.user.hasLobby )
            {
                Debug.Log($"Lobby ID {m_network_manager.user.LobbyID} has been left");
                SteamMatchmaking.LeaveLobby(m_network_manager.user.LobbyID);
                m_network_manager.user.LobbyID = (Steamworks.CSteamID) 0;
            }
        }
    }

    public void JoinLobby(Steamworks.CSteamID lobbyId)
    {
        // ! m_network_manager.user.LobbyID is already done in EnterLobby_t and LobbyCreated_t Callbacks !

        if (SteamManager.Initialized)
        {
            if (m_network_manager.user.hasLobby )
            {
                LeaveLobby();
            }
            if ( (uint) lobbyId != 0)
            {
                SteamMatchmaking.JoinLobby(lobbyId);
            }
        }
    }


    #endregion

    #region callback-results

    void OnLobbyChatMsgReceived(LobbyChatMsg_t pCallBack)
    {
        /// <summary>
        ///m_ulSteamIDLobby  The Steam ID of the lobby this message was sent in.
        ///m_ulSteamIDUser uint64  Steam ID of the user who sent this message.Note that it could have been the local user.
        ///m_eChatEntryType uint8   Type of message received. This is actually a EChatEntryType.
        ///m_iChatID uint32  The index of the chat entry to use with GetLobbyChatEntry, this is not valid outside of the scope of this callback and should never be stored.
        /// </summary>

        uint lobby_id = (uint)pCallBack.m_ulSteamIDLobby;
        uint steamuser_id = (uint)pCallBack.m_ulSteamIDUser;
        uint entry_type = pCallBack.m_eChatEntryType;

        Debug.Log($"ChatMsgReceived | Lobby ID : {lobby_id} | SteamUser_ID : {steamuser_id} | chatEntry : {entry_type}");
    }

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
        
        Steamworks.EResult result = pCallBack.m_eResult;
        Steamworks.CSteamID createdLobbyId = (Steamworks.CSteamID) pCallBack.m_ulSteamIDLobby;

        Debug.Log($"OnLobbyCreated | Result : {result} | Lobby ID : {createdLobbyId}");
    }

    void OnLobbyDataUpdate(LobbyDataUpdate_t pCallBack)
    {
        ///<summary>
        ///m_ulSteamIDLobby uint64  The Steam ID of the Lobby.
        ///m_ulSteamIDMember uint64  Steam ID of either the member whose data changed, or the room itself.
        ///m_bSuccess uint8   true if the lobby data was successfully changed, otherwise false.
        ///</summary>
        
        uint lobby_id = (uint) pCallBack.m_ulSteamIDLobby;
        uint member_steam_id = (uint) pCallBack.m_ulSteamIDMember;
        byte success = pCallBack.m_bSuccess;

        Debug.Log($"OnLobbyDataUpdate | Lobby ID : {lobby_id} | Member ID : {member_steam_id} | Success : {success}");
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

        Steamworks.CSteamID enteredLobbyId = (Steamworks.CSteamID) pCallBack.m_ulSteamIDLobby;
        bool locked = pCallBack.m_bLocked;
        uint response = pCallBack.m_EChatRoomEnterResponse;

        m_network_manager.user.LobbyID = enteredLobbyId;

        Debug.Log($"OnLobbyEntered | Lobby ID : {enteredLobbyId} | Locked : {locked} | Response : {response} | User Lobby : {m_network_manager.user.LobbyID}");
        Debug.Log(m_network_manager.user.ToString() );
    }

    void OnLobbyRequestList(LobbyMatchList_t pCallBack)
    {
        ///<summary>
        ///m_nLobbiesMatching	uint32	Number of lobbies that matched search criteria and we have Steam IDs for.
        ///</summary>
        
        uint matched_lobbies = pCallBack.m_nLobbiesMatching;

        Debug.Log($"OnLobbyRequestList | Number of Matching Lobbies : {matched_lobbies}");
    }

    #endregion
}
