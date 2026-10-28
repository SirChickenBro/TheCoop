using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class ShadowScript : NetworkBehaviour, IInteractable
{
    [SerializeField] private SelectionOutline m_selectionOutline;

    [SerializeField]
    private NavMeshAgent _agent;
    private Vector3 _destination = new Vector3(0, 0, 0);

    private NetworkVariable<int> m_health = new(3);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
            m_health.Value = 3;
        _agent.SetDestination(_destination);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    public void getChopped()
    {
        m_health.Value--;
        Debug.Log(m_health.Value);
        if (m_health.Value < 1)
            shadowDie();
    }

    public void shadowDie()
    {
        if (!IsServer)
            return;
        Debug.Log("Shadow Die");
        Destroy(gameObject);

    }

    public void ToggleSelection(bool isSelected)
    {
        if (m_selectionOutline != null)
            m_selectionOutline.ToggleOutline(isSelected);
    }
    
    void OnTriggerEnter(Collider collider)
    {
        if (!IsServer)
            return;
        
        if (collider.gameObject.name == "MagicCube")
        {
            shadowDie();
            GameManager.instance.m_cubeHealth.Value--;
            Debug.Log(GameManager.instance.m_cubeHealth.Value);
        }
    }
}
