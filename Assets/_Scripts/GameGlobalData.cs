using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameGlobalData
{

    public static int fd;
    public static string roomOwnerName;
    public static int roomOwnerFd;
    public static string roomName;
    public static int currentMan;
    public static int maxManCount;
    public static Dictionary<int, PlayerNetWorkData> playerList = new Dictionary<int, PlayerNetWorkData>();
    public static Dictionary<int, RobotInputController> robotList = new Dictionary<int, RobotInputController>();
    public static PlayerInputController player;

}
