using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;
using Unity.Netcode.Components;

public class ResourceFortifications : NetworkBehaviour, IInteractable
{
    [SerializeField] private SelectionOutline m_selectionOutline;

    [SerializeField]
    private List<ComponentController> m_componentControllers;

    [SerializeField]
    private ComponentController m_obstacleActive;

    [SerializeField]
    private List<ObjectType> m_acceptedObjectTypes;

    [SerializeField]
    private ObjectType m_currentObjectType;

    private NetworkVariable<int> m_stackedResources = new(0);

    public int StackedResources => m_stackedResources.Value;
    public event Action OnPalletFilled;

    [SerializeField] private ItemsAudio m_itemsAudio;

    public bool Interact(ObjectType objectType)
    {
        if (IsServer == false)
            return false;
        if (m_stackedResources.Value == 0)
            m_currentObjectType = objectType;
        if (objectType != m_currentObjectType)
            return false;
        if (m_stackedResources.Value >= m_componentControllers.Count)
            return false;
        PlayerAudioClientRpc();
        m_componentControllers[m_stackedResources.Value].SetEnabled(true);
        m_stackedResources.Value++;
        if(m_stackedResources.Value > 0)
            m_obstacleActive.SetEnabled(true);
        Debug.Log(m_stackedResources.Value);
        if (m_stackedResources.Value >= m_componentControllers.Count)
        {
            OnPalletFilled?.Invoke();
        }
        return true;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayerAudioClientRpc()
    {
        if (m_itemsAudio != null)
            m_itemsAudio.PlaySound();
    }

    public void ToggleSelection(bool isSelected)
    {
        if (m_selectionOutline != null)
            m_selectionOutline.ToggleOutline(isSelected);
    }
}
