using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.UI;

public class PlayerInfoRenderer : MonoBehaviour
{
    public TextMeshProUGUI expBarUsername;
    public TextMeshProUGUI profileUsername;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI adviceText;
    public GameObject updatedMessage;

    public TextMeshProUGUI bestClassic1Text;
    public TextMeshProUGUI bestClassic2Text;
    public TextMeshProUGUI bestClassic3Text;

    public TextMeshProUGUI bestEndurance1Text;
    public TextMeshProUGUI bestEndurance2Text;
    public TextMeshProUGUI bestEndurance3Text;

    public Transform ClassicLeaderboardContainer;
    public Transform EnduranceLeaderboardContainer;
    public GameObject ClassicLeaderboardItem;
    public GameObject EnduranceLeaderboardItem;

    private int bestClassic1;
    private int bestClassic2;
    private int bestClassic3;

    private int bestEndurance1;
    private int bestEndurance2;
    private int bestEndurance3;

    private bool synced = false;

    public GameObject inputField;
    public GameObject confirmButton;

    private string PlayFabID;
    public string Username;

    // Start is called before the first frame update
    void Start()
    {
        PlayFabID = PlayerPrefs.GetString("USER_FP_ID");
        Username = PlayerPrefs.GetString("USERNAME");

        RenderPlayerInfo();
        StartCoroutine(WaitForLogin());
    }

    IEnumerator WaitForLogin()
    {
        yield return new WaitForSeconds(3f);

        GetPlayerProfile();
        SyncBestScores();
        GetUserData();
        GetLeaderboards();
    }

