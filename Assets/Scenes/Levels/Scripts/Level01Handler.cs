using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Author : DRUCKES Lucas
/// This class handles Level related events
/// For example in Awake methods: enable player 2 controls if the game is started from multiplayer and user has lobby, etc...
/// Other than that, its purpose its just to pause, resume or leave the game.
/// </summary>

public class Level01Handler : MonoBehaviour
{
    public bool disabled = false;

    public GameObject playersContainer; //player container object in Level01
    public GameObject pauseMenu; //pauseMenu gameobject in Level01
    public GameObject playerPrefab; //playerPrefab
    public Material player2Material; //player2Material prefab

    private bool paused = false; //paused game stated

    //CHECKING IF ITS A MULTIPLAYER GAME :
    //IF STEAMWORKS STEAMAPI INITIALIZED, STEAMAPIMANAGER INITIALIZED, IF PLAYER HAS LOBBY AND PLAYER IS NOT THE HOST OF THE LOBBY
    //SPAWN A PLAYER 2 OBJECT IN GAME AND MAKE IT CONTROLLABLE WITH <>.controller = true; line 35
    private void Awake()
    {
        if (!disabled)
        {
            if (Steamworks.SteamAPI.Init())
            {
                if (STEAMAPIMANAGER.instance != null && STEAMAPIMANAGER.instance.IsInitialized())
                {
                    if (STEAMAPIMANAGER.instance.network_manager.user.hasLobby)
                    {
                        if (STEAMAPIMANAGER.instance.network_manager.user.steamid != STEAMAPIMANAGER.instance.GetLobbyHostSteamID())
                        {
                            GameObject player2 = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                            player2.GetComponent<MeshRenderer>().material = player2Material;
                            player2.transform.SetParent(playersContainer.transform);
                            player2.transform.localPosition = new Vector3(10,10,10);

                            player2.GetComponent<BunnyHopper>().playerID = 2;
                            player2.GetComponent<BunnyHopper>().controlled = true;

                            Debug.Log($"Player 2 is activated : {player2.GetComponent<BunnyHopper>().controlled} his ID : {player2.GetComponent<BunnyHopper>().playerID}");
                        }
                    }
                }
            }
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Start is called before the first frame update
    //SET THE GAME NOT PAUSED
    private void Start()
    {
        SetPauseGame(false);
    }

    // Update is called once per frame
    //SWITCH PAUSED STATE IF ESCAPE IS PRESSED
    private void Update()
    {
        if (Input.GetButtonDown("PauseGame"))
        {
            SetPauseGame(!paused);
        }
    }

    //CHANGE CURSOR MODE AND VISIBILITY ACCORDING TO PAUSE STATE
    public void SetPauseGame(bool paused_state)
    {
        paused = paused_state;
        pauseMenu.SetActive(paused);
        Cursor.visible = paused;

        if (paused)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
             Cursor.lockState = CursorLockMode.Locked;
        }
    }

    //LEAVE THE GAME AND MAKES SURE THAT WE LEAVE THE LOBBY IF IT WAS A MULTIPLAYER GAME
    public void LeaveGame()
    {
        if (Steamworks.SteamAPI.Init())
        {
            if(STEAMAPIMANAGER.instance != null)
            {
                if (STEAMAPIMANAGER.instance.network_manager.user.hasLobby)
                {
                    STEAMAPIMANAGER.instance.LeaveLobby();
                }
            }
        }

        SceneManager.LoadScene("TitleScreen");
    }
}
