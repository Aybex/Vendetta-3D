using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RessourceType
{
    Wood,
    Stone
}

[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory/Inventory")]
public class Inventory : ScriptableObject
{
    public uint wood = 0;
    public uint stone = 0;

    public void AddRessource(RessourceType type, uint amount)
    {
        switch (type)
        {
            case RessourceType.Wood:
                wood += amount;
                break;
            case RessourceType.Stone:
                stone += amount;
                break;
        }
    }

}