    #region Profile Info
    private void GetPlayerProfile()
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = PlayFabID, 
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        },
        result => { 
            PlayerPrefs.SetString("USERNAME", result.PlayerProfile.DisplayName);
            Debug.Log("The player's DisplayName profile data is: " + result.PlayerProfile.DisplayName);
            RenderPlayerInfo();
        },
        error => {
            RenderPlayerInfo();
            //Debug.LogError(error.GenerateErrorReport());
        });
    }

    private void RenderPlayerInfo()
    {
        Username = PlayerPrefs.GetString("USERNAME");
        expBarUsername.text = Username;
        profileUsername.text = Username;

        // if offline
        if(!synced)
        {
            bestClassic1Text.text = "1° " + PlayerPrefs.GetInt("ClassicBest1", 0) + " taps / " + (PlayerPrefs.GetInt("ClassicBest1", 0) / 5f) + "TPS";
            bestClassic2Text.text = "2° " + PlayerPrefs.GetInt("ClassicBest2", 0) + " taps / " + (PlayerPrefs.GetInt("ClassicBest2", 0) / 5f) + "TPS";
            bestClassic3Text.text = "3° " + PlayerPrefs.GetInt("ClassicBest3", 0) + " taps / " + (PlayerPrefs.GetInt("ClassicBest3", 0) / 5f) + "TPS";

            bestEndurance1Text.text = (PlayerPrefs.GetInt("EnduranceBest1", 0) / 1000f) + "s";
            bestEndurance2Text.text = (PlayerPrefs.GetInt("EnduranceBest2", 0) / 1000f) + "s";
            bestEndurance3Text.text = (PlayerPrefs.GetInt("EnduranceBest3", 0) / 1000f) + "s";
        }
        else
        {
            bestClassic1Text.text = "1° " + bestClassic1 + " taps / " + (bestClassic1 / 5f) + "TPS";
            bestClassic2Text.text = "2° " + bestClassic2 + " taps / " + (bestClassic2 / 5f) + "TPS";
            bestClassic3Text.text = "3° " + bestClassic3 + " taps / " + (bestClassic3 / 5f) + "TPS";

            bestEndurance1Text.text = "1° " + (bestEndurance1 / 1000f) + "s";
            bestEndurance2Text.text = "2° " + (bestEndurance2 / 1000f) + "s";
            bestEndurance3Text.text = "3° " + (bestEndurance3 / 1000f) + "s";
        }


        
    }

    public void UpdateDisplayName()
    {
        TMP_InputField inputFieldText = GameObject.Find("Username InputField").GetComponent<TMP_InputField>();

        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = inputFieldText.text
        }, 
        result =>
        {
            PlayerPrefs.SetString("USERNAME", result.DisplayName);
            RenderPlayerInfo();

            descriptionText.text = "Username changed successfully!";
            inputField.SetActive(false);
            confirmButton.SetActive(false);
            adviceText.text = "";
            
        }, 
        error => {

            switch (error.ErrorMessage)
            {
                case "Invalid input parameters":
                    descriptionText.text = "Enter a valid username between 3-25 characters.";
                    break;

                case "Name not available":
                    descriptionText.text = "This username is already taken.";
                    break;

                case "Cannot connect to destination host":
                    descriptionText.text = "Impossible to reach the servers. Please check your internet connection.";
                    break;

                default:
                    descriptionText.text = "There has been an error while changing the name, please contact with the support team.";
                    break;
            }

            Debug.Log(error.ErrorMessage);
        });
    }

    public void enableChangeUsernamePopUp()
    {
        descriptionText.text = "Type your new username here:";
        adviceText.text = "A network connection is required to make this change.";
        inputField.SetActive(true);
        confirmButton.SetActive(true);
    }

    #endregion Profile Info

    #region Personal Scores 
    private void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = PlayFabID,
            Keys = null
        }, result =>
        {
            synced = true;

            bestClassic1 = Int32.Parse(result.Data["ClassicBest1"].Value);
            bestClassic2 = Int32.Parse(result.Data["ClassicBest2"].Value);
            bestClassic3 = Int32.Parse(result.Data["ClassicBest3"].Value);

            bestEndurance1 = Int32.Parse(result.Data["EnduranceBest1"].Value);
            bestEndurance2 = Int32.Parse(result.Data["EnduranceBest2"].Value);
            bestEndurance3 = Int32.Parse(result.Data["EnduranceBest3"].Value);

            RenderPlayerInfo();
        }, (error) =>
        {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    private void SyncBestScores()
    {
        // sets player data (best 3 - classic & endurance)
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                { "ClassicBest1", PlayerPrefs.GetInt("ClassicBest1", 0).ToString() },
                { "ClassicBest2", PlayerPrefs.GetInt("ClassicBest2", 0).ToString() },
                { "ClassicBest3", PlayerPrefs.GetInt("ClassicBest3", 0).ToString() },

                { "EnduranceBest1", PlayerPrefs.GetInt("EnduranceBest1", 0).ToString() },
                { "EnduranceBest2", PlayerPrefs.GetInt("EnduranceBest2", 0).ToString() },
                { "EnduranceBest3", PlayerPrefs.GetInt("EnduranceBest3", 0).ToString() },
            }
        },
        result => Debug.Log("Successfully updated user data"),
        error =>
        {
            Debug.Log("Got error setting user data Ancestor to Arthur");
            Debug.Log(error.GenerateErrorReport());
        });

        //sets player statistics (best score in classic)
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate { StatisticName = "ClassicBest", Value = PlayerPrefs.GetInt("ClassicBest1", 0) },
            }
        },
        result => { Debug.Log("User statistics updated - classic"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });

        //sets player statistics (best score in endurance)
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate { StatisticName = "EnduranceBest", Value = PlayerPrefs.GetInt("EnduranceBest1", 0) },
            }
        },
        result => { Debug.Log("User statistics updated - endurance"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    #endregion Personal Scores

    public void GetLeaderboards()
    {
        #region Get Classic Leaderboard
        PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest()
        {
            MaxResultsCount = 20,
            PlayFabId = PlayFabID,
            StatisticName = "ClassicBest",
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowStatistics = true
            }

        }, result =>
        {
            ClassicLeaderboardContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 521);

            if (GameObject.FindGameObjectWithTag("ClassicScoreItem"))
            {
                GameObject.Destroy(GameObject.Find("To Delete"));
                GameObject[] registries = GameObject.FindGameObjectsWithTag("ClassicScoreItem");
                foreach (GameObject item in registries)
                {
                    GameObject.Destroy(item);
                    //Debug.Log("deleted");
                }
            }



            foreach (var item in result.Leaderboard)
            {
                //Debug.Log("POSITION: " + item.Position);
                //Debug.Log("USER: " + item.DisplayName);
                //Debug.Log("SCORE: " + item.StatValue);

                ClassicLeaderboardContainer.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 120);

                GameObject ScoreItem = Instantiate(ClassicLeaderboardItem, ClassicLeaderboardContainer, false);
                ScoreItem.tag = "ClassicScoreItem";

                ClassicLeaderboardItem.SetActive(false);
                ScoreItem.SetActive(true);


                if (item.PlayFabId == PlayerPrefs.GetString("USER_PF_ID"))
                {
                    Image tmp = ScoreItem.GetComponent<Image>();
                    tmp.color = new Color32(255, 125, 0, 255);
                    //Debug.Log("changed color");
                }

                Array components = ScoreItem.GetComponentsInChildren<TextMeshProUGUI>();

                foreach (TextMeshProUGUI component in components)
                {
                    switch (component.fontSize)
                    {
                        case 65:
                            component.text = item.Position + 1 + "°";
                            break;

                        case 53:
                            component.text = item.DisplayName;
                            break;

                        case 43:
                            component.text = item.StatValue + " taps / " + (item.StatValue / 5f) + "TPS";
                            break;
                        }
                    }
                }

        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
        #endregion Get Classic Leaderboard


















        #region Get Endurance Leaderboard
        PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest()
        {
            MaxResultsCount = 20,
            PlayFabId = PlayFabID,
            StatisticName = "EnduranceBest",
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowStatistics = true
            }

        }, result =>
        {
            EnduranceLeaderboardContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 521);

            if (GameObject.FindGameObjectWithTag("EnduranceScoreItem"))
            {
                GameObject[] registries = GameObject.FindGameObjectsWithTag("EnduranceScoreItem");
                foreach (GameObject item in registries)
                {
                    GameObject.Destroy(item);
                    //Debug.Log("deleted");
                }
            }



            foreach (var item in result.Leaderboard)
            {
                //Debug.Log("POSITION: " + item.Position);
                //Debug.Log("USER: " + item.DisplayName);
                //Debug.Log("SCORE: " + item.StatValue);

                EnduranceLeaderboardContainer.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 120);


                GameObject ScoreItem = Instantiate(EnduranceLeaderboardItem, EnduranceLeaderboardContainer, false);
                ScoreItem.tag = "EnduranceScoreItem";

                EnduranceLeaderboardItem.SetActive(false);
                ScoreItem.SetActive(true);

                if (item.PlayFabId == PlayerPrefs.GetString("USER_PF_ID"))
                {
                    Image tmp = ScoreItem.GetComponent<Image>();
                    tmp.color = new Color32(255, 125, 0, 255);
                    //Debug.Log("changed color");
                }

                Array components = ScoreItem.GetComponentsInChildren<TextMeshProUGUI>();

                foreach (TextMeshProUGUI component in components)
                {
                    switch (component.fontSize)
                    {
                        case 65:
                            component.text = item.Position + 1 + "°";
                            break;

                        case 53:
                            component.text = item.DisplayName;
                            break;

                        case 43:
                            component.text = item.StatValue / 1000f + "s";
                            break;
                    }
                }
            }

        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
        #endregion Get Endurance Leaderboard
    }

    public void updateData()
    {
        GetPlayerProfile();
        SyncBestScores();
        GetUserData();
        GetLeaderboards();
        StartCoroutine(showTempMsg());
    }

    IEnumerator showTempMsg()
    {
        updatedMessage.SetActive(true);
        yield return new WaitForSeconds(3f);
        updatedMessage.SetActive(false);
    }

}
