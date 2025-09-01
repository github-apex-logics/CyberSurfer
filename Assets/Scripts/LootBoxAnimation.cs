using UnityEngine;

public class LootBoxAnimation : MonoBehaviour
{
    Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        anim= GetComponent<Animator>();
        anim.SetBool("Rotate", true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
