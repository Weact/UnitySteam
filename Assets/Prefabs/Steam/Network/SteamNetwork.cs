using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamNetwork : MonoBehaviour
{
    public struct s_SteamUser
    {
        public Steamworks.CSteamID steamid { get; private set; }
        public string username { get; private set; }

        public bool hasLobby { get; private set; }
        private Steamworks.CSteamID lobbyId;
        
        public static s_SteamUser Init(Steamworks.CSteamID id, string name)
        {
            if (SteamManager.Initialized)
            {
                s_SteamUser user = new s_SteamUser();
                user.steamid = id;
                user.username = name;

                return user;
            }
        
            return new s_SteamUser();
        }


        public Steamworks.CSteamID LobbyID
        {
            get => lobbyId;
            set
            {
                if((uint)value == 0)
                {
                    hasLobby = false;
                    lobbyId = (Steamworks.CSteamID)0;
                }
                else
                {
                    hasLobby = true;
                    lobbyId = value;
                }
            }
        }
        public override string ToString() => $"(USER ID : {steamid} | USER NAME : {username} | IS IN LOBBY : {hasLobby} | LOBBY ID : {LobbyID} )";
    }

    s_SteamUser user;

    #region callbacks

    protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;
    protected Callback<GameConnectedFriendChatMsg_t> m_GameConnectedFriendChatMsg;

    #endregion

    #region callresults

    private CallResult<NumberOfCurrentPlayers_t> m_CurrentPlayers;

    #endregion

    #region builtin-methods

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SteamAPICall_t handle = SteamUserStats.GetNumberOfCurrentPlayers();
            m_CurrentPlayers.Set(handle);
        }
    }

    void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            SteamFriends.SetListenForFriendsMessages(true);

            //Create all necessary steam callbacks
            //GameOverlayActivated_t, GameConnectedFriendChatMsg_t
            CreateCallbacks();

            //Create all necessary callresults
            //NumberOfCurrentPlayers_t
            CreateCallResults();

            //Assign and Print User's SteamID and Username
            //SteamID which can be converted to CSteamID later
            user = s_SteamUser.Init(SteamUser.GetSteamID(), SteamFriends.GetPersonaName());
            Debug.Log(user.ToString());
        }
    }

    void OnDisable()
    {

    }
    #endregion

    #region logic

    void CreateCallbacks()
    {
        m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
        m_GameConnectedFriendChatMsg = Callback<GameConnectedFriendChatMsg_t>.Create(OnGameConnectedFriendChatMsg);
        Debug.Log("Callbacks created successfully");
    }

    void CreateCallResults()
    {
        m_CurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(OnPlayersRequested);
        Debug.Log("Callresults created successfully");
    }

    void AutoReply(CSteamID senderID, string message)
    {
        SteamFriends.ReplyToFriendMessage(senderID, message);
        Debug.Log("Replied to " + SteamFriends.GetFriendPersonaName(senderID) + " " + senderID + " with " + message );
    }

    #endregion

    #region callbacks-results

    void OnGameOverlayActivated(GameOverlayActivated_t pCallBack)
    {
        if(pCallBack.m_bActive != 0)
        {
            Debug.Log("Overlay is now activated");
        }
        else
        {
            Debug.Log("Overlay is now deactivated");
        }
    }

    void OnGameConnectedFriendChatMsg(GameConnectedFriendChatMsg_t pCallBack)
    {
        CSteamID senderID = pCallBack.m_steamIDUser;
        int messageID = pCallBack.m_iMessageID;

        Debug.Log("Message received from " + SteamFriends.GetFriendPersonaName(senderID) + " " + senderID + " ID : " + messageID);

        Steamworks.EChatEntryType entry = Steamworks.EChatEntryType.k_EChatEntryTypeInvalid;
        string msg;

        SteamFriends.GetFriendMessage(senderID, messageID, out msg, 4096, out entry);

        if (entry == Steamworks.EChatEntryType.k_EChatEntryTypeChatMsg)
        {
            AutoReply(senderID, msg);
        }
    }

    void OnPlayersRequested(NumberOfCurrentPlayers_t pCallBack, bool bIOFailure)
    {
        if(pCallBack.m_bSuccess != 1 || bIOFailure)
        {
            Debug.Log("Error getting player quantity");
        }
        else
        {
            Debug.Log("Number of players playing : " + pCallBack.m_cPlayers);
        }
    }

    #endregion
}
