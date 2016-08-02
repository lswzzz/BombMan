using UnityEngine;
using System.Collections;
using proto.serverproto;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using proto.clientproto;
using System.Collections.Generic;

public class RoomPanelEvent : MonoBehaviour {
    public GameObject playerPrefabs;
    private int ownerFd;
    private string ownerName;
    private string roomName;
    private int currentMan;
    private int maxMan;
    public Transform content;
    public Text showText;
    public Button gameStartButton;
    public void roomOwnerSetData(string ownerName, int ownerFd, string roomName, int currentMan, int maxMan)
    {
        this.ownerName = ownerName;
        this.ownerFd = ownerFd;
        this.roomName = roomName;
        this.currentMan = currentMan;
        this.maxMan = maxMan;
        GameGlobalData.roomName = roomName;
        GameGlobalData.roomOwnerFd = ownerFd;
        GameGlobalData.roomOwnerName = ownerName;
        Debug.Log("room info :" + ownerFd + "    " + GameGlobalData.fd);
        addRoomOwner();
        gameStartButton.gameObject.SetActive(true);
    }

    public void clearAndReset()
    {
        this.ownerName = GameGlobalData.roomOwnerName;
        this.ownerFd = GameGlobalData.roomOwnerFd;
        this.roomName = GameGlobalData.roomName;
        this.currentMan = GameGlobalData.currentMan;
        this.maxMan = GameGlobalData.maxManCount;
    }

    public void addRoomOwner()
    {
        ResetChild(1);
        PlayerPanel player = content.GetChild(0).GetComponent<PlayerPanel>();
        player.setData(ownerName, ownerFd);
    }

    public void addPlayer(string name, int fd, int index)
    {
        PlayerPanel player = content.GetChild(index).GetComponent<PlayerPanel>();
        player.setData(name, fd);
    }

	void Awake(){
	
	}
	
	// Use this for initialization
	void Start () {

    }

    void ResetChild(int count)
    {
        
        int curChildCount = content.childCount;
        if (curChildCount < count)
        {
            int num = count - curChildCount;
            for (int i = 0; i < num; i++)
            {
                Instantiate(playerPrefabs).transform.parent = content;
            }
        }
        else if (curChildCount > count)
        {
            int num = curChildCount - count;
            for (int i = curChildCount - 1; i > count - 1; i--)
            {
                content.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void PlayerJoinRoom(JoinRoomResponse resp)
    {
        if(GameGlobalData.fd == resp.roomOwnerFd)
        {
            gameStartButton.gameObject.SetActive(true);
        }
        else
        {
            gameStartButton.gameObject.SetActive(false);
        }
                
        ResetChild(resp.manCount);
        int index = 0;
        foreach(SinglePlayerInfo info in resp.playerInfo)
        {
            addPlayer(info.userName, info.fd, index);
            ++index;
        }
    }

    //不处理退出

    public void PlayerGameClick()
    {
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.ROOMOWNERREADYSTARTGAME;
        RoomOwnerReadyStartGame data = new RoomOwnerReadyStartGame();
        data.fd = ownerFd;
        req.roomOwnerReadyStartGame = data;
        ConnectSocket.getSocketInstance().sendMSG(PBCSerialize.Serialize(req));
    }

    IEnumerator StartGameRequest()
    {
        yield return new WaitForSeconds(3);
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.ROOMOWNERSTARTGAME;
        RoomOwnerStartGame data = new RoomOwnerStartGame();
        data.fd = ownerFd;
        req.roomOwnerStartGame = data;
        ConnectSocket.getSocketInstance().sendMSG(PBCSerialize.Serialize(req));
    }

    public void serverAcceptStartGame(RoomOwnerReadyStartGameResponse resp)
    {
        showText.text = "游戏即将开始";
        showText.gameObject.SetActive(true);
        savaPlayerData();
        if(GameGlobalData.fd == GameGlobalData.roomOwnerFd)
        {
            StartCoroutine(StartGameRequest());
        }
        Debug.Log("serverAcceptStartGame");
    }

    public void startGameInit(RoomOwnerStartGameResponse resp)
    {
        Random.seed = resp.seed;
        foreach(PlayerGameInitInfo info in resp.playerGameInitInfo)
        {
            foreach(KeyValuePair<int, PlayerNetWorkData> key in GameGlobalData.playerList)
            {
                PlayerNetWorkData player = key.Value;
                if (player.fd == info.fd)
                {
                    player.initPosition = new Vector2(info.posX, info.posY);
                    player.initPosName = info.posName;
                    player.maxSpeed = resp.maxSpeed;
                    player.speed = resp.speed;
                    player.power = resp.power;
                    player.maxPower = resp.maxPower;
                    player.bubbleCount = resp.bubbleCount;
                    player.maxBubbleCount = resp.maxBubbleCount;
                    player.boomTime = resp.boomTime;
                    player.maxDistance = resp.maxDistance;
                    player.joystickPercent = resp.joystickPercent;
                    break;
                }
            }
        }
        if(resp.playerGameInitInfo.Count == 1)
        {
            ExceptionControl.Instance.setShowReload(true);
        }
        SceneManager.LoadScene("scene1");
    }

    void savaPlayerData()
    {
        GameGlobalData.playerList.Clear();
        foreach (Transform child in content)
        {
            PlayerPanel data = child.GetComponent<PlayerPanel>();
            PlayerNetWorkData gameData = new PlayerNetWorkData();
            gameData.fd = data.fd;
            gameData.playerName = data.playerName;
            GameGlobalData.playerList.Add(gameData.fd, gameData);
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
	
	void FixedUpdate(){
	
	}
	
	void LateUpdate(){
	
	}
}
