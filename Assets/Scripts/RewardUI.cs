using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RewardUI : MonoBehaviour
{
    public GameObject rewardPanel;
    public Image rewardIcon;
    public TMP_Text rewardText;

    public void ShowReward(RewardItem reward)
    {
        rewardPanel.SetActive(true);
        rewardIcon.sprite = reward.icon;
        rewardText.text = $"You got {reward.amount}x {reward.rewardName}!";
    }

    public void Close()
    {
        rewardPanel.SetActive(false);
    }
}
