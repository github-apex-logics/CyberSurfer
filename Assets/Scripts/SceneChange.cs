
using LightDI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void ChangeScene(int i)
    {
        InjectionManager.ResetInjection();
        SceneManager.LoadScene(i);
    }

    public void ReloadScene()
    {
        InjectionManager.ResetInjection();
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
