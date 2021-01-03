using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hsScript : MonoBehaviour
{

    public Text hs1;
    public Text hs2;
    public Text hs3;

    public GameObject confirmation;
    public GameObject localScores;
    public GameObject onlineScores;

    private int hs1Value;
    private int hs2Value;
    private int hs3Value;

    void Start()
    {
        hs1 = GameObject.Find("first place").GetComponent<Text>();
        hs2 = GameObject.Find("second place").GetComponent<Text>();
        hs3 = GameObject.Find("third place").GetComponent<Text>();
        hs1Value = PlayerPrefs.GetInt("HighScore1", 0);
        hs2Value = PlayerPrefs.GetInt("HighScore2", 0);
        hs3Value = PlayerPrefs.GetInt("HighScore3", 0);
        
        if (hs1Value == 0)
        {
            hs1.text = "play at least once :)";
            hs2.text = "";
            hs3.text = "";
        }
        else
        {
            hs1.text = "1° " + hs1Value.ToString() + " taps / " + (hs1Value / 5f).ToString() + " TPS";
            hs2.text = "2° " + hs2Value.ToString() + " taps / " + (hs2Value / 5f).ToString() + " TPS";
            hs3.text = "3° " + hs3Value.ToString() + " taps / " + (hs3Value / 5f).ToString() + " TPS";
        }

    }

    public void confirmationPopup()
    {
        confirmation.SetActive(true);
    }

    public void keepHs()
    {
        confirmation.SetActive(false);
    }

    public void resetHs()
    {
        PlayerPrefs.DeleteKey("HighScore1");
        PlayerPrefs.DeleteKey("HighScore2");
        PlayerPrefs.DeleteKey("HighScore3");

        hs1.text = "play at least once :)";
        hs2.text = "";
        hs3.text = "";

        confirmation.SetActive(false);
    }

    public void showLocal()
    {
        onlineScores.SetActive(false);
        localScores.SetActive(true);
    }

    public void showOnline()
    {
        localScores.SetActive(false);
        onlineScores.SetActive(true);
    }

}
