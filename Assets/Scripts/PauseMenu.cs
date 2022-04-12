using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] 
    private RectTransform _pauseMenu;

    private bool _isEnable;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_pauseMenu.gameObject.activeSelf)
            {
                SetPause();
            }
        }
    }

    private void SetPause()
    {
        _isEnable = !_isEnable;
      
        if (_isEnable)
        {
            Time.timeScale = 0f;
            _pauseMenu.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            _pauseMenu.gameObject.SetActive(false);
        }
    }

    public void OnResumeButtonClick()
    {
        SetPause();
    }

    public void OnMainMenuButtonClick()
    {
        SetPause();
        SceneManager.LoadScene("MenuScene");
    }
}
