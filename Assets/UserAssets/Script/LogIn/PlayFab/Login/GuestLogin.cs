using System;
using System.Globalization;
using System.Text;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace FMGames.Playfab.Login {
    public class GuestLogin : ILogin {
        public class GuestLoginParameters {
            public string guestId;

            public GuestLoginParameters(string guestId) {
                this.guestId = guestId;
            }
        }
        
        public void Login(GetPlayerCombinedInfoRequestParams loginInfoParams, Action<LoginResult> loginSuccess,
            Action<PlayFabError> loginFailure, object loginParams) {
            string guestCustomID;
            
            GuestLoginParameters guestLoginParams = loginParams as GuestLoginParameters;
            if (guestLoginParams == null) {
                loginFailure.Invoke(new PlayFabError());
                Debug.LogError("Login Parameter is null");

                return;
            }

            if (guestLoginParams.guestId.Length < 1) {
                StringBuilder sb = new StringBuilder("guest://");
                System.Random rnd = new System.Random();
                string guestID = rnd.Next(100000, 1000000).ToString();

                sb.Append(guestID);
                sb.Append("/");
                sb.Append(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));

                guestCustomID = sb.ToString();

                PlayerPrefs.SetString("GUEST_ID", guestCustomID);
            } else guestCustomID = guestLoginParams.guestId;

            var request = new LoginWithCustomIDRequest {
                CustomId = guestCustomID,
                CreateAccount = true,
                InfoRequestParameters = loginInfoParams,
            };

            PlayFabClientAPI.LoginWithCustomID(request, loginSuccess, loginFailure);
        }
    }
}