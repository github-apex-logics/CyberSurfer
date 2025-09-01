
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIEffects;
using LightDI;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;


public class LevelButton : MonoBehaviour, IInjectable
{
    public int levelIndex;
    public Image image;
    public Button btn;
    public Sprite lockedFrame,unlockedFrame;
    public GameObject lockIcon;
    public UIEffect uiEffect;
    bool unlocked;
    [Inject] private MainMenu menu;
    void OnEnable()
    {

       
        unlocked = IsLevelUnlocked(levelIndex);
      
        lockIcon.SetActive(!unlocked);
        if (unlocked)
        {
            uiEffect.effectMode = EffectMode.None;
            image.sprite = unlockedFrame;
            btn.interactable = true;
        }
        else
        {
            uiEffect.effectMode = EffectMode.Grayscale;
            image.sprite = lockedFrame;
            btn.interactable = false;
        }
        StartCoroutine(InjectionDelay());
    }



    IEnumerator InjectionDelay()
    {
        yield return new WaitForEndOfFrame();
        InjectionManager.RegisterObject(this);


    }

    public void LoadLevel()
    {
        if (IsLevelUnlocked(levelIndex) && unlocked)
        {
            //SceneManager.LoadScene( levelIndex);
            menu.LoadScene( levelIndex);
            PlayerPrefs.SetInt("CurrentLevel", levelIndex);
        }
    }


    public bool IsLevelUnlocked(int levelIndex)
    {
      //  return PlayerPrefs.GetInt("LevelUnlocked_" + levelIndex, levelIndex == 1 ? 1 : 0) == 1;
        return Database.LevelUnlock(levelIndex) == 1 || (levelIndex == 1); 
    }


    public bool IsLevelCompleted(int levelIndex)
    {
        //return PlayerPrefs.GetInt("LevelCompleted_" + levelIndex, 0) == 1;

        return Database.LevelComplete(levelIndex) == 1;
        

    }


    public void CompleteLevel(int levelIndex)
    {
        if (!IsLevelCompleted(levelIndex))
        {
            Database.LevelComplete(levelIndex, 1);
            Database.LevelUnlock((levelIndex + 1), 1);
          
            //PlayerPrefs.SetInt("LevelCompleted_" + levelIndex, 1);
            //PlayerPrefs.SetInt("LevelUnlocked_" + (levelIndex + 1), 1); // Unlock next level
        }
    }

    public void PostInject()
    {
        //throw new System.NotImplementedException();
    }
}
