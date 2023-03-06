using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace FMGames.Playfab.Login {
    public class GoogleLogin : ILogin {
        public void Login(GetPlayerCombinedInfoRequestParams loginInfoParams, Action<LoginResult> loginSuccess,
            Action<PlayFabError> loginFailure, object loginParams) {
#if UNITY_ANDROID
            // The following grants profile access to the Google Play Games SDK.
            // Note: If you also want to capture the player's Google email, be sure to add
            // .RequestEmail() to the PlayGamesClientConfiguration
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                .AddOauthScope("profile")
                .RequestServerAuthCode(false)
                .Build();
            PlayGamesPlatform.InitializeInstance(config);

            // recommended for debugging:
            PlayGamesPlatform.DebugLogEnabled = true;

            // Activate the Google Play Games platform
            PlayGamesPlatform.Activate();


            Social.localUser.Authenticate((bool success) => {

                if (success) {

                    var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                    Debug.Log("Server Auth Code: " + serverAuthCode);

                    PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest() {
                        TitleId = PlayFabSettings.TitleId,
                        ServerAuthCode = serverAuthCode,
                        CreateAccount = true
                    }, OnLoginSuccess, OnLoginFailure);
                } else {
                    Debug.Log("Failed to log in");
                }

            });
#endif
        }
    }
}