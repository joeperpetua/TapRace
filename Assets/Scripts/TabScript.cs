using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabScript : MonoBehaviour
{
    public GameObject Classic;
    public GameObject Versus;
    public GameObject Endurance;

    public GameObject ClassicContainer;
    public GameObject VersusContainer;
    public GameObject EnduranceContainer;

    private Image ClassicImg;
    private Image VersusImg;
    private Image EnduranceImg;

    public TextMeshProUGUI ClassicText;
    public TextMeshProUGUI VersusText;
    public TextMeshProUGUI EnduranceText;

    public GameObject PlayerInfoRenderer;


    private void Start()
    {
        ClassicImg = Classic.GetComponent<Image>();
        VersusImg = Versus.GetComponent<Image>();
        EnduranceImg = Endurance.GetComponent<Image>();
    }

    public void toClassic()
    {
        ClassicImg.color = new Color32(255, 255, 255, 255);
        VersusImg.color = new Color32(58, 58, 58, 255);
        EnduranceImg.color = new Color32(58, 58, 58, 255);

        ClassicText.color = new Color32(112, 112, 112, 255);
        VersusText.color = new Color32(255, 255, 255, 255);
        EnduranceText.color = new Color32(255, 255, 255, 255);

        VersusContainer.SetActive(false);
        EnduranceContainer.SetActive(false);
        ClassicContainer.SetActive(true);

        PlayerInfoRenderer.GetComponent<PlayerInfoRenderer>().GetLeaderboards();
    }

    public void toVersus()
    {
        ClassicImg.color = new Color32(58, 58, 58, 255);
        VersusImg.color = new Color32(255, 255, 255, 255);
        EnduranceImg.color = new Color32(58, 58, 58, 255);

        ClassicText.color = new Color32(255, 255, 255, 255);
        VersusText.color = new Color32(112, 112, 122, 255);
        EnduranceText.color = new Color32(255, 255, 255, 255);

        EnduranceContainer.SetActive(false);
        ClassicContainer.SetActive(false);
        VersusContainer.SetActive(true);

        PlayerInfoRenderer.GetComponent<PlayerInfoRenderer>().GetLeaderboards();
    }

    public void toEndurance()
    {
        ClassicImg.color = new Color32(58, 58, 58, 255);
        VersusImg.color = new Color32(58, 58, 58, 255);
        EnduranceImg.color = new Color32(255, 255, 255, 255);

        ClassicText.color = new Color32(255, 255, 255, 255);
        VersusText.color = new Color32(255, 255, 255, 255);
        EnduranceText.color = new Color32(112, 112, 112, 255);

        VersusContainer.SetActive(false);
        ClassicContainer.SetActive(false);
        EnduranceContainer.SetActive(true);

        PlayerInfoRenderer.GetComponent<PlayerInfoRenderer>().GetLeaderboards();
    }
}
