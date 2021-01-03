using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class scenesManagement : MonoBehaviourPunCallbacks
{
    
    public RectTransform mainMenu, profile, profileSettings, settings, playModes, leaderboards;
    public Animator transitionAnim;
    public string actualScreen = "mainMenu";
    //private string sceneToChange;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && actualScreen == "mainMenu")
        {
            Application.Quit();
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("back pressed, screen: " + actualScreen);
            goToMainMenu();
        }
    }

    public void goToMainMenu()
    {
        switch (actualScreen)
        {
            case "profile":
                //mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f);
                profile.DOAnchorPos(new Vector2(-1000, 0), 0.25f);
                actualScreen = "mainMenu";
                break;

            case "leaderboards":
                leaderboards.DOAnchorPos(new Vector2(-1000, 0), 0.25f);
                //profile.DOAnchorPos(new Vector2(0, 0), 0.25f);
                actualScreen = "profile";
                break;

            case "settings":
                //mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f);
                settings.DOAnchorPos(new Vector2(0, -2000), 0.25f);
                profileSettings.DOAnchorPos(new Vector2(-1000, -2000), 0.25f);
                actualScreen = "mainMenu";
                break;

            case "playModes":
                //mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f);
                playModes.DOAnchorPos(new Vector2(1000, 0), 0.25f);
                actualScreen = "mainMenu";
                break;

            case "profileSettings":
                profileSettings.DOAnchorPos(new Vector2(-1000, 0), 0.25f);
                actualScreen = "settings";
                break;

            default:
                Debug.Log("Screen not found : " + actualScreen);
                //sceneToChange = "mainMenu";
                StartCoroutine(loadScene("mainMenu"));
                actualScreen = "mainMenu";
                break;
        }

    }

    IEnumerator loadScene(string sceneToChange)
    {
        transitionAnim.SetTrigger("change");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneToChange);
    }

    public IEnumerator loadVersus()
    {
        transitionAnim.SetTrigger("change");
        yield return new WaitForSeconds(1f);
        PhotonNetwork.LoadLevel(4);
    }


    // FROM MAIN MENU
    #region

    public void goToProfile()
    {
        actualScreen = "profile";
        Debug.Log(actualScreen);
        //mainMenu.DOAnchorPos(new Vector2(1000, 0), 0.25f);
        profile.DOAnchorPos(new Vector2(0, 0), 0.25f);
    }

    public void goToSettings()
    {
        actualScreen = "settings";
        Debug.Log(actualScreen);
        //mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f);
        settings.DOAnchorPos(new Vector2(0, 0), 0.25f);
        profileSettings.DOAnchorPos(new Vector2(-1000, 0), 0.25f);
    }

    public void goToProfileSettings()
    {
        actualScreen = "profileSettings";
        Debug.Log(actualScreen);
        //mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f);
        profileSettings.DOAnchorPos(new Vector2(0, 0), 0.25f);
    }

    public void goToPlayModes()
    {
        actualScreen = "playModes";
        Debug.Log(actualScreen);
        //mainMenu.DOAnchorPos(new Vector2(-1000, 0), 0.25f);
        playModes.DOAnchorPos(new Vector2(0, 0), 0.25f);
    }

    public void goToLeaderboards()
    {
        actualScreen = "leaderboards";
        Debug.Log(actualScreen);
        //mainMenu.DOAnchorPos(new Vector2(-1000, 0), 0.25f);
        leaderboards.DOAnchorPos(new Vector2(0, 0), 0.25f);
    }

    #endregion


    public void goToClassic()
    {
        //sceneToChange = "ClassicPlayMode";
        StartCoroutine(loadScene("ClassicPlayMode"));
    }

    public void goToEndurance()
    {
        //sceneToChange = "EndurancePlayMode";
        StartCoroutine(loadScene("EndurancePlayMode"));
    }

    public void goToVersus()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        //sceneToChange = "EndurancePlayMode";
        actualScreen = "versus";
        Debug.Log(actualScreen);
        StartCoroutine(loadScene("VersusPlayMode"));
    }

    public void check()
    {
        Debug.Log("Asd");
    }

    public void LeaveVersus()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
        StartCoroutine(loadScene("mainMenu"));
        actualScreen = "mainMenu";
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }


}
