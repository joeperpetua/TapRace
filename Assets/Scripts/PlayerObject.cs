using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Photon.Pun.UtilityScripts;
using System.ComponentModel;

public class PlayerObject : MonoBehaviour
{
    Player Enemy;
    public TextMeshProUGUI EnemyUsername;
    public TextMeshProUGUI EnemyTaps;
    public TextMeshProUGUI EnemyTPS;

    Player Player;
    public TextMeshProUGUI PlayerUsername;
    public TextMeshProUGUI PlayerTaps;

    public GameObject tapButton;
    public GameObject backButton;

    public GameObject onStart;
    public GameObject onFinish;
    public GameObject onWin;
    public GameObject onDefeat;
    public GameObject onTie;

    public TextMeshProUGUI ping;

    private float time;
    private float startTime;
    private float limitTime = 999999;

    private bool hasTapped;
    private bool isPressed;

    private int[] taps = new int[2];
    private float[] tps = new float[2];


    // Start is called before the first frame update
    void Start()
    {
        time = 0f;

        hasTapped = true;
        startTime = time;
        limitTime = startTime + 5;

        Debug.Log("Start time: " + startTime);
        Debug.Log("Limit time: " + limitTime);

        hasTapped = false;
        isPressed = false;


        Player = PhotonNetwork.LocalPlayer;
        taps[0] = 0; // index 0 is player total taps
        Player.SetScore(taps[0]);

        Enemy = PhotonNetwork.PlayerListOthers[0];
        taps[1] = Enemy.GetScore(); // index 1 is enemy total taps

        Debug.Log("Score P: " + Player.GetScore());
        Debug.Log("Score E: " + Enemy.GetScore());
    }

    // Update is called once per frame
    void Update()
    {
        ping.text = PhotonNetwork.GetPing() + "ms";

        time += Time.deltaTime;

        if (Player.GetScore() != 0 || Enemy.GetScore() != 0)
        {
            // get values
            taps[0] = Player.GetScore();
            tps[0] = Mathf.Floor((taps[0] / (time - startTime)) * 100) / 100;

            taps[1] = Enemy.GetScore();
            tps[1] = Mathf.Floor((taps[1] / (time - startTime)) * 100) / 100;


            // render values
            if (tps[0] != float.PositiveInfinity)
            {
                PlayerTaps.text = taps[0] + " taps / " + tps[0] + "TPS";
            }

            if (tps[1] != float.PositiveInfinity)
            {
                EnemyTaps.text = taps[1].ToString();
                EnemyTPS.text = tps[1] + "TPS";
            }

            if (time >= limitTime)
            {

                hasTapped = false;
                onStart.SetActive(false);
                backButton.SetActive(true);

                if (taps[0] > taps[1])
                {
                    onFinish = onWin;
                }
                else if(taps[0] < taps[1])
                {
                    onFinish = onDefeat;
                }
                else if(taps[0] == taps[1])
                {
                    onFinish = onTie;
                }
                else
                {
                    Debug.Log("Cannot resolve match result");
                }

                if (onFinish)
                {
                    onFinish.SetActive(true);

                    TextMeshProUGUI[] texts = onFinish.GetComponentsInChildren<TextMeshProUGUI>();

                    foreach (TextMeshProUGUI item in texts)
                    {
                        if (item.tag == "PlayerData")
                        {
                            switch (item.text)
                            {
                                case "Username":
                                    item.text = Player.NickName;
                                    break;

                                case "0 taps":
                                    item.text = taps[0] + " taps";
                                    break;

                                case "0TPS":
                                    item.text = tps[0] + "TPS";
                                    break;
                            }
                        }

                        if (item.tag == "EnemyData")
                        {
                            switch (item.text)
                            {
                                case "Username":
                                    item.text = Enemy.NickName;
                                    break;

                                case "0 taps":
                                    item.text = taps[1] + " taps";
                                    break;

                                case "0TPS":
                                    item.text = tps[1] + "TPS";
                                    break;
                            }
                        }
                    }

                    if (PhotonNetwork.CurrentRoom != null)
                    {
                        PhotonNetwork.CurrentRoom.IsOpen = false;
                    }
                    
                }

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
                // will add score if is the first tap
                if (time < limitTime)
                {
                    Player.AddScore(1);
                }

            }
            else
            {
                if (time < limitTime)
                {
                    Player.AddScore(1);
                }
            }
        }
    }

    public void onPointerUp()
    {
        isPressed = false;
    }

}
