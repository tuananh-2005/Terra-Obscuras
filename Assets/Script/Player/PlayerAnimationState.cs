using UnityEngine;

public class PlayerAnimationState : StateMachineBehaviour
{
    private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
    private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
    private static readonly int DieHash = Animator.StringToHash("Die");

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset parameters based on the state being entered
        if (stateInfo.shortNameHash != Animator.StringToHash("Run"))
        {
            animator.SetBool(IsRunningHash, false);
        }
        if (stateInfo.shortNameHash != Animator.StringToHash("Jump"))
        {
            animator.SetBool(IsJumpingHash, false);
        }
        if (stateInfo.shortNameHash == Animator.StringToHash("Die_0") ||
            stateInfo.shortNameHash == Animator.StringToHash("Die_1") ||
            stateInfo.shortNameHash == Animator.StringToHash("Die_2"))
        {
            animator.SetTrigger(DieHash); // Ensure Die trigger is cleared if needed
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset parameters when exiting specific states
        if (stateInfo.shortNameHash == Animator.StringToHash("Jump"))
        {
            animator.SetBool(IsJumpingHash, false);
        }
        else if (stateInfo.shortNameHash == Animator.StringToHash("Die_0") ||
                 stateInfo.shortNameHash == Animator.StringToHash("Die_1") ||
                 stateInfo.shortNameHash == Animator.StringToHash("Die_2"))
        {
            animator.ResetTrigger(DieHash); // Reset Die trigger after exiting
        }
    }
}