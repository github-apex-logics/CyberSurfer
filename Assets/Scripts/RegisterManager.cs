
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
//using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;



public class RegisterManager : MonoBehaviour
{

    [HideInInspector] public string UserName;

    public string TitleId;


    public TMP_Text RegisterMessageText;
    public TMP_InputField RegisterUsernameField;
    public TMP_InputField RegisterEmailField;
    public TMP_InputField RegisterPasswordField;


    public TMP_Text LoginMessageText;
    public TMP_InputField LoginEmailField;
    public TMP_InputField LoginPasswordField;


    public TMP_Text PasswordResetMessageText;
    public TMP_InputField ResetEmailField;

    public GameObject RegistrationPanel;
    public GameObject LoginPanel;
    public GameObject noInternetMessage;
    public GameObject invalidMessage;
    public GameObject RegisterErrorMessage;
    public GameObject loginBtn;
    public GameObject profilePanel;
    public TextMeshProUGUI profileName;
    public Button RegisterButtonUI;
    public GameObject LoadingObj;

    public TutorialManager tutorialManager;

    PlayFabSyncManager syncManager;

    private void Awake()
    {
        syncManager = FindAnyObjectByType<PlayFabSyncManager>();
    }

    private void Start()
    {
        if (InternetChecker.IsConnectedToInternet())
        {
            if (PlayerPrefs.GetInt("NewLogin") == 1)
            {
                //Auto Login
                var request = new LoginWithEmailAddressRequest
                {

                    Email = PlayerPrefs.GetString("Email"),
                    Password = PlayerPrefs.GetString("Password"),
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                    {
                        GetPlayerProfile = true
                    }
                };
                PlayFabClientAPI.LoginWithEmailAddress(request, onAutoLoginSuccess, OnAtuoErrorLogin);
                Debug.Log("AutoLogin");
            }
        }
        else
        {
            noInternetMessage.SetActive(true);
        }
        OnInputChanged();

    }


  


    //-----------------------------AutoLogin------------------------------------------
   
    void onAutoLoginSuccess(LoginResult result)
    {


        PlayerPrefs.SetString("Session", result.PlayFabId);


        {
            UserName = result.InfoResultPayload.PlayerProfile.DisplayName;
        }
        loginBtn.SetActive(false);
        profilePanel.SetActive(true);
        profileName.text = UserName;

        PlayerPrefs.SetString("UserName", UserName);

        //clear text

        LoginEmailField.text = "";
        LoginPasswordField.text = "";

        LoginPanel.SetActive(false);
        RegistrationPanel.SetActive(false);
        Debug.Log("yes " + PlayerPrefs.GetString("Session"));
        syncManager.LoadLocalData();
        //tutorialManager.OnPlayerLoggedIn();
        //tutorialManager.NextStep();
    }


    void OnAtuoErrorLogin(PlayFabError error)
    {
        // LoginMessageText.text = error.ErrorMessage;
        invalidMessage.SetActive(true);
        Debug.Log(error);
    }




    //--------------------------------Registeration--------------------------------------

    // checks on value change wheather fields are filled correctly or not
    public void OnInputChanged()
    {
        bool isUsernameValid = !string.IsNullOrWhiteSpace(RegisterUsernameField.text);
        bool isEmailValid = IsValidEmail(RegisterEmailField.text);
        bool isPasswordValid = !string.IsNullOrWhiteSpace(RegisterPasswordField.text);

        RegisterButtonUI.interactable = isUsernameValid && isEmailValid && isPasswordValid;
    }

