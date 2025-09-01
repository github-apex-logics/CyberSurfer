
using UnityEngine;
using UnityEngine.UI;

public class UIControllerBtns : MonoBehaviour
{


    public Button powerUpBtn;
    public Image powerUpIconA, powerUpIconB;
    public Sprite slash, homing, boost,shield;
    public Powers powerUp;
   public  Powers slotA = Powers.None, slotB = Powers.None;
    bool slot_A, slot_B;

    public RectTransform touchArea;
    public GameObject shieldObj;
    public GameObject boostEffect;
    public GameObject hitEffect;

    private void Start()
    {
        // PowerButtonInteractbale(false);
        UpdateIcon(slotA, powerUpIconA);
        UpdateIcon(slotB, powerUpIconB);
    }



    public void PowerSlot(Powers pu)
    {
        if (!slot_A)
        {
            slotA = pu;
            slot_A = true;
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAA");
        }
        else
        {
            slotB = pu;
            slot_B = true;
            Debug.Log("BBBBBBBBBBBBBBBBBBBBBBBBB");
        }
        UpdateIcon(slotA, powerUpIconA);
        UpdateIcon(slotB, powerUpIconB);

    }
    public void SwitchSlots()
    {
        if (slot_B)
        {
            slotA = slotB;
            slotB = Powers.None;
            UpdateIcon(slotA, powerUpIconA);
            UpdateIcon(slotB, powerUpIconB);
            slot_B = false;
        }
        else
        {
           
            slotA = Powers.None;
            UpdateIcon(slotA, powerUpIconA);
            
            slot_A = false;
        }
    }


    public void UpdateIcon(Powers pu, Image powerUpIcon)
    {
        if (pu == Powers.None)
        {
            powerUpIcon.gameObject.transform.parent.gameObject.SetActive(false);
            return;
        }

        powerUp = slotA;
        powerUpIcon.gameObject.transform.parent.gameObject.SetActive(true);
        switch (pu)
        {
            case Powers.Slash:
                powerUpIcon.sprite = slash;
                break;
            case Powers.Boost:
                powerUpIcon.sprite = boost;
                break;
            case Powers.HomingSlash:
                powerUpIcon.sprite = homing;
                break;
            case Powers.Shield:
                powerUpIcon.sprite = shield;
                break;
            default:
                Debug.LogWarning("Unknown power-up type: " + pu);
                powerUpIcon.gameObject.transform.parent.gameObject.SetActive(false);
                break;
        }
    }

    public void UpdateIcon(Powers pu)
    {
        powerUp = pu;
        switch (powerUp)
        {
            case Powers.Slash:
                powerUpIconA.sprite = slash;
                break;
            case Powers.Boost:
                powerUpIconA.sprite = boost;
                break;
            case Powers.HomingSlash:
                powerUpIconA.sprite = homing;
                break;
            case Powers.Shield:
                powerUpIconA.sprite = shield;
                break;
            case Powers.None:
                powerUpIconA.sprite = null;
                break;
            default:
                Debug.LogWarning("Unknown power-up type: " + powerUp);
                break;
        }

    }

    public void PowerButtonInteractbale(bool b)
    {
      //  powerUpBtn.interactable = b;
       // powerUpBtn.transform.GetChild(1).gameObject.SetActive(!b);
    }


}


public enum Powers
{
    None,
    Boost,
    Slash,
    HomingSlash,
    Shield
}