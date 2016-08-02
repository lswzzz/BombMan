using UnityEngine;
using System.Collections;

public class AllPlayerFunctions{
	
	public static void SetInBubble(int row, int col)
    {
        GameGlobalData.player.GetComponent<PlayerMovement>().setInBubble(row, col);
        foreach(RobotInputController robot in GameGlobalData.robotList.Values)
        {
            RobotMovement movement = robot.GetComponent<RobotMovement>();
            movement.setInBubble(row, col);
        }
    }
}
