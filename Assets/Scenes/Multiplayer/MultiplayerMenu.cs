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

    public void Awake()    {

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

    public void SubmitLobby()
    {
        LobbyInfo lobby = new LobbyInfo
        {
            LobbyName = LobbyNameInputField.text,
            LobbyType = LobbyTypeInputDropdown.value,
            LobbyMaxMembers = 2
            //LobbyMaxMembers = (int)LobbyMembersInputSlider.value
        };

        Debug.Log($"Lobby has been submited for {lobby}");
        STEAMAPIMANAGER.instance.CreateLobby(lobby.LobbyName, (Steamworks.ELobbyType)lobby.LobbyType, lobby.LobbyMaxMembers);
        SceneManager.LoadScene("LobbyMenu");
    }
}
