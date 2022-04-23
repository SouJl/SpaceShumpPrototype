using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultScoreUI : MonoBehaviour
{
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

        txt.text = $"You final score: {Main.S.score}";
    }
}
