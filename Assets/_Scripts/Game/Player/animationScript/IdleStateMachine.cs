using UnityEngine;
using System.Collections;

public class IdleStateMachine : StateMachineBehaviour {

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerDirection direction = (PlayerDirection)animator.GetInteger(PlayerAnimationStateTransform.DirectionString);
        setChildState(animator, direction);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerDirection direction = (PlayerDirection)animator.GetInteger(PlayerAnimationStateTransform.DirectionString);
        PlayerState state = (PlayerState)animator.GetInteger(PlayerAnimationStateTransform.StateString);
        switch (state) {
            case PlayerState.Idle:
                setChildState(animator, direction);
                break;
            case PlayerState.Movement:
                PlayerAnimationStateTransform.setMovementStateMachine(animator, direction);
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
                animator.Play("IdleL");
                break;
            case PlayerDirection.Right:
                animator.Play("IdleR");
                break;
            case PlayerDirection.Up:
                animator.Play("IdleU");
                break;
            case PlayerDirection.Down:
                animator.Play("IdleD");
                break;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
