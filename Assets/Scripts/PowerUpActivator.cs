using UnityEngine;

public class PowerUpActivator : MonoBehaviour
{
    private void OnEnable()
    {
        Activate();
    }

    void Activate()
    {
        int randomIndex = Random.Range(0, transform.childCount);

        for (int i = 0; i < transform.childCount; i++)
        {
            bool shouldEnable = (i == randomIndex);
            transform.GetChild(i).gameObject.SetActive(shouldEnable);
            //transform.GetChild(i).gameObject.GetComponent<Collider>().enabled = shouldEnable;
        }
    }
}
