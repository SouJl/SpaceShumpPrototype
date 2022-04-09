using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLogic : MonoBehaviour
{
    [Header("Set in Inspector")]
    public Button playButton;
    public Button exitButton;
    public Button nextDiffButton;
    public Button prevDiffButton;
    public TextMeshProUGUI diffDescription;

    
    string[] _difficulties = { "easy", "normal", "hard" };    
    private int _difficalty = 0;
    
    // Start is called before the first frame update
    void Awake()
    {
        playButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(ExitGame);
        nextDiffButton.onClick.AddListener(UppDiff);
        prevDiffButton.onClick.AddListener(DownDiff);
        if (PlayerPrefs.HasKey("Difficulty"))
        {
            _difficalty = PlayerPrefs.GetInt("Difficulty");
        }
        PlayerPrefs.SetInt("Difficulty", _difficalty);
    }


    private void LateUpdate()
    {
        ChangeDifficalty();
    }

    void StartGame()
    {
        PlayerPrefs.SetInt("Difficulty", _difficalty);
        SceneManager.LoadScene(1);
    }

    void UppDiff()
    {
        _difficalty++;
        if (_difficalty >= _difficulties.Length) _difficalty = _difficulties.Length - 1;
    }

    void DownDiff()
    {
        _difficalty--;
        if (_difficalty < 0) _difficalty = 0;
    }

    void ChangeDifficalty()
    {
        diffDescription.text = _difficulties[_difficalty];
    }

    public void ExitGame()
    {
        PlayerPrefs.SetInt("Difficulty", _difficalty);
        Application.Quit();
    }

}
