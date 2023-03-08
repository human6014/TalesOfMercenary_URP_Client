using PlayFab;
using PlayFab.ClientModels;
using PlayfabTutorial.Scripts.PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FMGames.Playfab.Login {
    /// <summary>
    /// Handle login scene events
    /// </summary>
    public class PlayfabLogin : MonoBehaviour {
        [SerializeField] private LoginUi loginUi;
        [SerializeField] private RegisterUi registerUi;

        [SerializeField] private GameObject loginInProgress;

        public void Start() {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;

            //Filling login credentials based on local saves
            loginUi.username.text = PlayerPrefs.GetString(PlayFabConstants.SavedUsername, "");

            if (PlayFabClientAPI.IsClientLoggedIn()) {
                Debug.LogWarning("User is already logged in");
            }
        }

        private void Login(ILogin loginMethod, object loginParams) {
            loginMethod.Login(_loginInfoParams, OnLoginSuccess, OnLoginFailure, loginParams);

            loginInProgress.SetActive(true);
        }

        #region Steam Login

        public void LoginWithSteam() {
            Login(new SteamLogin(), null);
        }

        #endregion

        #region Email Login

        public void LoginWithEmail() {
            if (ValidateLoginData()) {
                Login(new EmailLogin(), new EmailLogin.EmailLoginParams(loginUi.username.text, loginUi.password.text));
            }
        }

        private bool ValidateLoginData() {
            //Validating data
            string errorMessage = "";

            if (loginUi.username.text.Length < 5) {
                errorMessage = "Username must be at least 5 characters";
            } else if (loginUi.password.text.Length < 8) {
                errorMessage = "Password must be at least 8 characters";
            }

            if (errorMessage.Length > 0) {
                Debug.LogError(errorMessage);
                return false;
            }

            return true;
        }

        #endregion

        #region Email Register

        public void Register() {
            if (!ValidateRegisterData()) return;

            var request = new RegisterPlayFabUserRequest {
                TitleId = "9C8D9",//PlayFabConstants.TitleID,
                Email = registerUi.email.text,
                Password = registerUi.password.text,
                Username = registerUi.username.text,
                InfoRequestParameters = _loginInfoParams,
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnLoginFailure);

            loginInProgress.SetActive(true);
        }

        bool ValidateRegisterData() {
            //Validating data
            string errorMessage = "";

            if (!registerUi.email.text.Contains("@")) {
                errorMessage = "E-mail is not valid";
            } else if (registerUi.email.text.Length < 5) {
                errorMessage = "E-mail is not valid";
            } else if (registerUi.username.text.Length < 5) {
                errorMessage = "Username must be at least 5 characters";
            } else if (registerUi.password.text.Length < 8) {
                errorMessage = "Password must be at least 8 characters";
            } else if (!registerUi.password.text.Equals(registerUi.verifyPassword.text)) {
                errorMessage = "Password doesn't match Repeat password";
            }

            if (errorMessage.Length > 0) {
                Debug.LogError(errorMessage);
                return false;
            }

            return true;
        }

        private void OnRegisterSuccess(RegisterPlayFabUserResult result) {
            Debug.Log("Register Success!");

            PlayerPrefs.SetString("USERNAME", registerUi.username.text);
            PlayerPrefs.SetString("PW", registerUi.password.text);

            Debug.Log(result.PlayFabId);
            Debug.Log(result.Username);
            loginInProgress.SetActive(false);
        }

        #endregion

        #region GuestLogin

        public void GuestLogin() {
            Login(new GuestLogin(), new GuestLogin.GuestLoginParameters(PlayerPrefs.GetString("GUEST_ID")));
        }
        
        #endregion

        private void OnLoginSuccess(LoginResult result) {
            Debug.Log("Login Success!");
            Debug.Log(result.PlayFabId);

            PlayerPrefs.SetString(PlayFabConstants.SavedUsername, loginUi.username.text);

            loginInProgress.SetActive(false);

            SceneManager.LoadScene("LobbyScene");
        }

        private readonly GetPlayerCombinedInfoRequestParams _loginInfoParams =
            new GetPlayerCombinedInfoRequestParams {
                GetUserAccountInfo = true,
                GetUserData = true,
                GetUserInventory = true,
                GetUserVirtualCurrency = true,
                GetUserReadOnlyData = true
            };

        private void OnLoginFailure(PlayFabError error) {
            Debug.LogError("Login failure: " + error.Error + "  " + error.ErrorDetails + error + "  " +
                           error.ApiEndpoint + "  " + error.ErrorMessage);
            loginInProgress.SetActive(false);
        }

        public void Exit() {
            Application.Quit();
        }
    }
}