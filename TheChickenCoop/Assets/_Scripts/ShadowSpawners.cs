using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class ShadowSpawners : NetworkBehaviour
{
    [SerializeField]
    private List<Transform> m_zombieSpawnPoints;

    [SerializeField]
    private NetworkObject m_shadowPrefab;

    [SerializeField]
    private int m_MaxSpawnWaves;

    private int m_SpawnWaves;

    [SerializeField]
    float m_SpawnRatePerSecond;

    private float m_lastSpawnTime;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsServer;
        if (!IsServer)
            return;

        m_SpawnWaves = 0;
    }

    public override void OnNetworkDespawn()
    {
        m_SpawnWaves = 0;
        enabled = false;
    }

    void FixedUpdate()
    {
        if (!IsServer)
            return;

        if (m_SpawnWaves < m_MaxSpawnWaves && Time.time - m_lastSpawnTime > m_SpawnRatePerSecond)
        {
            foreach (Transform spawnPoint in m_zombieSpawnPoints)
            {
                var newShadow = m_shadowPrefab.InstantiateAndSpawn(NetworkManager,
                        position: spawnPoint.transform.position,
                        rotation: spawnPoint.transform.rotation);
                var shadow = newShadow.GetComponent<ShadowScript>();
            }
            //m_SpawnWaves++;

            m_lastSpawnTime = Time.time;
        }
    }
}
