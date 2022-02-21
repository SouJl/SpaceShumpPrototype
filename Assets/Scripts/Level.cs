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

    Queue<GameObject> shipsPrefub;
    private int shipsCount = 0;
    public void InitShipsPrefub() => shipsPrefub = new Queue<GameObject>();

    public GameObject GetShipPrefub()
    {
        if (shipsPrefub != null)
        {
            shipsCount--;
            return shipsPrefub.Peek();
        }  
        else return null;
    }
    public void SetShipPrefub(GameObject prefub)
    {
        if (shipsPrefub != null)
        {
            shipsPrefub.Enqueue(prefub);
            shipsCount++;
        }         
    }

    public bool IsEnd() => shipsCount > 0 ? false : true; 

}

[System.Serializable]
public class Level
{
    [XmlArray("waves"), XmlArrayItem("wave")]
    public Wave[] waves;
    public float timeLimit = -1;
    public string name = "";
}
