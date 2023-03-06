using System;
using PlayFab;
using PlayFab.ClientModels;
using PlayfabTutorial.Scripts.PlayFab;

namespace FMGames.Playfab.Login {
    public class SteamLogin : ILogin {
        public void Login(GetPlayerCombinedInfoRequestParams loginInfoParams, Action<LoginResult> loginSuccess, Action<PlayFabError> loginFailure, object loginParams) {
            var request = new LoginWithSteamRequest {
                TitleId = PlayFabConstants.TitleID,
                InfoRequestParameters = loginInfoParams,
            };

            PlayFabClientAPI.LoginWithSteam(request, loginSuccess, loginFailure);
        }
    }
}