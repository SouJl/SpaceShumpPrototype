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

    private bool _isExecute;

    void Awake()
    {
        txt = GetComponent<Text>();
        txt.text = "";
        _isExecute = false;
    }

    void Update()
    {
        if (Hero.S.isAlive) return;
        if (_isExecute) return;
        txt.text = "Game Over";
        AudioManager.instance.Stop("Theme");
        AudioManager.instance.Play("GameOver");
        Main.S.DelayedRestart(gameRestartDelay + AudioManager.instance.GetSoundLength("GameOver"));
        _isExecute = true;
    }
    

}
