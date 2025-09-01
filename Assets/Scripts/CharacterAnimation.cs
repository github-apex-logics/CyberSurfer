using LightDI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CharacterAnimation : MonoBehaviour, IInjectable
{

    public Animator animator;
    public Animator knifeAnim;
    public Cyber_Controller controller;
    bool go=false;
    AudioSource src;
    public AudioClip footStep1, footStep2;
    [Inject] private LevelManager levelManager;

    private void Awake()
    {
       // InjectionManager.RegisterObject(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        src = GetComponent<AudioSource>();
        // controller = GetComponent<Cyber_Controller>();    
        StartCoroutine(delay());
        StartCoroutine(InjectionDelay());
    }



    IEnumerator InjectionDelay()
    {
        yield return new WaitForEndOfFrame();
        InjectionManager.RegisterObject(this);


    }


    IEnumerator delay()
    {
        yield return new WaitForSeconds(7f);
        go = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded)
        {
            animator.SetBool("jump", true);
            animator.SetBool("surf", false);

            //AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
            //Replay the current animation state from the beginning
            //animator.Play(currentState.fullPathHash, -1, 0f);
            //animator.Update(0f);
            

        }
        else
        {
           // Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa " + controller.isGrounded);
            animator.SetBool("jump", false);
        }

        if (controller.onRamp)
        {
            animator.SetBool("surf", true);
            animator.SetBool("jump", false);
           

           
        }
        //else if (go)
        //{
        //    animator.SetBool("surf", false);
        //    animator.SetBool("jump", true);
        //}

        if (controller.onRampEdge)
        {
           // animator.SetBool("jump", true);
            animator.SetBool("surf", true);


        }

        if (controller.slashAnim)
        {
            animator.SetBool("slash", true);
            StartCoroutine(DelayOff(controller.slashAnim, 0.02f));
            Invoke(nameof(SlashOff), 1f);
        }
        else
        {
           // animator.SetBool("slash", false);
        }
       
    }

    public void SlashOff()
    {
        animator.SetBool("slash", false);
    }


    IEnumerator DelayOff(bool b, float t)
    {
        yield return new WaitForSeconds(t);
        b = !b;
    }

    public void JumpOff()
    {
        animator.SetBool("jump", false);
    }

    public void CallJump()
    {
       // controller.Jump();
    }

    public void Steps(int num)
    {
        if (levelManager.startGame)
        {
            if (num == 0)
                src.PlayOneShot(footStep1);
            else
                src.PlayOneShot(footStep2);
        }
    }



    public void Jump()
    {
        StartCoroutine(JumpSound());
        
    }

    IEnumerator JumpSound()
    {
        src.PlayOneShot(footStep1);


        yield return new WaitForSeconds(0.1f);

        src.PlayOneShot(footStep2);
    }


    public void KnifeRotate(int b)
    {
        if (b==1)
        {
            knifeAnim.enabled = true;
            knifeAnim.Play("KarambitAnimation");
            knifeAnim.SetTrigger("go");
        }
        else
        {
            knifeAnim.enabled = false;
            knifeAnim.ResetTrigger("go");
        }

    }

   
    IEnumerator RotateKnife()
    {
        GameObject kinfeObj = knifeAnim.gameObject;

        for (int i = 0; i < 360; i++)
        {
            kinfeObj.transform.localRotation = new Quaternion(i, 97f, 104f,1);
            yield return new WaitForSeconds(0.1f);

        }

        kinfeObj.transform.localRotation = new Quaternion(0, 97f, 140f, 1);
    }




    public void KnifeAnim()
    {
        int i = Random.Range(0, 2);
        animator.SetInteger("Knife", i);
        Debug.Log("setting up knife anim: " + i);
    }

    public void PostInject()
    {
        
    }
}
