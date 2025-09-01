using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] tutorialSteps; // Assign tutorial panels in order
    private int currentStep = 0;

    public string TutorialKey;
    public float timeDelay;

    public GameObject msgPanel;

    // Optional: conditions for certain steps
    public enum StepCondition { None, RequireLogin, RequireAction }
    public StepCondition[] stepConditions;

    private bool isLoginDone = false; // Example condition flag

    void Start()
    {
        
        StartCoroutine(StartAfterDelay());
    }

    IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(timeDelay);
        if (!IsTutorialCompleted())
        {
            if(msgPanel)
                msgPanel.SetActive(true);
            ShowStep(0);
            Time.timeScale = 0f; // Pause game during tutorial
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void NextStep()
    {
        // Check condition before moving forward
        if (!CanProceed(currentStep)) return;

        tutorialSteps[currentStep].SetActive(false);
        currentStep++;

        if (currentStep < tutorialSteps.Length)
        {
            ShowStep(currentStep);
        }
        else
        {
            CompleteTutorial();
        }
    }

    public void TimeScaleSet(bool b)
    {
        if (b)
            Time.timeScale = 1;
        else
            Time.timeScale = 0;
    }
    private void ShowStep(int index)
    {
        tutorialSteps[index].SetActive(true);
    }

    private void CompleteTutorial()
    {
        PlayerPrefs.SetInt(TutorialKey, 1);
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    public bool IsTutorialCompleted()
    {
        return PlayerPrefs.GetInt(TutorialKey, 0) == 1;
    }

    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey(TutorialKey);
    }

    // --------------------------
    //  CONDITION CHECKER
    // --------------------------
    private bool CanProceed(int stepIndex)
    {
        if (stepConditions == null || stepIndex >= stepConditions.Length)
            return true;

        switch (stepConditions[stepIndex])
        {
            case StepCondition.None:
                return true;

            case StepCondition.RequireLogin:
                return isLoginDone; // only proceed if login is done

            case StepCondition.RequireAction:
                // Example placeholder (could be pickup, click, etc.)
                return false;

            default:
                return true;
        }
    }

    //  Call this from your Login script when player logs in
    public void OnPlayerLoggedIn()
    {
        isLoginDone = true;
        Debug.Log("Tutorial: Login completed ");
    }
}
