using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotGenerator{
	
	public static void GeneratedRobot()
    {
        foreach(KeyValuePair<int,PlayerNetWorkData> key in GameGlobalData.playerList)
        {
            PlayerNetWorkData data = key.Value;
            if (data.fd != GameGlobalData.fd)
            {
                GameObject rb = GameObject.Instantiate(Resources.Load("Prefabs/robot") as GameObject);
                rb.GetComponent<RobotInputController>().fd = data.fd;
            }
        }
    }
}
