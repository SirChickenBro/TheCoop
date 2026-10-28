using UnityEngine;
using Unity.Netcode; 

public class MagicCube : NetworkBehaviour
{
    public NetworkVariable<int> cube_health = new(25);

    /*
    public void Update()
    {
        if(Collider.)
        material.SetColor("_EmissionColor", yourNewEmissionColour);
    }
    */
}
