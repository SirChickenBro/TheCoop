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

    public void getChopped()
    {
        m_health.Value--;
        Debug.Log(m_health.Value);
    }

    public void ToggleSelection(bool isSelected)
    {
        if (m_selectionOutline != null)
            m_selectionOutline.ToggleOutline(isSelected);
    }

}
