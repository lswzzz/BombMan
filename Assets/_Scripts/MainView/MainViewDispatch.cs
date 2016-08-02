using UnityEngine;
using System.Collections;
using proto.serverproto;
using UnityEngine.UI;
using System;
using proto.clientproto;

public class MainViewDispatch : MonoBehaviour {
    private ConnectSocket socket;
    private Transform createRoomPanel;
    private string roomName;
    private int currentMan;
    private int maxManCount;
    private string roomOwner;
    private Text roomOwnerText;
    private OrganizePanelEvent organizePanelEvent;
    private RoomPanelEvent roomPanelEvent;
    private float deltaTime;
    private float maxDeltaTime;
    private bool needFlush;
    private Text type;
	// Use this for initialization
	void Start () {
        socket = ConnectSocket.getSocketInstance();
        SocketQueue.getInstance().setCallback(getPbcResponse);
        createRoomPanel = gameObject.transform.parent.Find("CreateRoomPanel").GetComponent<Transform>();
        roomOwnerText = transform.Find("name").GetComponent<Text>();
        roomOwner = PlayerPrefs.GetString("UserName");
        roomOwnerText.text = roomOwner;
        organizePanelEvent = gameObject.transform.parent.Find("OrganizePanel").GetComponent<OrganizePanelEvent>();
        roomPanelEvent = gameObject.transform.parent.Find("RoomPanel").GetComponent<RoomPanelEvent>();
        needFlush = true;
        deltaTime = 0.0f;
        maxDeltaTime = 10f;
        type = transform.parent.Find("Type").GetComponent<Text>();
        createRoomPanel.Find("roomName").GetComponent<InputField>().text = "123";
        createRoomPanel.Find("maxManCount").GetComponent<InputField>().text = "2";
    }
	
	// Update is called once per frame
	void Update () {
        deltaTime += Time.deltaTime;
        if (deltaTime >= maxDeltaTime && needFlush)
        {
            flushRoomListRequest();
            deltaTime = 0.0f;
        }
        SocketQueue.getInstance().tick();
    }

    public void getPbcResponse(ProtocolNetResponse resp)
    {
        switch (resp.cmd)
        {
            case (int)NetRequestType.CREATEROOM:
                if (resp.createRoomResponse.result)
                {
                    organizePanelEvent.gameObject.SetActive(false);
                    roomPanelEvent.gameObject.SetActive(true);
                    roomPanelEvent.roomOwnerSetData(roomOwner, GameGlobalData.fd, roomName, currentMan, maxManCount);
                    type.text = "组队界面";
                    needFlush = false;
                }
                break;
            case (int)NetRequestType.GETROOMLIST:
                if (resp.getRoomListResponse.result)
                {
                    
                    organizePanelEvent.GenerateRoomList(resp.getRoomListResponse);
                }
                break;
            case (int)NetRequestType.JOINROOM:
                organizePanelEvent.gameObject.SetActive(false);
                roomPanelEvent.gameObject.SetActive(true);
                needFlush = false;
                type.text = "组队界面";
                roomPanelEvent.PlayerJoinRoom(resp.joinRoomResponse);
                break;
            case (int)NetRequestType.EXITROOM:
                break;
            case (int)NetRequestType.ROOMOWNEREXIT:
                break;
            case (int)NetRequestType.ROOMOWNERREADYSTARTGAME:
                roomPanelEvent.serverAcceptStartGame(resp.roomOwnerReadyStartGameResponse);
                break;
            case (int)NetRequestType.ROOMOWNERSTARTGAME:
                roomPanelEvent.startGameInit(resp.roomOwnerStartGameResponse);
                break;
        }
    }

    public void flushRoomListRequest()
    {
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.GETROOMLIST;
        GetRoomList getRoomList = new GetRoomList();
        getRoomList.fd = GameGlobalData.fd;
        req.getRoomList = getRoomList;
        socket.sendMSG(PBCSerialize.Serialize(req));
    }


    public void CreateRoomClick()
    {
        createRoomPanel.gameObject.SetActive(!createRoomPanel.gameObject.activeSelf);
    }

    public void ReadyCreateRoomClick()
    {
        createRoomPanel.gameObject.SetActive(false);
        roomName = createRoomPanel.Find("roomName").GetComponent<InputField>().text;
        maxManCount = int.Parse(createRoomPanel.Find("maxManCount").GetComponent<InputField>().text);
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.CREATEROOM;
        CreateRoom createRoom = new CreateRoom();
        createRoom.fd = GameGlobalData.fd;
        createRoom.roomOwner = roomOwner;
        createRoom.maxMan = maxManCount;
        createRoom.roomName = roomName;
        req.createRoom = createRoom;
        socket.sendMSG(PBCSerialize.Serialize(req));
    }

}
