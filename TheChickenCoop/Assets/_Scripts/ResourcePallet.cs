using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;
using Unity.Netcode.Components;

public class ResourcePallet : NetworkBehaviour, IInteractable
{
    [SerializeField] private SelectionOutline m_selectionOutline;

    [SerializeField]
    private List<ComponentController> m_componentControllers;
    
    [SerializeField]
    private ObjectType m_acceptedObjectType;

    private NetworkVariable<int> m_stackedResources = new(0);

    public int StackedResources => m_stackedResources.Value;
    public event Action OnPalletFilled;

    [SerializeField] private ItemsAudio m_itemsAudio;

    public bool Interact(ObjectType objectType)
    {
        if (IsServer == false)
            return false;
        if (objectType != m_acceptedObjectType)
            return false;
        if (m_stackedResources.Value >= m_componentControllers.Count)
            return false;
        PlayerAudioClientRpc();
        m_componentControllers[m_stackedResources.Value].SetEnabled(true);
        m_stackedResources.Value++;
        if(m_stackedResources.Value >= m_componentControllers.Count)
        {
            OnPalletFilled?.Invoke();
            if (objectType is ObjectType.Stone)
                GameManager.instance.m_stoneResource.Value += 200;
            if (objectType is ObjectType.Wood)
                GameManager.instance.m_woodResource.Value += 200;

            Debug.Log(GameManager.instance.m_woodResource.Value);
            Debug.Log(GameManager.instance.m_stoneResource.Value);
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
