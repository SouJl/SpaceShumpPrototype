using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    float delayBeforeWave = 1f;
    GameObject[] ships;
    bool delayNextWave = false;
}

[System.Serializable]
public class Level : MonoBehaviour
{
    Wave[] waves;
    float timeLimit = -1;
    string name = "";
}
