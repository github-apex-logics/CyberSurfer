using System.Collections;
using UnityEngine;

public class HomingSlash : MonoBehaviour
{
    public Transform target;         // Assign the enemy/player target
    public Transform dummyTarget;
    public float speed = 10f;        // How fast the slash moves
    public float rotateSpeed = 200f; // How sharply it turns
    public float destroyTime;
    public Vector3 pos;
    LevelManager levelManager;

    private void OnEnable()
    {
        levelManager = FindAnyObjectByType<LevelManager>(); 
        target =   levelManager.dummyTarget;
       // dummyTarget = FindClosestEnemyPlayer();
        //StartCoroutine(SwitchToRealTarget());
    }


    IEnumerator SwitchToRealTarget()
    {
        yield return new WaitForSeconds(0.5f);
        if (dummyTarget != null)
            target = dummyTarget;
    }

    void Update()
    {
        if (!target)
        {
            Destroy(gameObject); // Optionally destroy if there's no target
            return;
        }




        // Direction to the target
        Vector3 direction = target.position - this.transform.position;
        direction.Normalize();

        float dist = Vector3.Distance(target.position, transform.position);
        //// Calculate rotation step
        //Vector3 rotateAmount = Vector3.Cross(-transform.forward, direction);
        //transform.Rotate(rotateAmount * rotateSpeed * Time.deltaTime);
        // if (dist < 0.01f)
        transform.LookAt(target);
        // Move forward
        transform.position += transform.forward * speed * Time.deltaTime;

    }



    public Transform FindClosestEnemyPlayer()
    {
        
        Transform closest = null;
        float minDist = Mathf.Infinity;
        Vector3 myPos = transform.position;

        foreach (GameObject p in levelManager.Enemies)
        {
           // PhotonView pv = p.GetComponent<PhotonView>();
           // if (pv != null && !pv.IsMine) // Make sure it's not yourself
            {
                float dist = Vector3.Distance(myPos, p.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = p.transform;
                }
            }
        }

        return closest;
    }


}



