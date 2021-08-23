using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour
{
    [SerializeField]
    private Image crossHair;

#if DEBUG
    void Awake()
    {
        if (crossHair == null)
        {
            Debug.LogError("The Image for the crosshair was not assigned for the Script");
        }
    }
#endif

    public void UpdateBoomerangCooldown(float cooldown)
    {
        //TODO set text of a label to the number
    }

    public void UpdateSwordCooldown(float cooldown)
    {
        //TODO set text of a label to the number
    }

    public void UpdateBowlCooldown(float cooldown)
    {
        //TODO set text of a label to the number
    }

    public void UpdateCrossHairPosition(Vector2 mousePos)
    {
        crossHair.transform.position = mousePos;
    }
}
