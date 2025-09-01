using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class CoolDown_Power : MonoBehaviour
{

    public Image lockImage;
    public float coolDownTime;
    public Button powerButton;

    private void Start()
    {
        lockImage.fillAmount = 0;
        powerButton = GetComponent<Button>();   
    }


    public void CoolDown()
    {
        StartCoroutine(StartCoolDown());
    }

    IEnumerator StartCoolDown()
    {
        powerButton.interactable = false;
        float num = 1;
        lockImage.fillAmount = num;
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(coolDownTime);
            num -= 0.01f;
            lockImage.fillAmount = num;
        }
        powerButton.interactable = true;
    }

}
