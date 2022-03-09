using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level01Handler : MonoBehaviour
{
    public GameObject playersContainer;
    public GameObject pauseMenu;
    public GameObject playerPrefab;
    public Material player2Material;

    private bool paused = false;

    private void Awake()
    {
        if (Steamworks.SteamAPI.Init())
        {
            if (STEAMAPIMANAGER.instance.IsInitialized())
            {
                if (STEAMAPIMANAGER.instance.network_manager.user.steamid != STEAMAPIMANAGER.instance.GetLobbyHostSteamID())
                {
                    GameObject player2 = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    player2.GetComponent<PlayerController>().playerBodyObject.GetComponent<MeshRenderer>().material = player2Material;
                    player2.transform.SetParent(playersContainer.transform);

                    //player2.GetComponent<PlayerController>().playerID = 2;
                    //player2.GetComponent<PlayerController>().controlled = true;
                }
            }
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        SetPauseGame(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("PauseGame"))
        {
            SetPauseGame(!paused);
        }
    }

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
