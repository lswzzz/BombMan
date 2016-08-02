using UnityEngine;
using System.Collections;
using ProtoBuf;
using proto.serverproto;
using proto.clientproto;

public class SendDataController {

    private ConnectSocket socket;

    private static SendDataController instance;

    public static SendDataController getInstance()
    {
        if (instance == null)
        {
            instance = new SendDataController();
            instance.socket = ConnectSocket.getSocketInstance();
        }
        return instance;
    }
   

    public void ConnectToServerRequest(string name)
    {
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.CONNECTTOSERVER;
        ConnectToServer connectToServer = new ConnectToServer();
        connectToServer.name = name;
        req.connectToServer = connectToServer;
        socket.sendMSG(PBCSerialize.Serialize(req));
    }

    public void CreateRoomRequest(int fd, string roomOwner, int maxMan, string roomName)
    {
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.CREATEROOM;
        CreateRoom createRoom = new CreateRoom();
        createRoom.fd = fd;
        createRoom.roomOwner = roomOwner;
        createRoom.maxMan = maxMan;
        createRoom.roomName = roomName;
        socket.sendMSG(PBCSerialize.Serialize(req));
    }

    public void GetRoomListRequest(int fd)
    {
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.GETROOMLIST;
        GetRoomList getRoomList = new GetRoomList();
        getRoomList.fd = fd;
        socket.sendMSG(PBCSerialize.Serialize(req));
    }

    public void JoinRoomRequest(int fd, int roomOwnerFd)
    {
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.JOINROOM;
        JoinRoom joinRoom = new JoinRoom();
        joinRoom.fd = fd;
        socket.sendMSG(PBCSerialize.Serialize(req));
    }

    public void ExitRoomRequest(int fd, int roomOwnerFd)
    {
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.EXITROOM;
        ExitRoom exitRoom = new ExitRoom();
        exitRoom.fd = fd;
        exitRoom.roomOwnerFd = roomOwnerFd;
        socket.sendMSG(PBCSerialize.Serialize(req));
    }

    public void RoomOwnerExitRequest(int fd)
    {
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.ROOMOWNEREXIT;
        RoomOwnerExit roomOwnerExit = new RoomOwnerExit();
        roomOwnerExit.fd = fd;
        socket.sendMSG(PBCSerialize.Serialize(req));
    }

    public void RoomOwnerReadyStartGameRequest(int fd)
    {
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.ROOMOWNERREADYSTARTGAME;
        RoomOwnerReadyStartGame roomOwnerReadyStartGame = new RoomOwnerReadyStartGame();
        roomOwnerReadyStartGame.fd = fd;
        socket.sendMSG(PBCSerialize.Serialize(req));
    }

    public void RoomOwnerStartGameRequest(int fd)
    {
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.ROOMOWNERSTARTGAME;
        RoomOwnerStartGame roomOwnerStartGame = new RoomOwnerStartGame();
        roomOwnerStartGame.fd = fd;
        socket.sendMSG(PBCSerialize.Serialize(req));
    }

    public void PlayerGameDataRequest(int fd, byte[] bytes)
    {
    //    ProtocolNetRequest req = new ProtocolNetRequest();
    //    req.cmd = (int)NetRequestType.PLAYERGAMEDATA;
    //    PlayerGameData playerGameData = new PlayerGameData();
    //    playerGameData.fd = fd;
    //    playerGameData.playerData = bytes;
    //    socket.sendMSG(PBCSerialize.Serialize(req));
    }

    public void GameResultRequest(int fd, bool result)
    {
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.GAMERESULT;
        GameResult gameResult = new GameResult();
        gameResult.fd = fd;
        gameResult.result = result;
        socket.sendMSG(PBCSerialize.Serialize(req));
    }

    public void PlayerExitGameRequest(int fd)
    {
        ProtocolNetRequest req = new ProtocolNetRequest();
        req.cmd = (int)NetRequestType.PLAYEREXITGAME;
        PlayerExitGame playerExitGame = new PlayerExitGame();
        playerExitGame.fd = fd;
        socket.sendMSG(PBCSerialize.Serialize(req));
    }
}