    // validating the email field;
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }


    public void RegisterButton()
    {
        if (InternetChecker.IsConnectedToInternet())
        {
            if (RegisterPasswordField.text.Length < 6)
            {
                ErrorMessage("Password Too Short", RegisterErrorMessage);
                
                return;
            }
            var request = new RegisterPlayFabUserRequest
            {
                DisplayName = RegisterUsernameField.text,
                Username = RegisterUsernameField.text,
                Email = RegisterEmailField.text,
                Password = RegisterPasswordField.text,
                RequireBothUsernameAndEmail = false
            };
            PlayFabClientAPI.RegisterPlayFabUser(request, onRegisterSuccess, OnError);
        }
        else
        {
            noInternetMessage.SetActive(true);
        }
    }

    void onRegisterSuccess(RegisterPlayFabUserResult result)
    {
        //RegisterMessageText.text = "New Account created";
        RegisterUsernameField.text = "";
        RegisterEmailField.text = "";
        RegisterPasswordField.text = "";
        RegistrationPanel.GetComponent<PanelClose>().ClosePanel();
        Invoke(nameof(EnableLoginPanel),0.25f);
        
        LoginEmailField.text = "";
        LoginPasswordField.text = "";

    }
    void EnableLoginPanel()
    {
        LoginPanel.SetActive(true);
    }


    private void OnError(PlayFabError error)
    {
        RegisterButtonUI.interactable = true;

        Debug.LogError("Registration Failed: " + error.GenerateErrorReport());

        switch (error.Error)
        {
            case PlayFabErrorCode.InvalidEmailAddress:
                ErrorMessage("Invalid Email Address", RegisterErrorMessage);
                break;

            case PlayFabErrorCode.EmailAddressNotAvailable:
                ErrorMessage("Email Address Already Registered", RegisterErrorMessage);
                break;
            case PlayFabErrorCode.AccountAlreadyExists:
                ErrorMessage("Account Already Exist", RegisterErrorMessage);
                break;
           

            default:
                ErrorMessage(error.ErrorMessage, RegisterErrorMessage);
                break;
        }
    }

    //-------------------------------------Login---------------------------

    void ErrorMessage(string s , GameObject obj)
    {
        obj.GetComponent<TextMeshProUGUI>().text = s;
        obj.SetActive(true);   
    }

    public void ClearFields()
    {
        RegisterUsernameField.text = "";
        RegisterEmailField.text = "";
        RegisterPasswordField.text = "";

        LoginEmailField.text = "";
        LoginPasswordField.text = "";
    }




    public void LoginButton()
    {
        if (InternetChecker.IsConnectedToInternet())
        {
            var request = new LoginWithEmailAddressRequest
            {
                Email = LoginEmailField.text,
                Password = LoginPasswordField.text,
                TitleId = TitleId,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true
                }
            };
            LoadingObj.SetActive(true);
            PlayFabClientAPI.LoginWithEmailAddress(request, onLoginSuccess, OnErrorLogin);
        }
        else
        {
            noInternetMessage.SetActive(true);
        }
    }


    void onLoginSuccess(LoginResult result)
    {
        PlayerPrefs.SetString("Session", result.PlayFabId);
        PlayerPrefs.SetString("Email", LoginEmailField.text);
        PlayerPrefs.SetString("Password", LoginPasswordField.text);
        PlayerPrefs.SetInt("NewLogin",1);

        LoadingObj.SetActive(false);
        UserName = result.InfoResultPayload.PlayerProfile.DisplayName;
        
        loginBtn.SetActive(false);
        profilePanel.SetActive(true);
        profileName.text = UserName;
        PlayerPrefs.SetString("UserName", UserName);

        //clear text
        LoginEmailField.text = "";
        LoginPasswordField.text = "";

        LoginPanel.SetActive(false);
        RegistrationPanel.SetActive(false);

        tutorialManager.OnPlayerLoggedIn();
        tutorialManager.NextStep();
        syncManager.LoadLocalData();
        Debug.Log("yes");
    }


    public void PasswordVisiblitiy(bool b)
    {
        if (b)
        {
            LoginPasswordField.contentType = TMP_InputField.ContentType.Standard;
            RegisterPasswordField.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            LoginPasswordField.contentType = TMP_InputField.ContentType.Password;
            RegisterPasswordField.contentType = TMP_InputField.ContentType.Password;
        }

        LoginPasswordField.ForceLabelUpdate();
    }
    

    void OnErrorLogin(PlayFabError error)
    {
       // LoginMessageText.text = error.ErrorMessage;
        invalidMessage.SetActive(true);
        LoadingObj.SetActive(false);
        Debug.Log(error);
    }

    //-----------------------------Guest Login----------------------------
    public void GuestLoginButton()
    {
        if (InternetChecker.IsConnectedToInternet())
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = GetOrCreateGuestId(),
                CreateAccount = true,
                TitleId = TitleId,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true
                }
            };
            LoadingObj.SetActive(true);
            PlayFabClientAPI.LoginWithCustomID(request, onGuestLoginSuccess, OnErrorLogin);

        }
        else
        {
            noInternetMessage.SetActive(true);
        }
    }

    private string GetOrCreateGuestId()
    {
        const string GUEST_ID_KEY = "PLAYFAB_GUEST_ID";

        if (PlayerPrefs.HasKey(GUEST_ID_KEY))
        {
            return PlayerPrefs.GetString(GUEST_ID_KEY);
        }

        string newGuestId = SystemInfo.deviceUniqueIdentifier;

        // Fallback (very rare, but safe)
        if (string.IsNullOrEmpty(newGuestId))
            newGuestId = System.Guid.NewGuid().ToString();

        PlayerPrefs.SetString(GUEST_ID_KEY, newGuestId);
        PlayerPrefs.Save();

        return newGuestId;
    }


    void onGuestLoginSuccess(LoginResult result)
    {
        PlayerPrefs.SetString("Session", result.PlayFabId);
        PlayerPrefs.SetInt("NewLogin", 1);

        // Email login only
        if (!string.IsNullOrEmpty(LoginEmailField.text))
        {
            PlayerPrefs.SetString("Email", LoginEmailField.text);
            PlayerPrefs.SetString("Password", LoginPasswordField.text);
        }

        // Handle Display Name safely
        UserName = result.InfoResultPayload?.PlayerProfile?.DisplayName;

        if (string.IsNullOrEmpty(UserName))
        {
            UserName = "Guest_" + result.PlayFabId.Substring(0, 6);
        }

        PlayerPrefs.SetString("UserName", UserName);

        loginBtn.SetActive(false);
        profilePanel.SetActive(true);
        profileName.text = UserName;

        // Clear fields
        LoginEmailField.text = "";
        LoginPasswordField.text = "";

        LoginPanel.SetActive(false);
        RegistrationPanel.SetActive(false);

        tutorialManager.OnPlayerLoggedIn();
        tutorialManager.NextStep();
        syncManager.LoadLocalData();

        Debug.Log("Login Success");
    }




    //----------------------------Password Recovery--------------------------


    public void ResetPasswordButton()
    {
        if (InternetChecker.IsConnectedToInternet())
        {
            var request = new SendAccountRecoveryEmailRequest
            {
                Email = ResetEmailField.text,
                TitleId = TitleId
            };
            PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordResetSuccess, OnErrorPasswordReset);
        }
        else
        {

            noInternetMessage.SetActive(true);
        }
    }


    void OnPasswordResetSuccess(SendAccountRecoveryEmailResult result)
    {
        PasswordResetMessageText.text = "Password Reset Mail Sent Successfully!";
        PasswordResetMessageText.gameObject.SetActive(true);
    }


    void OnErrorPasswordReset(PlayFabError error)
    {
        PasswordResetMessageText.text = error.ErrorMessage;
        PasswordResetMessageText.gameObject.SetActive(true);
    }

}
