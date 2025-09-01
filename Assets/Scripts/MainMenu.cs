using LightDI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour, ISystem
{

    public TextMeshProUGUI[] coinTxt;

    private void Awake()
    {
        InjectionManager.RegisterSystem(this);
    }

    private void OnEnable()
    {
       UpdateCoinTxt();
    }


    void UpdateCoinTxt()
    {
        foreach (TextMeshProUGUI coin in coinTxt)
        {
            coin.text = Database.Coins.ToString();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void LoadScene(int num)
    {
        StartCoroutine(PlayScene(num));
    }

    IEnumerator PlayScene(int num)
    {
        yield return new WaitForSeconds(1);
        InjectionManager.ResetInjection();
        SceneManager.LoadSceneAsync(num);
    }
}
