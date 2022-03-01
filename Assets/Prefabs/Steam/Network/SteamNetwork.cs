using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamNetwork : MonoBehaviour
{
    public SteamLobby m_lobby_manager = null;

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
                    lobbyId = (Steamworks.CSteamID) 0;
                    Debug.Log($"LOBBY ID HAS BEEN SET TO 0 ! [ lobby id : {lobbyId} ]");
                }
                else
                {
                    hasLobby = true;
                    lobbyId = value;
                    Debug.Log($"LOBBY ID HAS BEEN SET ! [ lobby id : {lobbyId} ]");
                }
            }
        }

        public override string ToString() => $"USER ID : {steamid} | USER NAME : {username} | IS IN LOBBY : {hasLobby} | LOBBY ID : {LobbyID}";
    }

    public s_SteamUser user;

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


    public void InitAPI()
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

            //if (m_lobby_manager != null)
            //{
            //    Debug.Log("Making an attempt to create a lobby with 5 max_players");
            //    m_lobby_manager.CreateLobby(Steamworks.ELobbyType.k_ELobbyTypePublic, 5);
            //}
            //else
            //{
            //    Debug.Log("Lobby Manager is null");
            //}

            Debug.Log(user.ToString());
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
        m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
        m_GameConnectedFriendChatMsg = Callback<GameConnectedFriendChatMsg_t>.Create(OnGameConnectedFriendChatMsg);
    }

    void CreateCallResults()
    {
        m_CurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(OnPlayersRequested);
    }

    void AutoReply(Steamworks.CSteamID senderID, string message)
    {
        if(message == "invite me" || message == "inv me" || message == "invite moi" || message == "inv moi")
        {
            SteamFriends.ReplyToFriendMessage(senderID, "Got It !");
            InviteFriendToPlay(senderID);
            
            Debug.Log($"Replied to { SteamFriends.GetFriendPersonaName(senderID) } and sent an invite");
            return;
        }

        SteamFriends.ReplyToFriendMessage(senderID, message);
        Debug.Log("Replied to " + SteamFriends.GetFriendPersonaName(senderID) + " " + senderID + " with " + message );
    }

    void InviteFriendToPlay(Steamworks.CSteamID friendIdToInvite)
    {
        if( (uint) user.LobbyID == 0 )
        {
            Debug.Log("Tried to invite a friend to a Invalid Lobby");
            return;
        }
        if( (uint)friendIdToInvite == 0)
        {
            Debug.Log("Tried to invite a friend with invalid Steam ID");
            return;
        }

        SteamMatchmaking.InviteUserToLobby(user.LobbyID, friendIdToInvite);
        Debug.Log($"{ SteamFriends.GetFriendPersonaName(friendIdToInvite) } has been Invited to play! Lobby ID : { user.LobbyID }");
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

        Steamworks.EChatEntryType entry = Steamworks.EChatEntryType.k_EChatEntryTypeInvalid;
        string msg;

        SteamFriends.GetFriendMessage(senderID, messageID, out msg, 4096, out entry);

        if (entry == Steamworks.EChatEntryType.k_EChatEntryTypeChatMsg)
        {
            Debug.Log("Message received from " + SteamFriends.GetFriendPersonaName(senderID) + " " + senderID + " ID : " + messageID);
            AutoReply(senderID, msg);
        }
    }

    // space to trigger
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
