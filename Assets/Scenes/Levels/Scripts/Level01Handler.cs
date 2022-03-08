using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level01Handler : MonoBehaviour
{
    public GameObject pauseMenu;

    private bool paused = false;

    private void Awake()
    {
        
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
