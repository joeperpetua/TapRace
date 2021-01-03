using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;
using System.Linq;

public class PlayFabLogin : MonoBehaviour
{

    public PlayerInfoRenderer PIR;
    public scenesManagement SM;
    public GameObject changeUsernameScreen;

    public void Start()
    {
        #if UNITY_ANDROID
        CreatePlayer();
        #endif

        
    }

    private static string ReturnMobileID()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        return deviceID;
    }

    void CreatePlayer()
    {
        PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest
        {
            AndroidDeviceId = ReturnMobileID(),
            CreateAccount = true
        }, result =>
        {
            if (PlayerPrefs.GetString("USER_PF_ID") != result.PlayFabId)
            {
                PlayerPrefs.SetString("USER_PF_ID", result.PlayFabId);
                UpdateDisplayName();
            }

            char[] usernameArray = PIR.Username.ToCharArray();
            if (usernameArray.Length == 25 && PIR.Username.Substring(0, 8) == "playerE9")
            {
                UpdateDisplayName();
            }

            Debug.Log("Successfully logged in a player with PlayFabId: " + result.PlayFabId);
        }, error => {
            if (!PlayerPrefs.HasKey("USERNAME"))
            {
                PlayerPrefs.SetString("USERNAME", "playerE957684GES6R4EF8O4E");
            }
            Debug.LogError(error.GenerateErrorReport());
        });
    }


    private void UpdateDisplayName()
    {
        //PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        //{
        //    DisplayName = "playerE9" + PlayerPrefs.GetString("USER_PF_ID") + "E"
        //},
        //result =>
        //{
        //    Debug.Log("USERNAME " + result.DisplayName);
        //    PlayerPrefs.SetString("USERNAME", result.DisplayName);
        //},
        //error =>
        //{
        //    Debug.Log(error.GenerateErrorReport());
        //});

        SM.actualScreen = "profileSettings";
        SM.profileSettings.DOAnchorPos(new Vector2(0, 0), 0f);
        changeUsernameScreen.SetActive(true);

    }



    //void checkPlayer()
    //{
    //    PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { },
    //        result => Debug.Log(result),
    //        error => CreatePlayer()
    //    );
    //}
}