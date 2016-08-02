﻿using UnityEngine;
using System.Collections;

enum NetRequestType
{
    FORECEEXITGAME = -2,
    HEARTBEAT = -1,
    NONE,
    CONNECTTOSERVER,
    CREATEROOM,
    GETROOMLIST,
    JOINROOM,
    EXITROOM,
    ROOMOWNEREXIT,
    ROOMOWNERREADYSTARTGAME,
    ROOMOWNERSTARTGAME,
    PLAYERGAMEDATA,
    GAMERESULT,
    PLAYEREXITGAME,
    PLAYERRELOADSCENE,
}

