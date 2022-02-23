using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamLobby : MonoBehaviour
{
    [SerializeField] SteamNetwork network;

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

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    #endregion

    #region logic

    void CreateLobby(Steamworks.ELobbyType type, int maxMembers)
    {
        if(type != Steamworks.ELobbyType.k_ELobbyTypePrivate ||
            type != Steamworks.ELobbyType.k_ELobbyTypePublic ||
            type != Steamworks.ELobbyType.k_ELobbyTypeFriendsOnly ||
            type != Steamworks.ELobbyType.k_ELobbyTypeInvisible)
        {
            return;
        }

        SteamMatchmaking.CreateLobby(type, maxMembers);
    }

    void LeaveLobby()
    {

    }

    void JoinLobby()
    {

    }

    void CreateCallbacks()
    {
        m_LobbyChatMsgReceived = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMsgReceived);
        m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        m_LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        m_LobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
        m_LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        m_LobbyRequestList = Callback<LobbyMatchList_t>.Create(OnLobbyRequestList);

        Debug.Log("Callbacks created successfully");
    }

    void CreateCallResults()
    {
        Debug.Log("Callresults created successfully");
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
    }

    void OnLobbyChatUpdate(LobbyChatUpdate_t pCallBack)
    {
    }

    void OnLobbyCreated(LobbyCreated_t pCallBack)
    {
    }

    void OnLobbyDataUpdate(LobbyDataUpdate_t pCallBack)
    {
    }

    void OnLobbyEntered(LobbyEnter_t pCallBack)
    {
    }

    void OnLobbyRequestList(LobbyMatchList_t pCallBack)
    {
    }

    #endregion
}
