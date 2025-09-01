using System.Collections;
using UnityEngine;
using Fusion;
using PlayFab;
using PlayFab.ClientModels;
using Fusion.Photon.Realtime;


public class FusionAuthCheck : MonoBehaviour
{
    public NetworkRunner runnerPrefab; // Assign this in the Inspector
    private NetworkRunner _runner;
    public FusionLauncher fl;
    
    public static AuthenticationValues AuthValues { get; private set; }
    private void Starts()
    {

        StartCoroutine(AuthenticateAndStartFusion());
    }
    public void Test()
    {
        Starts();
    }

    private static string fusionToken = "";
    private bool tokenReceived = false;

    IEnumerator  AuthenticateAndStartFusion()
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {

            // Step 1: Get Photon Fusion Authentication Token from PlayFab
            PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest
            {
                PhotonApplicationId = "7d99792b-5c73-4305-9198-6184e18591d1" // Use your App ID
            }, AuthenticateWithPhoton, OnPlayFabError);


            // Wait until the AuthValues are actually set (important to avoid race condition)
            yield return new WaitUntil(() => AuthValues != null);

            fl.StartGame(AuthValues);
        }
        else
        {
            errorMsg.SetActive(true);
            errorMsg.GetComponent<ErrorMsg>().SetErrorMsg("Login To Proceed");
            authLoad.SetActive(false);
        }
    }



    public GameObject errorMsg,authLoad;

    private  void OnPlayFabError(PlayFabError obj)
    {
        Debug.Log(obj.GenerateErrorReport());
        errorMsg.SetActive(true);
        errorMsg.GetComponent<ErrorMsg>().SetErrorMsg("Unable to Authenticate");
        authLoad.SetActive(false);


    }

    public static void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult authenticationTokenResult)
    {
        Debug.Log("Photon token acquired: " + authenticationTokenResult.PhotonCustomAuthenticationToken + "  Authentication complete.");

        //We set AuthType to custom, meaning we bring our own, PlayFab authentication procedure.
        var customAuth = new AuthenticationValues() { AuthType = CustomAuthenticationType.Custom };

        //We add "username" parameter. Do not let it confuse you: PlayFab is expecting this parameter to contain player PlayFab ID (!) and not username.
        customAuth.AddAuthParameter("username", PlayerPrefs.GetString("Session","Player")); // expected by PlayFab custom auth service

        //We add "token" parameter. PlayFab expects it to contain Photon Authentication Token issues to your during previous step.
        customAuth.AddAuthParameter("token", authenticationTokenResult.PhotonCustomAuthenticationToken);

        //We finally store to use this authentication parameters throughout the entire application.
        AuthValues = customAuth;

       
    }


    
}



