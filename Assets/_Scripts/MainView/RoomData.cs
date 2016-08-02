using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using proto.clientproto;

public class RoomData : MonoBehaviour {

    private Text OwnerNameText;
    private Text RoomNameText;
    private Text ManCountText;
    private int ownerFd;
    private int manCount;
    private int maxManCount;
	public void SetData(string owner, string roomName, int maxManCount, int currentMan, int fd){
        Start();
        OwnerNameText.text = owner;
        RoomNameText.text = roomName;
        ManCountText.text = string.Format("{0}//{1}", currentMan, maxManCount);
        ownerFd = fd;
        this.manCount = currentMan;
        this.maxManCount = maxManCount;

    }
	
	// Use this for initialization
	void Start () {
        OwnerNameText = transform.Find("OwnerName").GetComponent<Text>();
        RoomNameText = transform.Find("RoomName").GetComponent<Text>();
        ManCountText = transform.Find("ManCount").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate(){
	
	}
	
	void LateUpdate(){
	
	}

    public void ButtonClick()
    {
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.JOINROOM;
        JoinRoom joinRoom = new JoinRoom();
        joinRoom.fd = GameGlobalData.fd;
        joinRoom.roomOwnerFd = ownerFd;
        GameGlobalData.roomOwnerFd = ownerFd;
        GameGlobalData.roomOwnerName = OwnerNameText.text;
        GameGlobalData.roomName = RoomNameText.text;
        GameGlobalData.currentMan = this.manCount;
        GameGlobalData.maxManCount = this.maxManCount;
        req.joinRoom = joinRoom;
        ConnectSocket.getSocketInstance().sendMSG(PBCSerialize.Serialize(req));
    }
}
