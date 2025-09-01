using UnityEngine;

public class RewardRawIUI : MonoBehaviour
{

    

    public GameObject lootboxCasual, lootboxRare, keySpecial, keyRare;
  public  RewardType type;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {

        switch (type)
        {
            case RewardType.Casual:
                {
                    lootboxCasual.SetActive(true);
                }
                break;
            case RewardType.Rare:
                {
                    lootboxRare.SetActive(true);
                }
                break;
            case RewardType.CasualKey:
                {
                    keySpecial.SetActive(true);
                }
                break;
            case RewardType.RareKey:
                {
                    keyRare.SetActive(true);
                }
                break;

        }
        SoundManager.instance.PlayClip(ClipName.Reward);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
