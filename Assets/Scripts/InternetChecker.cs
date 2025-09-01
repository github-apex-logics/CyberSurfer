using UnityEngine;

public class InternetChecker : MonoBehaviour
{
    
    public static bool IsConnectedToInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("No internet connection.");
            return false;
        }
        else
        {
            Debug.Log("Internet connection available.");
            return true;
        }
    }


}
