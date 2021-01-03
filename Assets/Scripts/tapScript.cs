using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class tapScript : MonoBehaviour
{

    public TextMeshProUGUI totalValue;
    public TextMeshProUGUI speedValue;
    public TextMeshProUGUI highScore;
    //public GameObject totalObject;
    //public GameObject speedObject;
    public TextMeshProUGUI descriptionText;
    public GameObject tapButton;
    public GameObject tryAgainButton;


    private int hs1Value;
    private int hs2Value;
    private int hs3Value;

    private float time;
    private float startTime;
    private float limitTime;

    private int totalTaps;
    private float tps = 0;
    private bool hasTapped;
    private bool isPressed;

    private void Start()
    {
        time = 0f;
        hasTapped = false;
        totalTaps = 1;
        isPressed = false;

        //PlayerPrefs.DeleteKey("ClassicBest1");
        //PlayerPrefs.DeleteKey("ClassicBest2");
        //PlayerPrefs.DeleteKey("ClassicBest3");

        hs1Value = PlayerPrefs.GetInt("ClassicBest1", 0);

        if (hs1Value == 0)
        {
            highScore.text = "0 / 0TPS";
        }
        else
        {
            highScore.text = hs1Value.ToString() + " / " + (hs1Value / 5f).ToString() + " TPS";
        }
    }

    private void Update()
    {
        time += Time.deltaTime;
        

        if (hasTapped)
        {

            // sets the tps to the real taps made in the seconds passed instead of just dividing by 5s
            tps = Mathf.Floor((totalTaps / (time - startTime)) * 100) / 100;

            //descriptionObject.SetActive(false);
            //speedObject.SetActive(true);
            //totalObject.SetActive(true);

            if(tps != float.PositiveInfinity)
            {
                Debug.Log(tps + " : " + tps.GetType());
                speedValue.text = tps.ToString();
            }

            totalValue.text = totalTaps.ToString();

            // 5 secs ran out
            if (time >= limitTime)
            {
                hasTapped = false;
                tapButton.SetActive(false);
                tryAgainButton.SetActive(true);

                hs1Value = PlayerPrefs.GetInt("ClassicBest1", 0);
                hs2Value = PlayerPrefs.GetInt("ClassicBest2", 0);
                hs3Value = PlayerPrefs.GetInt("ClassicBest3", 0);

                #region Set High Scores

                if (hs1Value == 0 && hs2Value == 0 && hs3Value == 0)
                {
                    // new high score
                    PlayerPrefs.SetInt("ClassicBest1", totalTaps);
                    setNewHighScore();
                }
                else if (totalTaps < hs1Value && hs2Value == 0 && hs3Value == 0)
                {
                    PlayerPrefs.SetInt("ClassicBest2", totalTaps);
                }
                else if (totalTaps < hs2Value && hs3Value == 0)
                {
                    PlayerPrefs.SetInt("ClassicBest3", totalTaps);
                }
                else if (totalTaps > hs1Value && hs2Value == 0)
                {
                    // new high score
                    PlayerPrefs.SetInt("ClassicBest1", totalTaps);
                    PlayerPrefs.SetInt("ClassicBest2", hs1Value);
                    setNewHighScore();
                }
                else if (totalTaps < hs1Value && totalTaps > hs2Value && hs3Value == 0)
                {
                    PlayerPrefs.SetInt("ClassicBest2", totalTaps);
                    PlayerPrefs.SetInt("ClassicBest3", hs2Value);
                }
                else if (totalTaps > hs1Value)
                {
                    // new high score
                    PlayerPrefs.SetInt("ClassicBest1", totalTaps);
                    PlayerPrefs.SetInt("ClassicBest2", hs1Value);
                    PlayerPrefs.SetInt("ClassicBest3", hs2Value);
                    setNewHighScore();
                }
                else if (totalTaps > hs2Value)
                {
                    PlayerPrefs.SetInt("ClassicBest2", totalTaps);
                    PlayerPrefs.SetInt("ClassicBest3", hs2Value);
                }
                else if (totalTaps > hs3Value)
                {
                    PlayerPrefs.SetInt("ClassicBest3", totalTaps);
                }

                #endregion Set High Scores
            }
        }


    }

    public void onClick()
    {
        if (isPressed == false)
        {
            isPressed = true;
        
            if (hasTapped == false)
            {
                hasTapped = true;
                startTime = time;
                limitTime = startTime + 5;

                Debug.Log("Start time: " + startTime);
                Debug.Log("Limit time: " + limitTime);

            }
            else
            {
                if(time < limitTime)
                {
                    totalTaps++;
                }
            }
        }
    }

    public void onPointerUp()
    {
        isPressed = false;
    }

    private void setNewHighScore()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate { StatisticName = "ClassicBest", Value = PlayerPrefs.GetInt("ClassicBest1", 0) },
            }
        },
        result => { Debug.Log("User statistics updated"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });
    }
}
