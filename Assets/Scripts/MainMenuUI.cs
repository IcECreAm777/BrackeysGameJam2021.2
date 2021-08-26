using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Areas")]
    [SerializeField]
    private GameObject buttonArea;
    [SerializeField]
    private GameObject creditsArea;
    [SerializeField]
    private GameObject howToPlayArea;
    
    [Header("Button Area")] 
    [SerializeField]
    private Button playButton;
    [SerializeField]
    private Button howToPlayButton;
    [SerializeField]
    private Button creditsButton;
    [SerializeField]
    private Button quitButton;
    
    [Header("How To Play Area")]
    [SerializeField]
    private Button closeHowToPlayButton;

    [Header("Credits Area")]
    [SerializeField]
    private Button closeCreditsButton;
    
    // ENGINE METHODS
    
    private void Awake()
    {
        playButton.onClick.AddListener(PlayGame);
        quitButton.onClick.AddListener(QuitGame);
        
        howToPlayButton.onClick.AddListener(ShowHowToPlay);
        closeHowToPlayButton.onClick.AddListener(HideHowToPlay);
        
        creditsButton.onClick.AddListener(ShowCredits);
        closeCreditsButton.onClick.AddListener(HideCredits);
    }
    
    // BUTTON METHODS

    private void PlayGame()
    {
        SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void ShowCredits()
    {
        buttonArea.SetActive(false);
        creditsArea.SetActive(true);
    }

    private void HideCredits()
    {
        buttonArea.SetActive(true);
        creditsArea.SetActive(false);
    }

    private void ShowHowToPlay()
    {
        buttonArea.SetActive(false);
        howToPlayArea.SetActive(true);
    }

    private void HideHowToPlay()
    {
        buttonArea.SetActive(true);
        howToPlayArea.SetActive(false);
    }
}
