using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerMenu : MonoBehaviour
{
    private struct LobbyInfo
    {
        public LobbyInfo(string lobby_name, int lobby_type, int lobby_max_members)
        {
            LobbyName = lobby_name;
            LobbyType = lobby_type;
            LobbyMaxMembers = lobby_max_members;
        }

        public string LobbyName { get; set; }
        public int LobbyType { get; set; }
        public int LobbyMaxMembers { get; set; }

        public override string ToString()
        {
            return $"{LobbyName}, of type {LobbyType} with {LobbyMaxMembers} members";
        }
    }

    public Button createLobbyButton;
    public TMPro.TMP_InputField LobbyNameInputField;
    public TMPro.TMP_Dropdown LobbyTypeInputDropdown;
    public Slider LobbyMembersInputSlider;

    public GameObject LobbiesList = null;
    public GameObject prefabLobbyButtonJoin = null;

    public void Awake()
    {
        if (!STEAMAPIMANAGER.instance.IsInitialized())
        {
            Debug.Log("Could not initialize STEAMAPIMANAGER, going back to MainMenu");
            BackToMenu();
            return;
        }
        createLobbyButton.onClick.AddListener(SubmitLobby);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }


    //CREATE THE LOBBY WITH USER VARIABLES HE ENTERED IN SCENE
    public void SubmitLobby()
    {
        LobbyInfo lobby = new LobbyInfo
        {
            LobbyName = LobbyNameInputField.text,
            LobbyType = LobbyTypeInputDropdown.value,
            LobbyMaxMembers = 2
            //LobbyMaxMembers = (int)LobbyMembersInputSlider.value
        };

        if(lobby.LobbyName == "")
        {
            return;
        }

        Debug.Log($"Lobby has been submited for {lobby}");
        STEAMAPIMANAGER.instance.CreateLobby(lobby.LobbyName, (Steamworks.ELobbyType)lobby.LobbyType, lobby.LobbyMaxMembers);
        SceneManager.LoadScene("LobbyMenu");
    }

    // SHOULD UPDATE THE LIST OF LOBBIES FOUND
    // DOES NOT WORK FOR THE MOMENT..
    public void UpdateLobbyList()
    {
        if (!Steamworks.SteamAPI.Init())
        {
            return;
        }else if (!STEAMAPIMANAGER.instance.IsInitialized())
        {
            return;
        }

        STEAMAPIMANAGER.instance.SearchForLobbies();

        //string lobbyName = "lobby1";
        //GameObject LobbyButtonJoin = Instantiate(prefabLobbyButtonJoin, new Vector3(0, 0, 0), Quaternion.identity);
        //LobbyButtonJoin.transform.GetChild(0).gameObject.GetComponent<Text>().text = lobbyName;
        //LobbyButtonJoin.transform.SetParent(LobbiesList.transform);
    }
}
