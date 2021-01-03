using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class enduranceScript : MonoBehaviour
{
    public TMPro.TextMeshProUGUI timePassedValue;
    public TMPro.TextMeshProUGUI speedText;
    public TMPro.TextMeshProUGUI speedValue;
    public TMPro.TextMeshProUGUI highScore;
    //public GameObject totalObject;
    //public GameObject speedObject;
    public GameObject adviceText;
    public GameObject adviceText2;
    public GameObject tapButton;
    public GameObject tryAgainButton;


    private int hs1Value;
    private float hs1Valuef;
    private int hs2Value;
    private int hs3Value;

    private float time;
    private float startTime;
    private int startTimeMS;
    private float timePassed;
    private int timePassedMS;

    private int totalTaps;
    private float tps = 0;
    private bool hasTapped;
    private bool isPressed;

    void Start()
    {
        time = 0f;
        isPressed = false;
        hasTapped = false;
        totalTaps = 1;

        //PlayerPrefs.DeleteKey("EnduranceBest1");
        //PlayerPrefs.DeleteKey("EnduranceBest2");
        //PlayerPrefs.DeleteKey("EnduranceBest3");

        Debug.Log(PlayerPrefs.GetInt("EnduranceBest1"));

        hs1Valuef = Mathf.Floor((PlayerPrefs.GetInt("EnduranceBest1", 0) / 1000f) * 100) / 100;

        if (hs1Valuef == 0)
        {
            highScore.text = "0s";
        }
        else
        {
            Debug.Log(hs1Valuef);
            highScore.text = hs1Valuef + "s";
        }
    }


    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (hasTapped)
        {
            //Debug.Log("Time: " + timePassed + " // TPS: " + tps + " // Total taps: " + totalTaps);

            // sets the tps to the real taps made in the seconds passed instead of just dividing by 5s
            tps = (totalTaps / (time - startTime));

            //descriptionObject.SetActive(false);
            //speedObject.SetActive(true);
            //totalObject.SetActive(true);

            if (tps != float.PositiveInfinity)
            {
                //if (timePassed < 2 && tps < 8.7f)
                //{
                //    speedValue.text = "Tap faster!";
                //}
                //else
                //{
                //    speedValue.text = tps.ToString("F2");
                //}

                speedValue.text = tps.ToString("F2");

            }

            timePassedMS = Mathf.FloorToInt((time - startTime) * 1000);


            timePassed = timePassedMS / 1000f;

            timePassedValue.text = timePassed.ToString("F2") + "s";

            // 5 secs ran out
            if (timePassed > 2f && tps < 8.5f)
            {
                hasTapped = false;
                tapButton.SetActive(false);
                tryAgainButton.SetActive(true);

                adviceText.SetActive(false);
                adviceText2.SetActive(false);
                speedText.text = "";
                speedValue.text = "";


                Debug.Log("total taps: " + totalTaps + " - passed time: " + timePassedMS );

                hs1Value = PlayerPrefs.GetInt("EnduranceBest1", 0);
                hs2Value = PlayerPrefs.GetInt("EnduranceBest2", 0);
                hs3Value = PlayerPrefs.GetInt("EnduranceBest3", 0);

                // set high scores in seconds
                #region

                if (hs1Value == 0 && hs2Value == 0 && hs3Value == 0)
                {
                    // new high score
                    PlayerPrefs.SetInt("EnduranceBest1", timePassedMS);
                    setNewHighScore();
                }
                else if (timePassedMS < hs1Value && hs2Value == 0 && hs3Value == 0)
                {
                    PlayerPrefs.SetInt("EnduranceBest2", timePassedMS);
                }
                else if (timePassedMS < hs2Value && hs3Value == 0)
                {
                    PlayerPrefs.SetInt("EnduranceBest3", timePassedMS);
                }
                else if (timePassedMS > hs1Value && hs2Value == 0)
                {
                    // new highscore
                    PlayerPrefs.SetInt("EnduranceBest1", timePassedMS);
                    PlayerPrefs.SetInt("EnduranceBest2", hs1Value);
                    setNewHighScore();
                }
                else if (timePassedMS < hs1Value && totalTaps > hs2Value && hs3Value == 0)
                {
                    PlayerPrefs.SetInt("EnduranceBest2", timePassedMS);
                    PlayerPrefs.SetInt("EnduranceBest3", hs2Value);
                }
                else if (timePassedMS > hs1Value)
                {
                    // new highscore
                    PlayerPrefs.SetInt("EnduranceBest1", timePassedMS);
                    PlayerPrefs.SetInt("EnduranceBest2", hs1Value);
                    PlayerPrefs.SetInt("EnduranceBest3", hs2Value);
                    setNewHighScore();
                }
                else if (timePassedMS > hs2Value)
                {
                    PlayerPrefs.SetInt("EnduranceBest2", timePassedMS);
                    PlayerPrefs.SetInt("EnduranceBest3", hs2Value);
                }
                else if (timePassedMS > hs3Value)
                {
                    PlayerPrefs.SetInt("EnduranceBest3", timePassedMS);
                }

                #endregion


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
                startTimeMS = Mathf.FloorToInt(time * 1000);
                

                Debug.Log("Start time: " + startTime);
            }
            else
            {
                totalTaps++;
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
            new StatisticUpdate { StatisticName = "EnduranceBest", Value = PlayerPrefs.GetInt("EnduranceBest1", 0) },
            }
        },
        result => { Debug.Log("User statistics updated"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });
    }


}

