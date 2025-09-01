using UnityEngine;
using Fusion;
using System.Collections;

public class CharacterNetworkAnimation : NetworkBehaviour
{
    public Animator animator;
    public Animator knifeAnim;
    public CyberNetworkController controller;
    public AudioClip footStep1, footStep2;

    private NetworkedManager networkManager;
    private AudioSource src;


    private static readonly int JumpHash = Animator.StringToHash("jump");
    private static readonly int SurfHash = Animator.StringToHash("surf");
    private static readonly int SlashHash = Animator.StringToHash("slash");
    private static readonly int KnifeHash = Animator.StringToHash("Knife");

    public override void Spawned()
    {
        src = GetComponent<AudioSource>();
        animator.SetBool(JumpHash, true);

        networkManager = FindAnyObjectByType<NetworkedManager>();

       
    }

  
    public override void FixedUpdateNetwork()
    {
        // Apply animation updates for all players using synced data
        if (controller == null) return;

        animator.SetBool(JumpHash, controller.isGrounded);
        animator.SetBool(SurfHash, controller.onRamp || controller.onRampEdge);
        animator.SetBool(SlashHash, controller.slashAnim);

        if (controller.slashAnim)
        {
            StartCoroutine(ResetSlashAnim());
        }
    }

    private IEnumerator ResetSlashAnim()
    {
        yield return new WaitForSeconds(0.02f);
        if (controller != null)
            controller.slashAnim = false;
    }

    // Called via animation event
    public void Steps(int num)
    {
        if (networkManager != null && networkManager.startGame)
        {
            if (num == 0) src.PlayOneShot(footStep1);
            else src.PlayOneShot(footStep2);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayFootstep(int num)
    {
        if (num == 0)
            src.PlayOneShot(footStep1);
        else
            src.PlayOneShot(footStep2);
    }

    public void Jump()
    {
        StartCoroutine(PlayJumpSound());
    }

    private IEnumerator PlayJumpSound()
    {
        src.PlayOneShot(footStep1);
        yield return new WaitForSeconds(0.1f);
        src.PlayOneShot(footStep2);
    }

    public void KnifeRotate(int state)
    {
        if (HasStateAuthority)
        {
            RPC_PlayKnifeAnim(state);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayKnifeAnim(int state)
    {
        if (state == 1)
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

    public void KnifeAnim()
    {
        if (HasStateAuthority)
        {
            int i = Random.Range(0, 2);
            RPC_KnifeAnimState(i);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_KnifeAnimState(int state)
    {
        animator.SetInteger(KnifeHash, state);
    }

    public void JumpOff()
    {
        animator.SetBool(JumpHash, false);
    }
}
