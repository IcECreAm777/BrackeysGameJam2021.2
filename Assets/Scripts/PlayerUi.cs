using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour
{
    [Header("Cross hair")]
    [SerializeField]
    private Image crossHair;

    [Header("Knife Control")]
    [SerializeField]
    private Text knifeCooldown;
    [SerializeField]
    private Image knifeIcon;
    [SerializeField] 
    private string knifeText;
    [SerializeField]
    private Image knifePanel;

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
    private Image boomerangPanel;
    
    [Header("Boomerang Control")]
    [SerializeField]
    private Text bowlCooldown;
    [SerializeField]
    private Image bowlIcon;
    [SerializeField] 
    private string bowlText;
    [SerializeField]
    private Image bowlPanel;

    [Header("Bowls Left")]
    [SerializeField]
    private List<Image> bowlsLeft;

    [Header("Game Start")]
    [SerializeField]
    private Text startGameCountdown;
    
    // not exposed vars
    private bool _boomerangReturned;
    private bool _boomerangCd;
    
    // constants
    private const float IconAlphaMin = 100.0f;
    private const float IconAlphaMax = 255.0f;
    private const float PanelAlphaMin = 0.0f;
    private const float PanelAlphaMax = 5.0f;

#if DEBUG
    void Awake()
    {
        if (crossHair == null)
        {
            Debug.LogError("The Image for the crosshair was not assigned for the Script");
        }
    }
#endif

    private void Start()
    {
        knifeCooldown.text = knifeText;
        boomerangCooldown.text = boomerangText;
        bowlCooldown.text = bowlText;
    }

    public void UpdateBoomerangCatched(bool catched)
    {
        _boomerangReturned = catched;

        if (!_boomerangCd) return;
        boomerangCooldown.text = boomerangText;
        ChangeImageAlpha(ref boomerangIcon, IconAlphaMax);
        ChangeImageAlpha(ref boomerangPanel, PanelAlphaMax);
    }

    public void UpdateBoomerangCooldown(float cooldown)
    {
        _boomerangCd = cooldown <= 0.0f;
        var alphaIcon = _boomerangCd && _boomerangReturned ? IconAlphaMax : IconAlphaMin;
        var alphaPanel = _boomerangCd && _boomerangReturned ? PanelAlphaMax : PanelAlphaMin;

        var text = _boomerangCd
            ? _boomerangReturned ? boomerangText : boomerangTravellingText
            : cooldown.ToString("F1");

        boomerangCooldown.text = text;
        ChangeImageAlpha(ref boomerangIcon, alphaIcon);
        ChangeImageAlpha(ref boomerangPanel, alphaPanel);
    }

    public void UpdateKnifeCooldown(float cooldown)
    {
        var text = cooldown <= 0.0f ? knifeText : cooldown.ToString("F1");
        var alphaIcon = cooldown <= 0.0f ? IconAlphaMax : IconAlphaMin;
        var alphaPanel = cooldown <= 0.0f ? PanelAlphaMax : PanelAlphaMin;

        knifeCooldown.text = text;
        ChangeImageAlpha(ref knifeIcon, alphaIcon);
        ChangeImageAlpha(ref knifePanel, alphaPanel);
    }

    public void UpdateBowlCooldown(float cooldown)
    {
        var text = cooldown <= 0.0f ? bowlText : cooldown.ToString("F1");
        var alphaIcon = cooldown <= 0.0f ? IconAlphaMax : IconAlphaMin;
        var alphaPanel = cooldown <= 0.0f ? PanelAlphaMax : PanelAlphaMin;

        bowlCooldown.text = text;
        ChangeImageAlpha(ref bowlIcon, alphaIcon);
        ChangeImageAlpha(ref bowlPanel, alphaPanel);
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
    
    // UTILITY METHODS
    private void ChangeImageAlpha(ref Image image, float alpha)
    {
        alpha /= 255.0f;
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
