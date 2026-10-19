using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private PlayerInput m_playerInput;
    [SerializeField]
    private AgentMover m_agentMover;

    private void Update()
    {
        if (IsOwner == false)
            return;

        Vector2 movementInput = m_playerInput.MovementInput;
        m_agentMover.Move(movementInput);

    }
}
