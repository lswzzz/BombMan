using UnityEngine;
using System.Collections;

public class MovementStateMachine : StateMachineBehaviour {

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerDirection direction = (PlayerDirection)animator.GetInteger(PlayerAnimationStateTransform.DirectionString);
        setChildState(animator, direction);
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerDirection direction = (PlayerDirection)animator.GetInteger(PlayerAnimationStateTransform.DirectionString);
        PlayerState state = (PlayerState)animator.GetInteger(PlayerAnimationStateTransform.StateString);
        switch (state)
        {
            case PlayerState.Idle:
                PlayerAnimationStateTransform.setIdleStateMachine(animator, direction);
                break;
            case PlayerState.Movement:
                setChildState(animator, direction);
                break;
            case PlayerState.Save:
            case PlayerState.Trap:
            case PlayerState.Death:
                PlayerAnimationStateTransform.setSpecialStateMachine(animator, state);
                break;
        }
    }

    void setChildState(Animator animator, PlayerDirection direction)
    {
        switch (direction)
        {
            case PlayerDirection.Left:
                animator.Play("MoveL");
                break;
            case PlayerDirection.Right:
                animator.Play("MoveR");
                break;
            case PlayerDirection.Up:
                animator.Play("MoveU");
                break;
            case PlayerDirection.Down:
                animator.Play("MoveD");
                break;
        }
    }
    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMachineEnter is called when entering a statemachine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash){
    //
    //}

    // OnStateMachineExit is called when exiting a statemachine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
    //
    //}
}
