using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public float delayBeforeWave = 1f;
    [XmlArray("ships"), XmlArrayItem("ship")]
    public string[] ships;
    public bool delayNextWave = false;

    List<GameObject> shipsPrefub;
    private int shipsCount = 0;
    [System.NonSerialized]
    public bool IsEnd = false;
    public void InitShipsPrefub() => shipsPrefub = new List<GameObject>();

    public GameObject GetShipPrefub()
    {
        if (shipsPrefub != null)
        {
            shipsCount--;
            var result = shipsPrefub[shipsCount];
            if (shipsCount == 0)
            {
                shipsCount = shipsPrefub.Count;
                IsEnd = true;
            }
            return result;
        }
        else return null;
    }
    public void SetShipPrefub(GameObject prefub)
    {
        if (shipsPrefub != null)
        {
            shipsPrefub.Add(prefub);
            shipsCount++;
        }
    }

    // public bool IsEnd() => shipsCount > 0 ? false : true; 

}

[System.Serializable]
public class Level
{
    [XmlArray("waves"), XmlArrayItem("wave")]
    public Wave[] waves;
    public float timeLimit = -1;
    public string name = "";
}
