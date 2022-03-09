using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamNetwork : MonoBehaviour
{
    // TAKE THE REFERENCE OF THE LOBBY MANAGER
    public SteamLobby m_lobby_manager = null;


    //STRUCTURE TO STORE THE USER INFORMATIONS
    //ALL DATAS WILL BE STORED IN ONE PLACE
    //CONCERN ONLY THE LOCAL CONCERN
    public struct s_SteamUser
    {
        public CSteamID steamid { get; private set; }
        public string username { get; private set; }

        public bool hasLobby { get; private set; }
        private CSteamID lobbyId;
        
        public static s_SteamUser Init(CSteamID id, string name)
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


        public CSteamID LobbyID
        {
            get => lobbyId;
            set
            {
                if((uint)value == 0)
                {
                    hasLobby = false;
                    lobbyId = (CSteamID) 0;
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

    //DECLARATION OF THE USER
    public s_SteamUser user;

    #region callbacks

    protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;
    protected Callback<GameConnectedFriendChatMsg_t> m_GameConnectedFriendChatMsg;
    protected Callback<P2PSessionRequest_t> m_P2PSessionRequest;

    #endregion

    #region callresults

    private CallResult<NumberOfCurrentPlayers_t> m_CurrentPlayers;

    #endregion

    #region builtin-methods

    // Update is called once per frame
    // USED ONLY TO CHECK IF THE PLAYER REQUEST PLAYERS PLAYING THE SAME APPID
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SteamAPICall_t handle = SteamUserStats.GetNumberOfCurrentPlayers();
            m_CurrentPlayers.Set(handle);
        }
    }

    //INIT THE NETWORK API
    //CREATE CALLBACKS AND CALLRESULT
    //ENABLE LISTENING FOR FRIENDS MESSAGE SO THAT WE CAN USE THE "AUTOREPLY" METHOD AND "FRIENDSMESSAGE" CALLBACKS
    //>! AUTOREPLY METHOD IS DISABLED, TO BE USED CAREFULLY AND FOR DEBUG PURPOSES !<
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

            Debug.Log(user.ToString());
        }
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

    //void AutoReply(CSteamID senderID, string message)
    //{
    //    if(message == "invite me" || message == "inv me" || message == "invite moi" || message == "inv moi")
    //    {
    //        SteamFriends.ReplyToFriendMessage(senderID, "Got It !");
    //        InviteFriendToPlay(senderID);
    //        
    //        Debug.Log($"Replied to { SteamFriends.GetFriendPersonaName(senderID) } and sent an invite");
    //        return;
    //    }
    //
    //    SteamFriends.ReplyToFriendMessage(senderID, message);
    //   Debug.Log("Replied to " + SteamFriends.GetFriendPersonaName(senderID) + " " + senderID + " with " + message );
    //}

    //THIS METHOD TAKES AN USER STEAMID AND INVITE HIM TO LOBBY IF LOBBY VALID
    void InviteFriendToPlay(CSteamID friendIdToInvite)
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

        //INVITING USER AND DISPLAY DEBUG MESSAGE
        SteamMatchmaking.InviteUserToLobby(user.LobbyID, friendIdToInvite);
        Debug.Log($"{ SteamFriends.GetFriendPersonaName(friendIdToInvite) } has been Invited to play! Lobby ID : { user.LobbyID }");
    }

    #endregion

    #region callbacks-results

    //OVERLAY
    //DOES NOT WORK BECAUSE UNITY(?)
    //WORKS IF WE HIT <BUILD AND RUN> IN UNITY
    //DOES NOT WORK IN UNITY EDITOR ; DOES NOT WORK WHEN PROJECT IS BUILT
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

    //WHEN WE RECEIVE A MESSAGE FROM A FRIEND...
    void OnGameConnectedFriendChatMsg(GameConnectedFriendChatMsg_t pCallBack)
    {
        CSteamID senderID = pCallBack.m_steamIDUser;
        int messageID = pCallBack.m_iMessageID;

        EChatEntryType entry = EChatEntryType.k_EChatEntryTypeInvalid;
        string msg;

        //STORE FRIEND MESSAGE AS STRING INTO msg VARIABLE <string>
        SteamFriends.GetFriendMessage(senderID, messageID, out msg, 4096, out entry);

        //IF THE TYPE OF MESSAGE IS A VALID TEXT MESSAGE, PRINT DEBUG INFORMATIONS/RESULTS
        if (entry == Steamworks.EChatEntryType.k_EChatEntryTypeChatMsg)
        {
            Debug.Log("Message received from " + SteamFriends.GetFriendPersonaName(senderID) + " " + senderID + " ID : " + messageID);
            //AutoReply(senderID, msg); //DISABLE IT
        }
    }

    // space to trigger request number of player playing current game appid
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
