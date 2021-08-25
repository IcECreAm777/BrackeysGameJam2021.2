using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour
{
    [Header("Cross hair")]
    [SerializeField]
    private Image crossHair;

    [Header("Pause Screen")] 
    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private Button continueButton;
    [SerializeField]
    private Button quitButton;

    [Header("Knife Control")]
    [SerializeField]
    private Text knifeCooldown;
    [SerializeField]
    private Image knifeIcon;
    [SerializeField] 
    private string knifeText;
    [SerializeField]
    private Slider knifeSlider;

    [Header("Boomerang Control")]
    [SerializeField]
    private Text boomerangCooldown;
    [SerializeField]
    private Image boomerangIcon;
    [SerializeField] 
    private string boomerangText;
    [SerializeField]
    private string boomerangTravellingText;
    [SerializeField]
    private Slider boomerangSlider;
    
    [Header("BowlControl")]
    [SerializeField]
    private Text bowlCooldown;
    [SerializeField]
    private Image bowlIcon;
    [SerializeField] 
    private string bowlText;
    [SerializeField]
    private Slider bowlSlider;

    [Header("Stamina")]
    [SerializeField]
    private Slider staminaSlider;

    [Header("Bowls Left")]
    [SerializeField]
    private List<Image> bowlsLeft;

    [Header("Game Control")]
    [SerializeField]
    private Text startGameCountdown;
    [SerializeField]
    private Text timeLeft;
    
    // not exposed vars
    private bool _boomerangReturned;
    private bool _boomerangCd;

    // constants
    private const float IconAlphaMin = 100.0f;
    private const float IconAlphaMax = 255.0f;
    
    
    // ENGINE METHODS
    
    void Awake()
    {
        continueButton.onClick.AddListener(OnContinue);
        quitButton.onClick.AddListener(OnQuit);
        
#if DEBUG
        if (crossHair == null)
        {
            Debug.LogError("The Image for the cross hair was not assigned for the Script");
        }
#endif
    }

    private void Start()
    {
        knifeCooldown.text = knifeText;
        boomerangCooldown.text = boomerangText;
        bowlCooldown.text = bowlText;
    }
    
    // UI METHODS
    
    private void OnContinue()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1.0f;
    }

    private void OnQuit()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    // UPDATE UI METHODS

    public void Initialize(float boomerCd, float knifeCd, float bowlCd, float stamina)
    {
        bowlSlider.maxValue = bowlCd;
        boomerangSlider.maxValue = boomerCd;
        knifeSlider.maxValue = knifeCd;
        staminaSlider.maxValue = stamina;

        bowlSlider.value = bowlCd;
        boomerangSlider.value = boomerCd;
        knifeSlider.value = knifeCd;
        staminaSlider.value = stamina;
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        pauseScreen.SetActive(true);
    }

    public void UpdateBoomerangCatched(bool catched)
    {
        _boomerangReturned = catched;

        if (!_boomerangCd) return;
        boomerangCooldown.text = boomerangText;
        ChangeImageAlpha(ref boomerangIcon, IconAlphaMax);
    }

    public void UpdateBoomerangCooldown(float cooldown)
    {
        _boomerangCd = cooldown <= 0.0f;
        var alphaIcon = _boomerangCd && _boomerangReturned ? IconAlphaMax : IconAlphaMin;

        var text = _boomerangCd
            ? _boomerangReturned ? boomerangText : boomerangTravellingText
            : cooldown.ToString("F1");

        boomerangSlider.value = boomerangSlider.maxValue - cooldown;
        boomerangCooldown.text = text;
        ChangeImageAlpha(ref boomerangIcon, alphaIcon);
    }

    public void UpdateKnifeCooldown(float cooldown)
    {
        var text = cooldown <= 0.0f ? knifeText : cooldown.ToString("F1");
        var alphaIcon = cooldown <= 0.0f ? IconAlphaMax : IconAlphaMin;

        knifeSlider.value = knifeSlider.maxValue - cooldown;
        knifeCooldown.text = text;
        ChangeImageAlpha(ref knifeIcon, alphaIcon);
    }

    public void UpdateBowlCooldown(float cooldown)
    {
        var text = cooldown <= 0.0f ? bowlText : cooldown.ToString("F1");
        var alphaIcon = cooldown <= 0.0f ? IconAlphaMax : IconAlphaMin;

        bowlSlider.value = bowlSlider.maxValue - cooldown;
        bowlCooldown.text = text;
        ChangeImageAlpha(ref bowlIcon, alphaIcon);
    }

    public void UpdateCrossHairPosition(Vector2 mousePos)
    {
        crossHair.transform.position = mousePos;
    }

    public void UpdateBowlsLeft(int bowls)
    {
        bowlsLeft[bowls].enabled = false;
    }

    public void UpdateCountdown(int countdown)
    {
        var text = countdown > 0 ? countdown.ToString() : countdown == 0 ? "Start" : "";
        startGameCountdown.text = text;
    }

    public void UpdateTimeLeft(float left)
    {
        if (left <= 0.0f)
        {
            timeLeft.enabled = false;
            return;
        }
        
        var text = $"Time Left: {left:F1}";
        timeLeft.text = text;
    }

    public void UpdateStamina(float stamina)
    {
        staminaSlider.value = stamina;
    }
    
    // UTILITY METHODS
    private void ChangeImageAlpha(ref Image image, float alpha)
    {
        alpha /= 255.0f;
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
