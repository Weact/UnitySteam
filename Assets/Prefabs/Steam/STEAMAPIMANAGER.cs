using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STEAMAPIMANAGER : MonoBehaviour
{
    public static STEAMAPIMANAGER instance;

    [SerializeField] public SteamNetwork network_manager = null;
    [SerializeField] public SteamLobby lobby_manager = null;

    private void Awake()
    {
        if (instance != null || network_manager == null || lobby_manager == null)
        {
            Debug.Log("ERROR, Could not create more than one instance of STEAMAPIMANAGER / Network Manager or Lobby Manager is null");
            return;
        }

        Debug.Log("Setting instance, network manager and lobby manager from STEAMAPIMANAGER...");

        instance = this;
        network_manager.m_lobby_manager = lobby_manager;
        lobby_manager.m_network_manager = network_manager;

        InitAPIs();
    }

    private void InitAPIs()
    {
        if(network_manager != null && lobby_manager != null)
        {
            network_manager.InitAPI();
            lobby_manager.InitAPI();
        }
    }

    public void CallApi()
    {
        Debug.Log("API Successfully Called !");
    }
}
