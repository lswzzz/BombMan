using UnityEngine;
using System.Collections;

public class SpecialStateMachine : StateMachineBehaviour {

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerState state = (PlayerState)animator.GetInteger(PlayerAnimationStateTransform.StateString);
        setChildState(animator, state);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerState state = (PlayerState)animator.GetInteger(PlayerAnimationStateTransform.StateString);
        switch (state)
        {
            case PlayerState.Idle:
                PlayerAnimationStateTransform.setIdleStateMachine(animator, PlayerDirection.Down);
                break;
            case PlayerState.Movement:
                PlayerAnimationStateTransform.setMovementStateMachine(animator, PlayerDirection.Down);
                break;
            case PlayerState.Trap:
            case PlayerState.Save:
            case PlayerState.Death:
                setChildState(animator, state);
                break;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    void setChildState(Animator animator, PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Trap:
                TrapLevel level = (TrapLevel)animator.GetInteger(PlayerAnimationStateTransform.TrapString);
                setTrapState(animator, level);
                break;
            case PlayerState.Save:
                animator.Play("Save");
                break;
            case PlayerState.Death:
                animator.Play("Death");
                break;
        }
    }

    void setTrapState(Animator animator, TrapLevel level)
    {
        switch (level)
        {
            case TrapLevel.Trap1:
                animator.Play("Trap");
                break;
            case TrapLevel.Trap2:
                animator.Play("Trap2");
                break;
            case TrapLevel.Trap3:
                animator.Play("Trap3");
                break;
        }
    }
    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
