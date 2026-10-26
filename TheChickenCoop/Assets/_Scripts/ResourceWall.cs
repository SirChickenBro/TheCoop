using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;
using Unity.Netcode.Components;

public class ResourceWall : NetworkBehaviour, IInteractable
{
    [SerializeField] private SelectionOutline m_selectionOutline;

    [SerializeField]
    private List<ComponentController> m_componentControllers;

    [SerializeField] private int woodWallMaxDurability;
    [SerializeField] private int stoneWallMaxDurability;

    [SerializeField]
    private ObjectType m_currentObjectType;

    private NetworkVariable<int> m_wallDurability = new(0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buildWall()
    {

    }

    public void upgradeWall()
    {
        if (m_currentObjectType == ObjectType.Stone)
            return;

        m_currentObjectType = ObjectType.Stone;
        GameManager.instance.m_stoneResource.Value += 50;
        GameManager.instance.m_woodResource.Value += 50;
        m_wallDurability.Value = stoneWallMaxDurability;
    }

    public void repairWall()
    {
        if (m_currentObjectType == ObjectType.Stone)
        {
            if (m_wallDurability.Value == stoneWallMaxDurability)
                return;

            m_wallDurability.Value = stoneWallMaxDurability;
            GameManager.instance.m_stoneResource.Value -= 100;
        }

        if (m_currentObjectType == ObjectType.Wood)
        {
            if (m_wallDurability.Value == woodWallMaxDurability)
                return;

            m_wallDurability.Value = woodWallMaxDurability;
            GameManager.instance.m_woodResource.Value -= 50;
        }  
    }

    public void ToggleSelection(bool isSelected)
    {
        if (m_selectionOutline != null)
            m_selectionOutline.ToggleOutline(isSelected);
    }
}
