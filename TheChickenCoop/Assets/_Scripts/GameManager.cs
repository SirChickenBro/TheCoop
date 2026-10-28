using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    [SerializeField]
    private MultiplayerUI m_multiplayerUI;
    [SerializeField]
    private GameObject m_playerPrefab;

    [SerializeField]
    private List<ResourcePallet> m_pallets;

    public NetworkVariable<int> m_woodResource = new(0);
    public NetworkVariable<int> m_stoneResource = new(0);
    public NetworkVariable<int> m_cubeHealth = new(25);

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if(m_multiplayerUI != null)
        {
            m_multiplayerUI.OnStartHost += StartHost;
            m_multiplayerUI.OnStartClient += StartClient;
            m_multiplayerUI.OnDisconnectClient += DisconnectClient;
        }
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer == false)
            return;
        NetworkManager.OnClientConnectedCallback += SpawnPlayer;
        NetworkManager.SceneManager.OnLoadEventCompleted += HandleSceneLoadCompleted;
        /*
        foreach(ResourcePallet pallet in m_pallets)
        {
            pallet.OnPalletFilled += CheckWinCondition;
        }
        */
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback -= SpawnPlayer;
            NetworkManager.SceneManager.OnLoadEventCompleted -= HandleSceneLoadCompleted;
            
            
            /*
            foreach (ResourcePallet pallet in m_pallets)
            {
                pallet.OnPalletFilled -= CheckWinCondition;
            }
            */
        }
        base.OnNetworkDespawn();
    }

    private void DisconnectClient()
    {
        m_multiplayerUI.EnableButtons();
        NetworkManager.Shutdown();
    }

    private void SpawnPlayer(ulong clientID)
    {
        if (NetworkManager.ConnectedClients[clientID].PlayerObject != null)
            return;
        GameObject player = Instantiate(m_playerPrefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
    }

    private void HandleSceneLoadCompleted(string sceneName, LoadSceneMode loadSceneMode,
        List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach ( ulong clientId in clientsCompleted)
        {
            SpawnPlayer(clientId);
        }
    }

       /*
    private void CheckWinCondition()
    {
        int points = 0;
        foreach(ResourcePallet pallet in m_pallets)
        {
            points += pallet.StackedResources;
        }

        if(points >= m_pallets.Count * 3)
        {
            NetworkManager.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }
       */
    
    private void CheckLossCondition()
    {
        if(m_cubeHealth.Value <= 0)
        {
            NetworkManager.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }

    private void StartClient()
    {
        m_multiplayerUI.DisableButtons();
        NetworkManager.StartClient();
    }

    private void StartHost()
    {
        m_multiplayerUI.DisableButtons();
        NetworkManager.StartHost();
    }

    
}
