using LightDI;
using TMPro;
using UnityEngine;
using System.Collections;

public class CountDownText : MonoBehaviour, IInjectable
{
    public int i = 3;
    public TextMeshProUGUI countTxt;
    [Inject] LevelManager levelManager;


    private void Start()
    {
        StartCoroutine(InjectionDelay());
    }



    IEnumerator InjectionDelay()
    {
        yield return new WaitForEndOfFrame();
        InjectionManager.RegisterObject(this);


    }


    private void OnEnable()
    {
        i = 3;   
    }

    // Update is called once per frame
    public void UpdateText()
    {
        if (i > 0)
            countTxt.text = i.ToString();
        else if (i == 0)
            countTxt.text = "Go!";
        else
        {
            this.transform.parent.gameObject.SetActive(false);
            levelManager.TapToGo();
        }
        
        i--;
    }

    public void PostInject()
    {
       
    }
}
