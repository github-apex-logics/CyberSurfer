using UnityEngine;
using UnityEngine.UI;

public class HUD_Selector : MonoBehaviour
{
     Image image;
    public Sprite[] sprites;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
        image.sprite = sprites[Database.SelectedCharacter];
        
    }

   
}
