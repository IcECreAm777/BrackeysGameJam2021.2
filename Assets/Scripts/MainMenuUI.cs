using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")] 
    [SerializeField]
    private Button playButton;
    [SerializeField]
    private Button quitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(PlayGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void PlayGame()
    {
        SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
