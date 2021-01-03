using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Photon.Pun.UtilityScripts;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    #region public GameObjects - Components
    public GameObject LoadingText;
    public GameObject MatchedText;
    public GameObject StartingText;
    public GameObject StartingValue;
    public GameObject ErrorText;
    public GameObject MatchmakingContainer;
    public GameObject StartContainer;
    public GameObject FinishContainer;

    public TextMeshProUGUI ping;
    #endregion

    public byte maxPlayersPerRoom = 2;
    public scenesManagement SM;
    private string gameVersion = "1";

    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    void Start()
    {
        Connect();
        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("USERNAME");
    }

    private void Update()
    {
        ping.text = PhotonNetwork.GetPing() + "ms";
    }


    #region Public Methods

    public void Connect()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public void LoadGame()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            LoadingText.SetActive(false);
            MatchedText.SetActive(true);
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(SM.loadVersus());
            }
        }
    }

    
    #endregion


    #region MonoBehaviourPunCallbacks Callbacks


    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to " + PhotonNetwork.CloudRegion + " server");
        ErrorText.SetActive(false);
        LoadingText.SetActive(true);
        // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LocalPlayer.SetScore(0);
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        LoadGame();
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        LoadGame();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        MatchedText.SetActive(false);
        LoadingText.SetActive(true);
    }

    #endregion

}