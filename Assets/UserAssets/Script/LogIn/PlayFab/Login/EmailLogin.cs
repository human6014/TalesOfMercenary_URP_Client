using System;
using PlayFab;
using PlayFab.ClientModels;
using PlayfabTutorial.Scripts.PlayFab;
using UnityEngine;

namespace FMGames.Playfab.Login {
    public class EmailLogin : ILogin {
        public class EmailLoginParams {
            public string username;
            public string password;

            public EmailLoginParams(string username, string password) {
                this.username = username;
                this.password = password;
            }
        }
        
        public void Login(GetPlayerCombinedInfoRequestParams loginInfoParams, Action<LoginResult> loginSuccess, Action<PlayFabError> loginFailure, object loginParams) {
            EmailLoginParams emailLoginParams = loginParams as EmailLoginParams;
            if (emailLoginParams == null) {
                loginFailure.Invoke(new PlayFabError());
                Debug.LogError("Login Parameter is null");

                return;
            }
            
            var request = new LoginWithPlayFabRequest {
                TitleId = "9C8D9",//PlayFabConstants.TitleID,
                Password = emailLoginParams.password,
                Username = emailLoginParams.username,
                InfoRequestParameters = loginInfoParams,
            };

            PlayFabClientAPI.LoginWithPlayFab(request, loginSuccess, loginFailure);
        }
    }
}