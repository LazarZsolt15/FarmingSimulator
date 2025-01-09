using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    public Upgrade[] upgrades;

    void Start()
    {
       
    }

    void Update()
    {
        
    }
     public void InitializeAllUpgrades()
    {
        upgrades = FindObjectsOfType<Upgrade>();

        foreach (var upgrade in upgrades)
        {
            upgrade.Initialize();
        }
    }
}
