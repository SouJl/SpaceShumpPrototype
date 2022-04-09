using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float gameRestartDelay = 3f;

    [Header("Set Dynamically")]
    public Text txt;
 

    void Awake()
    {
        txt = GetComponent<Text>();
        txt.text = "";
    }

    void Update()
    {
        if (Hero.S.isAlive) return;

        txt.text = "Game Over";
        Main.S.DelayedRestart(gameRestartDelay);
    }
    

}
