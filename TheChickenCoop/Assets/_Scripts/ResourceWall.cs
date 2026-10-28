using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;
using Unity.Netcode.Components;

public class ResourceWall : NetworkBehaviour, IInteractable
{
    [SerializeField] private SelectionOutline m_selectionOutline;

    [SerializeField]
    private ComponentController m_woodWallController;

    [SerializeField]
    private ComponentController m_stoneWallController;
    
    [SerializeField]
    private ComponentController m_obstacleIsActive;

    [SerializeField] private int woodWallMaxDurability;
    [SerializeField] private int stoneWallMaxDurability;

    [SerializeField]
    private ObjectType m_currentObjectType;

    public NetworkVariable<int> m_wallDurability = new(0);

    public bool canBeBuilt = true;
    public bool canBeRepaired = false;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        m_woodWallController.SetEnabled(false);
        m_stoneWallController.SetEnabled(false);
        m_obstacleIsActive.SetEnabled(false);

        canBeBuilt = true;
        canBeRepaired = false;
    }

    public void buildWall(ObjectType objectType)
    {
        if (IsServer == false)
            return;
        if (m_wallDurability.Value > 0)
            return;
        if(m_wallDurability.Value <= 0 || m_currentObjectType == ObjectType.None)
            m_currentObjectType = objectType;

        if (m_currentObjectType == ObjectType.Stone)
        {
            if (GameManager.instance.m_stoneResource.Value < 200)
                return;

            GameManager.instance.m_stoneResource.Value -= 200;
            m_stoneWallController.SetEnabled(true);
            m_wallDurability.Value = stoneWallMaxDurability;
            Debug.Log("Stone Wall");

            Debug.Log(GameManager.instance.m_stoneResource.Value);
        }

        if (m_currentObjectType == ObjectType.Wood)
        {
            if (GameManager.instance.m_woodResource.Value < 100)
                return;

            GameManager.instance.m_woodResource.Value -= 100;
            m_woodWallController.SetEnabled(true);
            m_wallDurability.Value = woodWallMaxDurability;
            Debug.Log("Wood Wall");

            Debug.Log(GameManager.instance.m_woodResource.Value);
        }

        m_obstacleIsActive.SetEnabled(true);
    }

    public void upgradeWall()
    {
        if (IsServer == false)
            return;

        if (m_currentObjectType == ObjectType.Stone)
            return;

        if (GameManager.instance.m_stoneResource.Value < 100)
            return;

        m_currentObjectType = ObjectType.Stone;
        GameManager.instance.m_stoneResource.Value -= 100;
        GameManager.instance.m_woodResource.Value += 50;
        m_woodWallController.SetEnabled(false);
        m_stoneWallController.SetEnabled(true);
        m_wallDurability.Value = stoneWallMaxDurability;
    }

    public void repairWall()
    {
        if (IsServer == false)
            return;

        if (m_currentObjectType == ObjectType.Stone)
        {
            if (m_wallDurability.Value == stoneWallMaxDurability)
                return;
            if (GameManager.instance.m_woodResource.Value < 100)
                return;

            m_wallDurability.Value = stoneWallMaxDurability;
            GameManager.instance.m_stoneResource.Value -= 100;
        }

        if (m_currentObjectType == ObjectType.Wood)
        {
            if (m_wallDurability.Value == woodWallMaxDurability)
                return;
            if (GameManager.instance.m_woodResource.Value < 50)
                return;

            m_wallDurability.Value = woodWallMaxDurability;
            GameManager.instance.m_woodResource.Value -= 50;
        }  
    }

    public void onWallDestroyed()
    {
        if (!IsServer)
            return;

        if (m_wallDurability.Value <= 0)
        {
            m_currentObjectType = ObjectType.None;
            m_stoneWallController.SetEnabled(false);
            m_woodWallController.SetEnabled(false);
            m_obstacleIsActive.SetEnabled(false);
        }
    }

    public void ToggleSelection(bool isSelected)
    {
        if (m_selectionOutline != null)
            m_selectionOutline.ToggleOutline(isSelected);
    }
}
