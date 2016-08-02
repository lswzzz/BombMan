using UnityEngine;
using System.Collections;

public class PlayerAnimationStateTransform
{

    public static string DirectionString = "Direction";
    public static string StateString = "State";
    public static string IdleL = "Idle.IdleL";
    public static string IdleR = "Idle.IdleR";
    public static string IdleU = "Idle.IdleU";
    public static string IdleD = "Idle.IdleD";
    public static string MoveL = "Movement.MoveL";
    public static string MoveR = "Movement.MoveR";
    public static string MoveU = "Movement.MoveU";
    public static string MoveD = "Movement.MoveD";
    public static string Trap = "Special.Trap";
    public static string Trap2 = "Special.Trap2";
    public static string Trap3 = "Special.Trap3";
    public static string Save = "Special.Save";
    public static string Death = "Special.Death";
    public static string TrapString = "TrapLevel";
    public static void setIdleStateMachine(Animator animator, PlayerDirection direction)
    {
        switch (direction) {
            case PlayerDirection.Left:
                animator.Play(IdleL);
                break;
            case PlayerDirection.Right:
                animator.Play(IdleR);
                break;
            case PlayerDirection.Up:
                animator.Play(IdleU);
                break;
            case PlayerDirection.Down:
                animator.Play(IdleD);
                break;
        }
    }

    public static void setMovementStateMachine(Animator animator, PlayerDirection direction)
    {
        switch (direction)
        {
            case PlayerDirection.Left:
                animator.Play(MoveL);
                break;
            case PlayerDirection.Right:
                animator.Play(MoveR);
                break;
            case PlayerDirection.Up:
                animator.Play(MoveU);
                break;
            case PlayerDirection.Down:
                animator.Play(MoveD);
                break;
        }
    }

    public static void setSpecialStateMachine(Animator animator, PlayerState state)
    {
        switch (state) {
            case PlayerState.Trap:
                animator.Play(Trap);
                break;
            case PlayerState.Save:
                animator.Play(Save);
                break;
            case PlayerState.Death:
                animator.Play(Death);
                break;
        }

    }
}
